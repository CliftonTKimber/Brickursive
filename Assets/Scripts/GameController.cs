using System;
using System.Collections;
using System.Collections.Generic;
using static GameConfig;


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

    public Vector3 baseCellSize;

    public GameObject movableGrid;


    void Start()
    {
        brick = availableBricks[0];
        cameraScript = gameCamera.GetComponent<CameraController>();
        baseCellSize = BASE_CELL_SIZE;
        movableGrid = CreateMovableGrid();
        

        gridUtility = new GridUtils();
        raycastUtils = new RaycastUtils();
        gridUtility.Start(this);
        raycastUtils.Start();

        


        



        MakeGhostVersionOfCurrentBrick(brick);

        
    }

    

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {


        //SpawnBrickOnMouseClick();

        ProjectGhostOntoRaycastLocation(); 

        ChangeBrickOnKeyboardInput();
        SpawnBrickIntoTheAirOnKeyDown();
        MoveBrickUnderCursorOnMouseClick();



    }

  
    private void MakeGhostVersionOfCurrentBrick(GameObject chosenBrick)
        {
            if(ghostBrick!=null){
                Destroy(ghostBrick);
            }

            ghostBrick = Instantiate(chosenBrick, transform.position, transform.rotation);


            bool doCollide = false;
            ghostBrick.GetComponent<BoxCollider>().enabled = doCollide;

            ghostBrick.name = GHOST_BRICK_NAME;

           

            if (ghostBrick.GetComponent<MeshRenderer>() != null)
            {
                ghostBrick.GetComponent<MeshRenderer>().material = ghostMaterial;
            }

            
            MeshRenderer[] children;
            children = ghostBrick.GetComponentsInChildren<MeshRenderer>();

            for(int i = 0; i < children.Length; i++){
                children[i].material = ghostMaterial;
            }
            


            Collider[] childColliders;
            childColliders = ghostBrick.GetComponentsInChildren<Collider>();

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
                MakeGhostVersionOfCurrentBrick(brick);
            }
        }
        else if(Input.GetKeyDown("o"))
        {
            if(brickSelector > 0)
             {
                brickSelector--;
                brick = availableBricks[brickSelector];
                MakeGhostVersionOfCurrentBrick(brick);
             }
        }

        
    }

    void SpawnBrickIntoTheAirOnKeyDown()
    {
        if (Input.GetKeyDown("r"))
        {
                Vector3 spawnPos = cameraScript.grabPoint.transform.position;

                Transform objectFolder = GameObject.Find("Objects").transform;

                Instantiate(brick, spawnPos, transform.rotation, objectFolder);
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
        RaycastHit rayHit = raycastUtils.GetRaycastHitFromPhysicsRaycast(cameraScript.transform.position, cameraScript.transform.forward, RAY_LENGTH_FOR_BRICK_SELECTION);
        if (rayHit.collider != null)
        {
            GameObject hitObject = rayHit.collider.gameObject;

            if (!brickFollowCursor)
            {
                string basicTag = BASE_BRICK_TAG;
                string[] childTags = new string[] {SOCKET_TAG_MALE, SOCKET_TAG_FEMALE}; 

                SelectObjectBasedOnTag(hitObject.transform, basicTag, childTags);
            }
            else
            {
                if(mouseTargetedBrick != null)
                {
                    SetObjectAndChildrenColliderEnabled(mouseTargetedBrick, true);
                }

                brickFollowCursor = false;
                mouseTargetedBrick = null;

            }


        }
        else
        {
            if(mouseTargetedBrick != null)
            {
                SetObjectAndChildrenColliderEnabled(mouseTargetedBrick, true);
            }
            brickFollowCursor = false;
            mouseTargetedBrick = null;
        }

    }
 
    private void SelectObjectBasedOnTag(Transform hitTransform, string baseObjectTag, string[] childObjectTags)
    {
        if (hitTransform.CompareTag(baseObjectTag) && !brickFollowCursor)
        {
            brickFollowCursor = true;
            mouseTargetedBrick = hitTransform.gameObject;
            /*if(!Input.GetKey("left ctrl"))
            {  
                IfHasChildrenDetatch(mouseTargetedBrick);
            }*/

        }
        else
        {
            for(int i = 0; i < childObjectTags.Length; i++)
            {
                if (hitTransform.CompareTag(childObjectTags[i]) && !brickFollowCursor)
                {
                    brickFollowCursor = true;
                    /*if(Input.GetKey("left ctrl"))
                    {*/
                        mouseTargetedBrick = IfChildReturnUpperMostParentBesidesRoot(hitTransform.gameObject);
                        brickFollowCursor = true;
                    /*}
                    else
                    {
                        //Debug.Log("work");

                        mouseTargetedBrick = IfSocketReturnBrick(hitTransform.gameObject);
                        mouseTargetedBrick.transform.parent = GameObject.Find("Objects").transform;
                        IfHasChildrenDetatch(mouseTargetedBrick);
                        brickFollowCursor = true;
                    }*/


                    SetObjectAndChildrenColliderEnabled(mouseTargetedBrick, false);

                    MakeGhostVersionOfCurrentBrick(mouseTargetedBrick);
                    break;
                }
            }    
        }
    }

    private GameObject IfChildReturnUpperMostParentBesidesRoot(GameObject targetObject, string excludeObjectName = "Objects")
    {

        int arbNumber = 100;

        Transform parentTransform = targetObject.transform.parent;
        //Debug.Log(parentTransform);
        if(parentTransform != null && parentTransform.name != OBJECT_FOLDER_NAME)
        {
            for(int i = 0; i < arbNumber; i++)
            {
                parentTransform = targetObject.transform.parent;
                
                if(parentTransform.name != excludeObjectName)
                {    
                    targetObject = parentTransform.gameObject;      
                }
                else
                {
                    break;
                }
            }  
            
            return targetObject;
        }
        else 
        {
            return targetObject;
        }


    }
    
    private GameObject IfSocketReturnBrick(GameObject targetObject)
    {
        

        if(targetObject.transform.CompareTag(SOCKET_TAG_MALE) ||targetObject.transform.CompareTag(SOCKET_TAG_FEMALE))
        {
            //Debug.Log("Male or Female");
            return targetObject.transform.parent.gameObject;
        }
        else 
        {
            //Debug.Log("Just Brick");

            return targetObject;
        }


    }
   
    private void IfHasChildrenDetatch(GameObject targetObject)
    {

        if(targetObject.transform.childCount > 0)
        {
            for(int i = 0; i < targetObject.transform.childCount; i++)
            {
                
                GameObject child = targetObject.transform.GetChild(i).gameObject;
                if(child.CompareTag(BASE_BRICK_TAG))
                {
                    child.transform.parent = GameObject.Find(OBJECT_FOLDER_NAME).transform;

                }
                else
                {
                }

            }

        }


    }
    private void MoveSelectedBrickIfToggled()
    {

        if (brickFollowCursor && mouseTargetedBrick != null)
        {

            
            Vector3 grabPointPosition = cameraScript.grabPoint.transform.position; //Child1 = grabpoint
            
             //Make Rigidbody able to move

            Rigidbody brickRb = mouseTargetedBrick.GetComponent<Rigidbody>();
            brickRb.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            brickRb.GetComponent<Rigidbody>().excludeLayers = 0; //nothing

            brickRb.MovePosition(grabPointPosition);
            
            Vector3 tempPos = mouseTargetedBrick.transform.position;

            gridUtility.NEW_SnapObjectToGrid(mouseTargetedBrick, movableGrid, brickFollowCursor, 0.25f);


            
            if(tempPos != mouseTargetedBrick.transform.position) //did it snap?
            {
                mouseTargetedBrick = null;
                brickFollowCursor = false;

            }
            
        }
    }

    private void ProjectGhostOntoRaycastLocation()
    {
        if (ghostBrick!= null && mouseTargetedBrick)
            {  
                

                ghostBrick.SetActive(true);

                Vector3 targetBrickPos = mouseTargetedBrick.transform.position;
                ghostBrick.transform.SetPositionAndRotation(targetBrickPos, mouseTargetedBrick.transform.rotation);

                gridUtility.NEW_SnapObjectToGrid(ghostBrick, movableGrid, brickFollowCursor,  10f);

                ghostBrick.transform.parent = GameObject.Find(OBJECT_FOLDER_NAME).transform;

                SetObjectAndChildrenColliderEnabled(ghostBrick, false);

                if(ghostBrick.transform.position == targetBrickPos) 
                {
                    ghostBrick.SetActive(false);

                }

            }
    }

    public void SetObjectAndChildrenColliderEnabled(GameObject targetObject, bool doCollision)
    {
        Collider[] childColliders;
        childColliders = targetObject.GetComponentsInChildren<Collider>();

        for(int i = 0; i < childColliders.Length; i++){
            childColliders[i].enabled = doCollision;
        }

        targetObject.GetComponent<Collider>().enabled = doCollision;
    }

  
    private GameObject CreateMovableGrid()
    {
        GameObject gridObject = new()
        {
            name = "Movable Grid"
        };

        Grid grid = gridObject.AddComponent<Grid>();
        MeshFilter filter = gridObject.AddComponent<MeshFilter>();
        MeshRenderer renderer = gridObject.AddComponent<MeshRenderer>();


        grid.cellSize = BASE_CELL_SIZE; 
        //filter.mesh = Resources.Load<Mesh>("Plane");
        //renderer.material = Resources.Load<Material>("Default");

        return gridObject;

    }

    /*
        TODO:
        



















    */





    

    

    
}
