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


    private Vector3 baseCellSize;


    public float divideAmout = 2f;

    public GameObject movableGrid;

    void Start()
    {
        brick = availableBricks[0];
        cameraScript = gameCamera.GetComponent<CameraController>();
        movableGrid = CreateMovableGrid();

        gridUtility = new GridUtils();
        raycastUtils = new RaycastUtils();
        gridUtility.Start(this);
        raycastUtils.Start();

        


        baseCellSize = gridUtility.baseCellSize;



        MakeGhostVersionOfCurrentBrick();

        
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

    private void MakeGhostVersionOfCurrentBrick()
        {
            if(ghostBrick!=null){
                Destroy(ghostBrick);
            }

            ghostBrick = Instantiate(brick, transform.position, transform.rotation);


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
                RaycastHit rayHit = raycastUtils.GetRaycastHitFromCameraRay(Input.mousePosition, Camera.main);
                Vector3 finalGridPos = gridUtility.GetFinalGridPosition(brick, rayHit, baseCellSize, raycastUtils);

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
                   // mouseTargetedBrick = hitObject.transform.parent.gameObject;
                    break;
                }
            }    
        }
    }

    private GameObject IfChildReturnParent(GameObject targetObject, string excludeObjectName = "Objects")
    {
        Transform parentTransform = targetObject.transform.parent;
        if(parentTransform != null && parentTransform.name != excludeObjectName)
        {
            return parentTransform.gameObject;
        }
        else 
        {
            return targetObject;
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
                ghostBrick.transform.rotation = mouseTargetedBrick.transform.rotation;
                ghostBrick.transform.position = tempPos;
                
                gridUtility.SnapObjectToGrid(ghostBrick, movableGrid, brickFollowCursor,  6f);

                ghostBrick.transform.parent = GameObject.Find("Objects").transform;

                SetObjectAndChildrenColliderEnabled(ghostBrick, false);

                if(ghostBrick.transform.position == tempPos) //did it NOT snap?
            {
                //ghostBrick.SetActive(false);

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

    void ShrinkBoxColliderbounds(GameObject targetBrick)
    {
        Vector3 boxSize = targetBrick.GetComponent<BoxCollider>().size;
        targetBrick.GetComponent<BoxCollider>().size = Vector3.Scale(new Vector3(0.1f,0.1f,0.1f), boxSize);


    }

    private GameObject CreateMovableGrid()
    {
        GameObject gridObject = new();
        gridObject.name = "Movable Grid";

        //SetParent has WorldPositionStays argument that can decide if world/local values should be used

        //gridObject.transform.SetParent(gameObject.transform);


        Grid grid = gridObject.AddComponent<Grid>();
        MeshFilter filter = gridObject.AddComponent<MeshFilter>();
        MeshRenderer renderer = gridObject.AddComponent<MeshRenderer>();


        grid.cellSize = new Vector3(0.78f, 0.32f, 0.78f);  
        filter.mesh = Resources.Load<Mesh>("Capsule");
        renderer.material = Resources.Load<Material>("Default");

         

        return gridObject;

    }

    /*
        TODO:
        
        Add Snapping logic to Moved Bricks - Raycast from all Brick Male & Female Colliders

        Add Inheritence Logic after Snapping into Place.
        

        Modify Snapping Logic to work from any Angle. Snap must be relative to place being snapped.




















    */





    

    

    
}
