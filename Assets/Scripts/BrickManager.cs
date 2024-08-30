using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameConfig;

using UnityEngine.EventSystems;
using UnityEditor.XR.Interaction.Toolkit;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using Unity.XR.CoreUtils;

public class BrickManager : MonoBehaviour
{

    public List<GameObject> availableBricks;

    public List<GameObject> controllers;
     public GameObject brick;

    private GameObject rightController;

    private GameObject leftController;

    public float spawnDistance = 5f;

    private GameObject leftTargetedBrick;
    public GameObject rightTargetedBrick;

    private int brickSelector = 0;

    public GameObject leftGhostBrick;
    public GameObject rightGhostBrick;
    public Material ghostMaterial;


    public float cameraMoveSpeed = 5f;

    public CameraController cameraScript;

    public GameObject movableGrid;

    public GridUtils gridUtility;

    public RaycastUtils raycastUtils;

    private bool canPickup = true;
    void Start()
    {

        brick = availableBricks[0];
        movableGrid = CreateMovableGrid();
        
    }


    void /*Fixed*/Update()
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



    public void AllowControllerSelection(GameObject xrController)
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

            DelaySnapping();

        }
        else if(usedController == controllers[1])
        {
            rightTargetedBrick = targetBrick;
            rightGhostBrick = MakeGhostVersionOfCurrentBrick(targetBrick, rightGhostBrick);
            rightController = controllers[1];

            nfInteractor.interactionLayers =  1 << LAYER_MASK_INTERACT;
            targetBrick.GetComponent<XRGrabInteractable>().interactionLayers = 1 << LAYER_MASK_INTERACT;

            DelaySnapping();

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
        if(usedController == controllers[1])
        {
            rightTargetedBrick = null;
            GameObject.Destroy(rightGhostBrick);
            rightController = null;


        }



    }

    private void DelaySnapping()
    {
        canPickup = false;
        Invoke("SetCanPickupTrue", PICKUP_GRACE_TIME);

    }

    private void SetCanPickupTrue()
    {
        canPickup = true;

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

    public void SpawnBrickIntoTheAirOnKeyDown()
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

        if(leftTargetedBrick != null )
        {
            BrickBehavior brickScript = leftTargetedBrick.GetComponent<BrickBehavior>();
            if(Input.GetKeyDown("l"))
            {
                brickScript.extraRotation.y++;
            }
            if(Input.GetKeyDown("i"))
            {
                brickScript.extraRotation.x++;
            }
            if(Input.GetKeyDown("k"))
            {
                brickScript.extraRotation.z++;
            }
            if(Input.GetKeyDown("j"))
            {
                brickScript.extraRotation.y--;
            }
        }
        
        
        if(rightTargetedBrick !=null)
        {}

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

        if(activateInput.ReadValue() > 0)
        {
            nfInteractor.interactionLayers = 1 << LAYER_MASK_ONLY_PLUCKABLE;
        }

        if(selectInput.ReadValue() > 0)
        {
            nfInteractor.interactionLayers = 1 <<  LAYER_MASK_INTERACT;
        }
    }


    private void MoveBricksIfControllersGrab()
    {

        if(!canPickup)
        {
            return;
        }

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

            BrickBehavior targetedBrickBehavior = targetedBrick.GetComponent<BrickBehavior>();
            targetedBrickBehavior.InvokeBrickMethod("ToggleRigidbodyIsKinematic", 0.1f);

            //targetedBrick.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            //targetedBrick.GetComponent<Rigidbody>().velocity = Vector3.zero; 
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
            ghostBrick.GetComponent<BrickBehavior>().extraRotation = targetedBrick.GetComponent<BrickBehavior>().extraRotation;

            gridUtility.SnapObjectToGrid(ghostBrick, movableGrid, true);

            ghostBrick.transform.parent = GameObject.Find(OBJECT_FOLDER_NAME).transform;

            SetObjectAndChildrenColliderEnabled(ghostBrick, false);

   
    }
    
    public static GameObject IfChildReturnUpperMostParentBesidesRoot(GameObject targetObject)
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

}
