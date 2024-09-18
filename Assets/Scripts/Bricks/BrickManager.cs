using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameConfig;

using UnityEngine.EventSystems;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

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

    private SoundController soundController;

    private bool canPickup = true;
    void Start()
    {
        soundController = GameObject.Find("Sound Controller").GetComponent<SoundController>();

        brick = availableBricks[0];
        movableGrid = CreateMovableGrid();
        
    }


    void /*Fixed*/Update()
    {


        MoveBricksIfControllersGrab();


        AllowControllerSelection(controllers[0]);
        AllowControllerSelection(controllers[1]);

        ChangeInteractorLayerMaskOnTrigger(controllers[0]);
        ChangeInteractorLayerMaskOnTrigger(controllers[1]);

    }



    public void AllowControllerSelection(GameObject xrController)
    {   
        NearFarInteractor nearFarInteractor = xrController.GetComponentInChildren<NearFarInteractor>();

        if(xrController == null || nearFarInteractor == null)
        {
            return;
        }

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

            if(targetBrick.GetComponent<BlackboxBehavior>() != null)
                targetBrick.GetComponent<BlackboxBehavior>().powerLevel = 0;

            DelaySnapping();

        }
        else if(usedController == controllers[1])
        {
            rightTargetedBrick = targetBrick;
            rightGhostBrick = MakeGhostVersionOfCurrentBrick(targetBrick, rightGhostBrick);
            rightController = controllers[1];

            nfInteractor.interactionLayers =  1 << LAYER_MASK_INTERACT;
            targetBrick.GetComponent<XRGrabInteractable>().interactionLayers = 1 << LAYER_MASK_INTERACT;

            if(targetBrick.GetComponent<BlackboxBehavior>() != null)
                targetBrick.GetComponent<BlackboxBehavior>().powerLevel = 0;


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
  
 



    void ChangeInteractorLayerMaskOnTrigger(GameObject xrController)
    {
        if(xrController == null)
        {
            return;
        }


        NearFarInteractor nfInteractor = xrController.GetComponentInChildren<NearFarInteractor>();

        if(nfInteractor == null)
        {
            return;
        }

        UnityEngine.XR.Interaction.Toolkit.Inputs.Readers.XRInputButtonReader activateInput = nfInteractor.activateInput;
        UnityEngine.XR.Interaction.Toolkit.Inputs.Readers.XRInputButtonReader selectInput = nfInteractor.selectInput;

        if(activateInput.ReadValue() > 0)
        {
            //nfInteractor.interactionLayers = 1 << LAYER_MASK_ONLY_PLUCKABLE;
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

        MoveSelectedBrickToControllerIfToggled(leftTargetedBrick, leftGhostBrick, leftController);
        MoveSelectedBrickToControllerIfToggled(rightTargetedBrick, rightGhostBrick, rightController);

    }

    private void MoveSelectedBrickToControllerIfToggled(GameObject targetedBrick, GameObject ghostBrick, GameObject xrController)
    {
        if (targetedBrick == null || xrController == null)
        {
            return;
        }

        Vector3 originalPos = targetedBrick.transform.position;

        ghostBrick.SetActive(false);

        gridUtility.SnapObjectToGrid(targetedBrick, ghostBrick, movableGrid);

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

            if(targetedBrick.GetComponent<BlackboxBehavior>() != null)
                targetedBrick.GetComponent<BlackboxBehavior>().powerLevel = 1;


            soundController.PlayBrickSnap(targetedBrick.transform.position);


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

            //gridUtility.SnapObjectToGrid(ghostBrick, movableGrid);

            ghostBrick.transform.parent = GameObject.Find(OBJECT_FOLDER_NAME).transform;

            SetObjectAndChildrenColliderEnabled(ghostBrick, false);

   
    }

     #region Ghost Brick
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

        ghostBrick.SetActive(false);

        StripObjectAndChildren(ghostBrick);
        SetObjectAndChildrenColliderEnabled(ghostBrick, false);
        SetObjectAndChildrenMaterial(ghostBrick, ghostMaterial);

        return ghostBrick;
    }

    #endregion

    public static void StripObjectAndChildren(GameObject targetObject)
    {
        if (targetObject == null)
        {
            return;
        }

    
        for(int i = 0; i < targetObject.transform.childCount; i++)
        {
            GameObject child = targetObject.transform.GetChild(i).gameObject;

            if(child == null)
            { 
                continue;
            }
            if(!child.CompareTag(BASE_BRICK_TAG))
            {
                Destroy(child);
            }

            if(child.transform.childCount > 0)
            {
                StripObjectAndChildren(child);
            }

            Destroy(child.GetComponent<XRGeneralGrabTransformer>());
            Destroy(child.GetComponent<XRGrabInteractable>());
            Destroy(child.GetComponent<BlackboxBehavior>());
            Destroy(child.GetComponent<BrickBehavior>());
            Destroy(child.GetComponent<Rigidbody>());
            Destroy(child.GetComponent<Collider>());

            child.tag = "Untagged";
            
            
            
            
        }

        Destroy(targetObject.GetComponent<XRGeneralGrabTransformer>());
        Destroy(targetObject.GetComponent<XRGrabInteractable>());
        Destroy(targetObject.GetComponent<BlackboxBehavior>());
        Destroy(targetObject.GetComponent<BrickBehavior>());
        Destroy(targetObject.GetComponent<Rigidbody>());
        Destroy(targetObject.GetComponent<Collider>());

        targetObject.tag = "Untagged";
        
        

       

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
