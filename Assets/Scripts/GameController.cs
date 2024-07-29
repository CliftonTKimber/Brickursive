using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Overlays;


//using System.Numerics;
using UnityEngine;



/*NOTE: To make things work to scale, the smallest unit is 3.2mm mapped to 1m
This is the height of the smallest brick, not including the stud half.
*/

public class GameController : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject brick;

    public List<GameObject> availableBricks;

    public float spawnDistance = 5f;

    private bool brickFollowCursor = false;

    private GameObject mouseTargetedBrick;

    private int brickSelector = 0;

    private GameObject ghostBrick;
    public Material ghostMaterial;



    public Camera gameCamera;

    public float cameraMoveSpeed = 5f;

    private CameraController cameraScript;

    private GridUtils gridUtility;

    private RaycastUtils raycastUtils;


    private Vector3 baseCellSize;

    public float divideAmout = 2f;

    void Start()
    {
        brick = availableBricks[0];
        cameraScript = gameCamera.GetComponent<CameraController>();

        gridUtility = new GridUtils();
        raycastUtils = new RaycastUtils();
        raycastUtils.Start();


        baseCellSize = new Vector3(0.78f, 0.32f, 0.78f);



        //MakeGhostVersionOfCurrentBrick();

        
    }

    

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {


        //SpawnBrickOnMouseClick();

        //ProjectGhostOntoRaycastLocation(); 

        ChangeBrickOnKeyboardInput();
        SpawnBrickIntoTheAirOnKeyDown();
        MoveBrickUnderCursorOnMouseClick();



    }

    private void MakeGhostVersionOfCurrentBrick()
        {
            if(ghostBrick!=null){
                Destroy(ghostBrick);
            }

            ghostBrick = Instantiate(brick, transform.position, transform.rotation);


            bool doCollide = false;
            ghostBrick.GetComponent<BoxCollider>().enabled = doCollide;


           

            if (ghostBrick.GetComponent<MeshRenderer>() != null)
                ghostBrick.GetComponent<MeshRenderer>().material = ghostMaterial;

            {
                MeshRenderer[] children;
                children = ghostBrick.GetComponentsInChildren<MeshRenderer>();

                //Debug.Log(children.Length);
                for(int i = 0; i < children.Length; i++){
                    children[i].material = ghostMaterial;
                }
            }


            BoxCollider[] childColliders;
            childColliders = ghostBrick.GetComponentsInChildren<BoxCollider>();

            for(int i = 0; i < childColliders.Length; i++){
                childColliders[i].enabled = doCollide;
            }
            
            

        }

    void ChangeBrickOnKeyboardInput()
    {

        if(Input.GetKeyDown("p"))
        {
            if(brickSelector < availableBricks.Count-1)
             {
                brickSelector++;   
                brick = availableBricks[brickSelector];
                MakeGhostVersionOfCurrentBrick();
            }
        }
        else if(Input.GetKeyDown("o"))
        {
            if(brickSelector > 0)
             {
                brickSelector--;
                brick = availableBricks[brickSelector];
                MakeGhostVersionOfCurrentBrick();
             }
        }

        
    }

    void SpawnBrickOnMouseClick()
    {
        int left = 0;
        if (Input.GetMouseButtonUp(left))
        {
                Vector3 mousePos = Input.mousePosition;
                Vector3 finalGridPos = gridUtility.GetFinalGridPosition(brick, mousePos, baseCellSize, Camera.main, raycastUtils);

                Vector3 brickPosition = finalGridPos;

                //Gives the bricks a little randomness to make things feel less steril
                float randX = UnityEngine.Random.Range(-0.02f,0.01f);
                float randY = UnityEngine.Random.Range(-0.02f,0.01f);
                float randZ = UnityEngine.Random.Range(-0.02f,0.01f);


                GameObject temporaryBrick = Instantiate(brick, brickPosition, transform.rotation);

                temporaryBrick.transform.localScale += new Vector3(randX, randY, randZ);   

        }
    }


    void SpawnBrickIntoTheAirOnKeyDown()
    {
        if (Input.GetKeyDown("r"))
        {
                Vector3 spawnPos = cameraScript.transform.position + Vector3.Scale(cameraScript.transform.forward, new Vector3(1,1,spawnDistance));

                Transform objectFolder = GameObject.Find("Objects").transform;


                GameObject temporaryBrick = Instantiate(brick, spawnPos, transform.rotation, objectFolder);

                


        }

    }

    void MoveBrickUnderCursorOnMouseClick()
    {

        if (Input.GetMouseButtonDown(0))
        {
            ToggleBrickMovementSelection();
        }

        MoveSelectedBrickIfToggled();

    }

    private void ToggleBrickMovementSelection()
    {
        RaycastHit rayHit = raycastUtils.GetRaycastHitFromCameraRay(Input.mousePosition, Camera.main);
        if (rayHit.collider != null)
        {
            GameObject hitObject = rayHit.collider.gameObject;

            if (!brickFollowCursor)
            {
                string basicTag = "Brick";
                string[] childTags = new string[] {"Male", "Female"};

                SelectObjectBasedOnTag(hitObject, basicTag, childTags);
            }
            else
            {
                brickFollowCursor = false;
                mouseTargetedBrick = null;
            }


        }
        else
        {
            brickFollowCursor = false;
            mouseTargetedBrick = null;
        }

    }

    private void SelectObjectBasedOnTag(GameObject hitObject, string baseObjectTag, string[] childObjectTags)
    {
        if (hitObject.CompareTag(baseObjectTag) && !brickFollowCursor)
        {
            brickFollowCursor = true;
            mouseTargetedBrick = hitObject;
        }
        else
        {
            for(int i = 0; i < childObjectTags.Length; i++)
            {
                if (hitObject.CompareTag(childObjectTags[i]) && !brickFollowCursor)
                {
                    brickFollowCursor = true;
                    mouseTargetedBrick = hitObject.transform.parent.gameObject;
                    break;
                }
            }    
        }
    }

    private void MoveSelectedBrickIfToggled()
    {
        if (brickFollowCursor && mouseTargetedBrick != null)
        {
            Vector3 grabPointPosition = cameraScript.transform.GetChild(0).transform.position;


            Rigidbody brickRb = mouseTargetedBrick.GetComponent<Rigidbody>();

            brickRb.MovePosition(grabPointPosition);
        }
    }

    void ProjectGhostOntoRaycastLocation()
    {
        if (ghostBrick!= null && ghostBrick.activeSelf)
            {  
                
                Vector3 mousePos = Input.mousePosition;
                Vector3 finalGridPos = gridUtility.GetFinalGridPosition(ghostBrick, mousePos, baseCellSize, Camera.main, raycastUtils);
                
                ghostBrick.transform.position = finalGridPos;

            }
    }


    void ShrinkBoxColliderbounds(GameObject targetBrick)
    {
        Vector3 boxSize = targetBrick.GetComponent<BoxCollider>().size;
        targetBrick.GetComponent<BoxCollider>().size = Vector3.Scale(new Vector3(0.1f,0.1f,0.1f), boxSize);


    }





    

    

    
}
