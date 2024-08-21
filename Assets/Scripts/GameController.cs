using System;
using System.Collections;
using System.Collections.Generic;
using static GameConfig;


//using System.Numerics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor.XR.Interaction.Toolkit;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using Unity.XR.CoreUtils;



public class GameController : MonoBehaviour
{
    #region params

    public GameObject brick;

    public List<GameObject> availableBricks;

    public List<GameObject> controllers;

    private GameObject rightController;

    private GameObject leftController;

    public float spawnDistance = 5f;

    private GameObject leftTargetedBrick;
    public GameObject rightTargetedBrick;


    private int brickSelector = 0;

    public GameObject leftGhostBrick;
    public GameObject rightGhostBrick;
    public Material ghostMaterial;



    public Camera gameCamera;

    public float cameraMoveSpeed = 5f;

    private CameraController cameraScript;

    private GridUtils gridUtility;

    private RaycastUtils raycastUtils;

    public Vector3 baseCellSize;

    public GameObject movableGrid;

    private bool preventLeftGrab;
    private bool preventRightGrab;

    #endregion


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


        
        //MakeGhostVersionOfCurrentBrick(brick, leftGhostBrick);

        
    }

    

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {


        ChangeBrickOnKeyboardInput();
        SpawnBrickIntoTheAirOnKeyDown();
        SpawnBrickIntoTheAirOnControllerButtonDown(controllers[0], controllers[1]);

        MoveBricksIfControllersGrab();


        AllowControllerSelection(controllers[0]);
        AllowControllerSelection(controllers[1]);

        ChangeInteractorLayerMaskOnTrigger(controllers[0]);
        ChangeInteractorLayerMaskOnTrigger(controllers[1]);




    }

    private void AllowControllerSelection(GameObject xrController)
    {   
        NearFarInteractor nearFarInteractor = xrController.GetComponentInChildren<NearFarInteractor>();

        if (!nearFarInteractor.hasSelection && nearFarInteractor.selectInput.ReadValue() == 0)
            nearFarInteractor.allowSelect = true;
    }

    public void BeginSnapping(GameObject targetBrick, GameObject usedController)
    {
        NearFarInteractor nfInteractor = usedController.GetComponentInChildren<NearFarInteractor>();

        if(usedController == controllers[0])
        {
            leftTargetedBrick = targetBrick;
            leftGhostBrick = MakeGhostVersionOfCurrentBrick(targetBrick, leftGhostBrick);
            leftController = controllers[0];

            nfInteractor.interactionLayers = 1 <<  LAYER_MASK_INTERACT;
            targetBrick.GetComponent<XRGrabInteractable>().interactionLayers = 1 << LAYER_MASK_INTERACT;

        }
        else if(usedController == controllers[1])
        {
            rightTargetedBrick = targetBrick;
            rightGhostBrick = MakeGhostVersionOfCurrentBrick(targetBrick, rightGhostBrick);
            rightController = controllers[1];

            nfInteractor.interactionLayers =  1 << LAYER_MASK_INTERACT;
            targetBrick.GetComponent<XRGrabInteractable>().interactionLayers = 1 << LAYER_MASK_INTERACT;

        }
    }

    public void EndSnapping(GameObject usedController)
    {
        if(usedController == controllers[0])
        {
            leftTargetedBrick = null;
            GameObject.Destroy(leftGhostBrick);
            leftController = null;

        }
        else if(usedController == controllers[1])
        {
            rightTargetedBrick = null;
            GameObject.Destroy(rightGhostBrick);
            rightController = null;

        }
    }
  
    private GameObject MakeGhostVersionOfCurrentBrick(GameObject chosenBrick, GameObject ghostBrick)
    {
        if(chosenBrick == null)
        {
            return null;
        }

        if (ghostBrick != null)
        {
            Destroy(ghostBrick);
        }

        ghostBrick = Instantiate(chosenBrick, transform.position, transform.rotation);
        ghostBrick.name = GHOST_BRICK_NAME;

        ghostBrick.GetComponent<MeshRenderer>().enabled = false;


        SetObjectAndChildrenColliderEnabled(ghostBrick, false);
        SetObjectAndChildrenMaterial(ghostBrick, ghostMaterial);

        return ghostBrick;
    }

    void ChangeBrickOnKeyboardInput()
    {

        if(Input.GetKeyDown("p"))
        {
            if(brickSelector < availableBricks.Count - 1)
             {
                brickSelector++;   
                brick = availableBricks[brickSelector];
                leftGhostBrick = MakeGhostVersionOfCurrentBrick(brick, leftGhostBrick);
            }
        }
        else if(Input.GetKeyDown("o"))
        {
            if(brickSelector > 0)
             {
                brickSelector--;
                brick = availableBricks[brickSelector];
                rightGhostBrick = MakeGhostVersionOfCurrentBrick(brick, leftGhostBrick);
             }
        }

        
    }

    void SpawnBrickIntoTheAirOnKeyDown()
    {
        if (Input.GetKeyDown("space"))
        {
                Vector3 spawnPos = cameraScript.grabPoint.transform.position;

                Transform objectFolder = GameObject.Find(OBJECT_FOLDER_NAME).transform;

                Instantiate(brick, spawnPos, transform.rotation, objectFolder);
        }

        //

        if(Input.GetKey("up"))
        {

            GameObject.Find("Camera Offset").transform.position -= new Vector3(0,1,0);

        }
        if(Input.GetKey("down"))
        {

            GameObject.Find("Camera Offset").transform.position += new Vector3(0,1,0);

        }

    }

    private void SpawnBrickIntoTheAirOnControllerButtonDown(GameObject leftController, GameObject rightController)
    {


        XRDirectInteractor leftInteractor = leftController.GetComponentInChildren<XRDirectInteractor>();
        //NearFarInteractor rightInteractor = rightController.GetComponentInChildren<NearFarInteractor>();

        //.Log(Input.GetButtonDown());




        //Does not release from pressed state
        //Debug.Log(Input.GetButtonDown("XRI_Left_PrimaryButton"));

        /*if (Input.GetButtonDown("XRI_Left_PrimaryButton"))
        if(Input.GetKeyDown("r"))
        {
            //Debug.Log("Left button!");
            Transform grabSphere = controllers[0].transform.GetChild(0);
            Vector3 spawnPos = grabSphere.position;

            Transform objectFolder = GameObject.Find("Objects").transform;

            Instantiate(brick, spawnPos, transform.rotation, objectFolder);
        }

        if (Input.GetButtonDown("XRI_Right_PrimaryButton"))
        {
            Debug.Log("right button!");
            Transform grabSphere = controllers[1].transform.GetChild(0);
            Vector3 spawnPos = grabSphere.position;

            Transform objectFolder = GameObject.Find("Objects").transform;

            Instantiate(brick, spawnPos, transform.rotation, objectFolder);

            
        }*/

    }

    void ChangeInteractorLayerMaskOnTrigger(GameObject xrController)
    {
        NearFarInteractor nfInteractor = xrController.GetComponentInChildren<NearFarInteractor>();

        UnityEngine.XR.Interaction.Toolkit.Inputs.Readers.XRInputButtonReader activateInput = nfInteractor.activateInput;
        UnityEngine.XR.Interaction.Toolkit.Inputs.Readers.XRInputButtonReader selectInput = nfInteractor.selectInput;

        //Layermask uses ~ to negate associated part.
        if(activateInput.ReadValue() > 0)
        {
            nfInteractor.interactionLayers = 1 << LAYER_MASK_ONLY_PLUCKABLE;
        }

        if(selectInput.ReadValue() > 0)
        {
            nfInteractor.interactionLayers = 1 <<  LAYER_MASK_INTERACT;
        }
    }


    void MoveBricksIfControllersGrab()
    {
        MoveSelectedBrickToControllerIfToggled(leftTargetedBrick, leftController);
        ProjectGhostOntoControllerLocation(leftTargetedBrick, leftGhostBrick);

        MoveSelectedBrickToControllerIfToggled(rightTargetedBrick, rightController);
        ProjectGhostOntoControllerLocation(rightTargetedBrick, rightGhostBrick);

    }

    private void MoveSelectedBrickToControllerIfToggled(GameObject targetedBrick, GameObject xrController)
    {
        if (targetedBrick == null || xrController == null)
        {
            return;
        }

        Vector3 originalPos = targetedBrick.transform.position;

        gridUtility.SnapObjectToGrid(targetedBrick, movableGrid, true);

        DisableControllerSelectionIfBrickSnapped(targetedBrick, xrController, originalPos);

    }

    private void DisableControllerSelectionIfBrickSnapped(GameObject targetedBrick, GameObject xrController, Vector3 originalPos)
    {
        if (originalPos != targetedBrick.transform.position)
        {
            EndSnapping(xrController);
            xrController.GetComponentInChildren<NearFarInteractor>().allowSelect = false;

        }
    }


   

    private void ProjectGhostOntoControllerLocation(GameObject targetedBrick, GameObject ghostBrick)
    {
        if (ghostBrick == null || targetedBrick == null)
            {  
                return;
            }

            //ghostBrick.SetActive(true);

            ghostBrick.GetComponent<MeshRenderer>().enabled = false;

            Vector3 newBrickPos = targetedBrick.transform.position;

            //Vector3 targetBrickPos = mouseTargetedBrick.transform.position;
            ghostBrick.transform.SetPositionAndRotation(newBrickPos, targetedBrick.transform.rotation);

            gridUtility.SnapObjectToGrid(ghostBrick, movableGrid, true);

            ghostBrick.transform.parent = GameObject.Find(OBJECT_FOLDER_NAME).transform;

            SetObjectAndChildrenColliderEnabled(ghostBrick, false);

   
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
        

        if(targetObject.transform.CompareTag(SOCKET_TAG_MALE) || targetObject.transform.CompareTag(SOCKET_TAG_FEMALE))
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
        

            Held brick is continuing to "be held" and shoot raycasts when it should not.



            Blackbox Raycasts come from wrong place. Fix.

            Because Blackbox has multiple Male sockets, the grid origin can alter placement in unsusal ways. fix.



















    */





    

    

    
}
