using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Overlays;


//using System.Numerics;
using UnityEngine;
using UnityEngine.Animations;



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


    public float divideAmout = 2f;

    public GameObject movableGrid;

    void Start()
    {
        brick = availableBricks[0];
        cameraScript = gameCamera.GetComponent<CameraController>();
        baseCellSize = new(0.78f, 0.32f, 0.78f);
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

            ghostBrick.name = "Ghost Brick";

           

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
                Vector3 spawnPos = cameraScript.transform.GetChild(0).position;

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
        RaycastHit rayHit = raycastUtils.GetRaycastHitFromCameraRay(Input.mousePosition, Camera.main, 20f);
        if (rayHit.collider != null)
        {
            GameObject hitObject = rayHit.collider.gameObject;

            if (!brickFollowCursor)
            {
                string basicTag = "Brick";
                string[] childTags = new string[] {"Male", "Female"}; 

                SelectObjectBasedOnTag(hitObject.transform, basicTag, childTags);
            }
            else
            {
                if(mouseTargetedBrick != null)
                    SetObjectAndChildrenColliderEnabled(mouseTargetedBrick, true);
                brickFollowCursor = false;
                mouseTargetedBrick = null;

            }


        }
        else
        {
            if(mouseTargetedBrick != null)
                SetObjectAndChildrenColliderEnabled(mouseTargetedBrick, true);
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
        }
        else
        {
            for(int i = 0; i < childObjectTags.Length; i++)
            {
                if (hitTransform.CompareTag(childObjectTags[i]) && !brickFollowCursor)
                {
                    brickFollowCursor = true;
                    mouseTargetedBrick = IfChildReturnUpperMostParentBesidesRoot(hitTransform.gameObject);

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
        if(parentTransform != null && parentTransform.name != "Objects")
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
    private void MoveSelectedBrickIfToggled()
    {

        if (brickFollowCursor && mouseTargetedBrick != null)
        {
            
            Vector3 grabPointPosition = cameraScript.transform.GetChild(0).transform.position;


            Rigidbody brickRb = mouseTargetedBrick.GetComponent<Rigidbody>();

            brickRb.MovePosition(grabPointPosition);
            
            Vector3 tempPos = mouseTargetedBrick.transform.position;

            gridUtility.SnapObjectToGrid(mouseTargetedBrick, movableGrid, brickFollowCursor);

            
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

                Vector3 tempPos = mouseTargetedBrick.transform.position;
                ghostBrick.transform.SetPositionAndRotation(tempPos, mouseTargetedBrick.transform.rotation);

                gridUtility.SnapObjectToGrid(ghostBrick, movableGrid, brickFollowCursor,  6f);

                ghostBrick.transform.parent = GameObject.Find("Objects").transform;

                SetObjectAndChildrenColliderEnabled(ghostBrick, false);

                if(ghostBrick.transform.position == tempPos) //did it NOT snap?
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
        GameObject gridObject = new();
        gridObject.name = "Movable Grid";

        Grid grid = gridObject.AddComponent<Grid>();
        MeshFilter filter = gridObject.AddComponent<MeshFilter>();
        MeshRenderer renderer = gridObject.AddComponent<MeshRenderer>();


        grid.cellSize = new Vector3(0.78f, 0.32f, 0.78f); 
        //filter.mesh = Resources.Load<Mesh>("Plane");
        //renderer.material = Resources.Load<Material>("Default");

        return gridObject;

    }

    /*
        TODO:
        
        Add Snapping logic to Moved Bricks - Raycast from all Brick Male & Female Colliders

        Add Inheritence Logic after Snapping into Place.
        

        Modify Snapping Logic to work from any Angle. Snap must be relative to place being snapped.




















    */





    

    

    
}
