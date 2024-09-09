using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlasticPipe.PlasticProtocol.Messages;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using static GameConfig;

public class BlackboxBehavior : MonoBehaviour
{    public enum StructureType
    {
        /// <summary>
        /// This Structure takes one of the child bricks, and instantiates another.
        /// </summary>
        Replicator,

        Belt,

        Vacuum,

        Joiner

        
    }


    public StructureType structureType;

    /// <summary>
    /// This is not in real-time seconds. But scaled based on the target animation FPS of 12. 
    /// Therefore 12n -> n seconds.
    /// </summary>
    public int spawnTime = 1;

    public int powerLevel = 1;


    [SerializeField]
    private List<GameObject> detectedBricks;

    private int joinerBrickCount = 0;

    public GameObject oneByOne;
    public GameObject oneByFour;


    void Start()
    {
        detectedBricks = new();

      
    }

    void FixedUpdate()
    {

        if(powerLevel > 0)
        {
            //In Fixed update it is delayed already. Make sure to consider this .
            Invoke(nameof(RunBehaviorByStructureType), spawnTime * ANIMATION_UPDATE_TIME);
        }



    }



    private void RunBehaviorByStructureType()
    {

        switch (structureType)
        {
            case StructureType.Replicator:
                RunReplicatorBehavior();
                break;
            
            case StructureType.Belt:
                RunBeltBehavior();
                break;

            case StructureType.Vacuum:
                RunVacuumBehavior();
                break;

            case StructureType.Joiner:
                RunJoinerBehavior();
                break;

            default:
                break;

        };

        CancelInvoke();
    }


    private void RunReplicatorBehavior()
    {
        Transform childToReplicate = GetFirstValidChildTransformInHeirarchy();

        if(childToReplicate == null)
        {
            return;
        }

        Vector3 spawnPos = transform.position + Vector3.Scale(transform.forward, GetComponent<BrickBehavior>().trueScale );

        GameObject newBrick = Instantiate(childToReplicate.gameObject, spawnPos, transform.rotation, GameObject.Find(OBJECT_FOLDER_NAME).transform);

        Rigidbody newBrickRB = newBrick.GetComponent<Rigidbody>();

        newBrickRB.isKinematic = false;
        newBrickRB.useGravity = true;
        newBrickRB.excludeLayers = 0;
        //newBrickRB.AddForce(transform.forward * 3, ForceMode.VelocityChange);
        //newBrickRB.AddTorque(Vector3.one, ForceMode.Impulse);



    }

    private void RunBeltBehavior()
    {
        SetupBelt();

        if(detectedBricks.Count <= 0)
        {
            return;
        }

        for(int i = 0; i < detectedBricks.Count; i++)
        {
            GameObject brick = detectedBricks[i];

            if(brick == null)
            {
                Debug.Log("A null object is in the list");
                detectedBricks.Remove(brick);
                continue;

            }

            Rigidbody brickRb = brick.GetComponent<Rigidbody>();

            brickRb.useGravity = false;
            brickRb.constraints = RigidbodyConstraints.FreezeAll;


            if(brick.GetComponent<BlackboxBehavior>() == null)
            {
                brickRb.AddForce(Vector3.Scale(transform.up, -brickRb.velocity), ForceMode.VelocityChange);
            }

            Vector3 moveDirection = Quaternion.Inverse(brick.transform.rotation) * transform.forward;

            brick.GetComponent<BrickBehavior>().TranslateBrick(gameObject, 
            moveDirection * 100f * BASE_CELL_SIZE.y  * Time.deltaTime);

            //TranslateBrick(gameObject, moveDirection * 400f * BASE_CELL_SIZE.x * Time.deltaTime * ANIMATION_UPDATE_TIME);


        }





    }

    private void SetupBelt()
    {
        if(GetComponents<Collider>().Length <= 1)
        {
            Vector3 trueScale = GetComponent<BrickBehavior>().trueScale;

            BoxCollider detector = gameObject.AddComponent<BoxCollider>();

            detector.center = new Vector3(0f, trueScale.y / 2f, 0f);
            detector.size = new Vector3(trueScale.x - (STUD_HEIGHT * 2),
                                        trueScale.y - (STUD_HEIGHT * 2),
                                        trueScale.z - (STUD_HEIGHT * 2));

            detector.isTrigger = true;
        } 


    }

    private void RunVacuumBehavior()
    {
        if(detectedBricks.Count <= 0)
        {
            return;
        }


        for(int i = 0; i < detectedBricks.Count; i++)
        {
            GameObject brick = detectedBricks[i];

            detectedBricks.Remove(brick);
            Destroy(brick);

            //Debug.Log("Sucked up a " + brick.name);


        }


    }

    private void RunJoinerBehavior()
    {

        

        for(int i = 0; i < detectedBricks.Count; i++)
        {
            GameObject brick = detectedBricks[i];
            if(brick.GetComponent<BlackboxBehavior>() != null || 
               brick.GetComponentInChildren<BlackboxBehavior>() != null)
               {
                return;
               }
        }




        if(joinerBrickCount > 3 )
        {
            Vector3 spawnPos = transform.position + Vector3.Scale(transform.forward, GetComponent<BrickBehavior>().trueScale );
            
            
            GameObject newBrick = Instantiate(oneByFour, spawnPos, Quaternion.identity, GameObject.Find(OBJECT_FOLDER_NAME).transform);

            joinerBrickCount = 0;

            return;

        }

        if(detectedBricks.Count <= 0)
        {
            return;
        }

        for(int i = 0; i < detectedBricks.Count; i++)
        {
            GameObject brick = detectedBricks[i];
            if(brick.name == "1x1 Brick(Clone)")
                { joinerBrickCount++;}

            detectedBricks.Remove(brick);
            Destroy(brick);

            


        }


    }
    private Transform GetFirstValidChildTransformInHeirarchy()
    {
        Transform chosenChild = null;
        for(int i = 0; i < transform.childCount; i++)
        {

            Transform child = transform.GetChild(i);

            if(child.CompareTag(SOCKET_TAG_FEMALE) || child.CompareTag(SOCKET_TAG_MALE))
            {
                continue;
            }

            if(child.GetComponent<XRGrabInteractable>() == null)
            {
                continue;
            }

            chosenChild = child;



        }

        return chosenChild;

    }

    private bool IsAvailableDetector()
    {
        bool isAvailable = false;
        for(int i = 0; i < transform.childCount; i++)
        {

            Transform child = transform.GetChild(i);

            if(child.name == "Detector")
            {
                isAvailable = true;
                break;
            }
        }


        return isAvailable;

    }



    void OnTriggerEnter(Collider collider)
    {
        
        if(collider.isTrigger)
        {
            return;
        }

        GameObject hitBrick = collider.gameObject;
        
        
        if(!hitBrick.CompareTag(BASE_BRICK_TAG))
        {
            return;
        }

        if(hitBrick.GetComponent<BrickBehavior>().highestParent == GetComponent<BrickBehavior>().highestParent)
        {
            return;
        }

        if(hitBrick.transform.parent != null &&
           hitBrick.transform.parent.gameObject == gameObject)
        {
            return;
        }

        if(transform.parent != null &&
          hitBrick == transform.parent.gameObject)
        {
            return;
        }

        
        


        for (int i = 0; i < detectedBricks.Count; i++)
        {
            if(hitBrick == detectedBricks[i])
            {
                return;
            }  
        }

        detectedBricks.Add(hitBrick);

        if(structureType == StructureType.Belt)
        {
            hitBrick.GetComponent<BrickBehavior>().belts.Add(gameObject);
        }

        
        
    }

    void OnTriggerExit(Collider collider)
    {


        GameObject hitBrick = collider.gameObject;
        if(!collider.CompareTag(BASE_BRICK_TAG))
        {
            return;
        }

        if(hitBrick.GetComponent<BrickBehavior>().highestParent == GetComponent<BrickBehavior>().highestParent)
        {
            return;
        }

        detectedBricks.Remove(hitBrick);

        if(structureType == StructureType.Belt)
        {
            hitBrick.GetComponent<BrickBehavior>().belts.Remove(gameObject);

            if(hitBrick.GetComponent<BrickBehavior>().belts.Count <= 0)
            {

                Rigidbody brickRb = hitBrick.GetComponent<Rigidbody>();

                brickRb.useGravity = true;
                brickRb.constraints = RigidbodyConstraints.None;
                //brickRb.AddForce(transform.forward * 15f, ForceMode.Impulse);
            }
        }


    }



}
