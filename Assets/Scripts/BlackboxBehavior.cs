using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlasticPipe.PlasticProtocol.Messages;
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

    [SerializeField]
    public StructureType structureType;

    /// <summary>
    /// This is not in real-time seconds. But scaled based on the target animation FPS of 12. 
    /// Therefore 12n -> n seconds.
    /// </summary>
    public int spawnTime = 1;


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
        //In Fixed update it is delayed already. Make sure to consider this .
        Invoke(nameof(RunBehaviorByStructureType), spawnTime * ANIMATION_UPDATE_TIME);



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
        //newBrickRB.AddForce(transform.forward * 3, ForceMode.VelocityChange);
        //newBrickRB.AddTorque(Vector3.one, ForceMode.Impulse);



    }

    private void RunBeltBehavior()
    {
        /*if(!IsAvailableDetector())
        {
            CreateDetector();
        }*/

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
            brickRb.drag = 0;
            brickRb.angularDrag = 0.8f;
            //brickRb.angularVelocity = Vector3.zero;
            //brickRb.AddForce(transform.forward, ForceMode.VelocityChange);

            brick.transform.Translate(transform.forward * 80f * Time.deltaTime * ANIMATION_UPDATE_TIME);


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

            Debug.Log("Sucked up a " + brick.name);


        }


    }

    private void RunJoinerBehavior()
    {


        if(joinerBrickCount > 3 )
        {
            Vector3 spawnPos = transform.position + Vector3.Scale(transform.forward, GetComponent<BrickBehavior>().trueScale );
            GameObject newBrick = Instantiate(oneByFour, spawnPos, transform.rotation, GameObject.Find(OBJECT_FOLDER_NAME).transform);

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
    /*private void CreateDetector()
    {
        Vector3 posOffset = Vector3.Scale(transform.up, BASE_CELL_SIZE);
        posOffset.y *= 2;
        Vector3 spawnPos = transform.position + posOffset;

        GameObject detector = Instantiate(new GameObject(), spawnPos, transform.rotation);

        detector.name = "Detector";

        detector.AddComponent<BoxCollider>();
        detector.GetComponent<BoxCollider>().isTrigger = true;
    }*/


    void OnTriggerEnter(Collider collider){
        
        GameObject hitBrick = collider.gameObject;
        if(hitBrick.CompareTag(BASE_BRICK_TAG))
        {
            detectedBricks.Add(hitBrick);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        detectedBricks.Remove(collider.gameObject);

    }



}
