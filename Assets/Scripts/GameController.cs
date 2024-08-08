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

    private bool isBrickFollowingCursor = false;

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
        if (ghostBrick != null)
        {
            Destroy(ghostBrick);
        }

        ghostBrick = Instantiate(chosenBrick, transform.position, transform.rotation);
        ghostBrick.name = GHOST_BRICK_NAME;

        SetObjectAndChildrenColliderEnabled(ghostBrick, false);
        SetObjectAndChildrenMaterial(ghostBrick, ghostMaterial);
    }

 

    void ChangeBrickOnKeyboardInput()
    {

        if(Input.GetKeyDown("p"))
        {
            if(brickSelector < availableBricks.Count - 1)
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
        if (rayHit.collider == null)
        {
            SetObjectAndChildrenColliderEnabled(mouseTargetedBrick, true);
            isBrickFollowingCursor = false;
            mouseTargetedBrick = null;
            return;
        }
            
        if (!isBrickFollowingCursor)
        {
            GameObject hitObject = rayHit.collider.gameObject;
            SelectObjectBasedOnTag(hitObject);

        }
        else
        {
            SetObjectAndChildrenColliderEnabled(mouseTargetedBrick, true);
            isBrickFollowingCursor = false;
            mouseTargetedBrick = null;

        }
    }
 
     private void MoveSelectedBrickIfToggled()
    {
        if(!isBrickFollowingCursor)
        {
            return;
        }
        if (mouseTargetedBrick == null)
        {
            return;
        }
       
        Vector3 grabPointPosition = cameraScript.grabPoint.transform.position;
        
            //Make Rigidbody able to move

        //Rigidbody brickRb = mouseTargetedBrick.GetComponent<Rigidbody>();
        //brickRb.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        //brickRb.GetComponent<Rigidbody>().excludeLayers = 0; //nothing

        //brickRb.MovePosition(grabPointPosition);

        mouseTargetedBrick.transform.position = grabPointPosition;
        
        Vector3 tempPos = mouseTargetedBrick.transform.position;

        gridUtility.SnapObjectToGrid(mouseTargetedBrick, movableGrid, isBrickFollowingCursor);

        if(tempPos != mouseTargetedBrick.transform.position) //did it snap?
        {
            mouseTargetedBrick = null;
            isBrickFollowingCursor = false;

        }
            
    }

    private void ProjectGhostOntoRaycastLocation()
    {
        if (ghostBrick == null || mouseTargetedBrick == null)
            {  
                return;
            }

            ghostBrick.SetActive(true);

            Vector3 targetBrickPos = mouseTargetedBrick.transform.position;
            ghostBrick.transform.SetPositionAndRotation(targetBrickPos, mouseTargetedBrick.transform.rotation);

            gridUtility.SnapObjectToGrid(ghostBrick, movableGrid, isBrickFollowingCursor,  10f);

            ghostBrick.transform.parent = GameObject.Find(OBJECT_FOLDER_NAME).transform;

            SetObjectAndChildrenColliderEnabled(ghostBrick, false);

            if(ghostBrick.transform.position == targetBrickPos) 
            {
                //ghostBrick.SetActive(false);

            }     
    }

    private void SelectObjectBasedOnTag(GameObject hitObject)
    {
        if(hitObject.CompareTag(BASE_BRICK_TAG) || hitObject.CompareTag(SOCKET_TAG_MALE) || hitObject.CompareTag(SOCKET_TAG_FEMALE) )
        {
            isBrickFollowingCursor = true;
            mouseTargetedBrick = IfChildReturnUpperMostParentBesidesRoot(hitObject);

            SetObjectAndChildrenColliderEnabled(mouseTargetedBrick, false);

            MakeGhostVersionOfCurrentBrick(mouseTargetedBrick);

            return;
        }           
    }

    static public GameObject IfChildReturnUpperMostParentBesidesRoot(GameObject targetObject)
    {

        int arbNumber = 100;

        Transform parentTransform = targetObject.transform.parent;

        if(parentTransform == null)
        {
            return targetObject;
        }

        if (parentTransform.name == OBJECT_FOLDER_NAME)
        {
            return targetObject;
        }


        for(int i = 0; i < arbNumber; i++)
        {
            parentTransform = targetObject.transform.parent;

            if(parentTransform == null)
            {
                break;
            }
            if(parentTransform.name == OBJECT_FOLDER_NAME)
            {
                break;
            }
   
            targetObject = parentTransform.gameObject;      
          
        }  

        return targetObject;


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

    public void SetObjectAndChildrenColliderEnabled(GameObject targetObject, bool doCollision)
    {
        if (targetObject == null)
        {
            return;
        }

        targetObject.GetComponent<Collider>().enabled = doCollision;

        Collider[] childColliders;
        childColliders = targetObject.GetComponentsInChildren<Collider>();

        for(int i = 0; i < childColliders.Length; i++){

            childColliders[i].enabled = doCollision;
        }
    }

    public void SetObjectAndChildrenMaterial(GameObject targetObject, Material material)
    {
        if (targetObject.GetComponent<MeshRenderer>() != null)
        {
            targetObject.GetComponent<MeshRenderer>().material = material;
        }


        MeshRenderer[] children;
        children = targetObject.GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < children.Length; i++)
        {
            children[i].material = material;
        }
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
