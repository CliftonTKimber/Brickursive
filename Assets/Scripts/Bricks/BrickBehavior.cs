using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using static GameConfig;


public class BrickBehavior : MonoBehaviour
{


    [NonSerialized]
    public GameObject gameController;

    [NonSerialized]
    public Transform newParent;

    [NonSerialized]
    public Transform highestParent;



    [NonSerialized]
    public Vector3 trueScale;

    public bool canBeMovedByMachines = true;

    public Vector3 extraRotation;


    private HoverEnterEventArgs hoverData = null;

    [NonSerialized]
    public List<GameObject> belts = new();

    private BrickLibrary brickLibrary;


    void Start()
    {
        extraRotation = new();
        gameController = GameObject.Find("Game Controller");
        brickLibrary = GameObject.Find("Brick Library").GetComponent<BrickLibrary>();

        highestParent = transform;

        SetTrueScale();

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {

        
        HandelExtraInput();

        

    }



  
    public void TranslateBrick(GameObject sourceObject, Vector3 translation)
    {
        if(belts.Last<GameObject>() != sourceObject)
        {
            return;
        }


        transform.Translate(translation);
        

    }



    private void HandelExtraInput()
    {


        if(hoverData == null)
        {
            return;
        }


        NearFarInteractor hoverInteractor = hoverData.interactorObject.transform.GetComponent<NearFarInteractor>();
        float activateValue = hoverInteractor.activateInput.ReadValue();

        if(activateValue <= 0)
        {
            return;
        }



        BreakAwayBrick(hoverData);



    }


    public void CallSnappingMethods(SelectEnterEventArgs eventData)
    {
        //NOTE: Cannot grab by child bricks yet. On OnlyPluckable Interaction Mask.
        if(gameController == null)
        {
            Start();
        }



        Transform nearFarInteractor = eventData.interactorObject.transform;
        GameObject chosenObject = gameObject;
        
        GameObject usedController = nearFarInteractor.parent.gameObject;

        BrickManager brickManager = gameController.GetComponent<GameController>().brickManager;

        brickManager.BeginSnapping(chosenObject, usedController);

        if(GetComponent<BlackboxBehavior>() != null)
                GetComponent<BlackboxBehavior>().powerLevel = 0;


    }



    public void EndSnappingMethods(SelectExitEventArgs eventData)
    {
        if(gameController == null)
        {
            Start();
        }


        Transform nearFarInteractor = eventData.interactorObject.transform;
        GameObject chosenObject = gameObject;

        GameObject usedController = nearFarInteractor.parent.gameObject;

        BrickManager brickManager = gameController.GetComponent<GameController>().brickManager;


        brickManager.EndSnapping(usedController);


        GetComponent<Rigidbody>().useGravity = false;


        if(newParent == null)
        {
            transform.parent = GameObject.Find(OBJECT_FOLDER_NAME).transform;
        }
        else
        {
            transform.parent = newParent;
        }

    }

     public void BreakAwayBrick(HoverEnterEventArgs eventData)
    {


        Transform nfInteractorTransform = eventData.interactorObject.transform;
        List<Collider> colliders = eventData.interactableObject.colliders;

        Collider chosenCollider = GetClosestCollider(colliders, nfInteractorTransform);
        if(chosenCollider == null)
        {
            return;
        }

        GameObject chosenObject = chosenCollider.gameObject;

        /*if(chosenObject.transform.parent == null)
        {
            return;
        }*/

        if(chosenObject.CompareTag(SOCKET_TAG_FEMALE) || chosenObject.CompareTag(SOCKET_TAG_MALE))
        {
            chosenObject = chosenObject.transform.parent.gameObject;
        }

        /*if(!chosenObject.transform.parent.CompareTag(BASE_BRICK_TAG))
        {
            return;
        }*/

        if(!chosenObject.transform.parent.CompareTag(BASE_BRICK_TAG))
        {
            for(int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if(child.GetComponent<XRGrabInteractable>() == null )
                {
                    continue;
                }

                BreakAwayBrickFromBase(child.gameObject, chosenObject);

            }

        }
        else
        {
            BreakAwayBrickFromBase(chosenObject, gameObject);
        }

        
        
        
    }

    private void BreakAwayBrickFromBase(GameObject chosenObject, GameObject originalObject)
    {
        //For Juice
        Vector3 awayVector = (chosenObject.transform.position - chosenObject.transform.parent.position).normalized;
        awayVector = Vector3.Scale(awayVector, chosenObject.transform.up);
        ///

        chosenObject.GetComponent<XRGrabInteractable>().enabled = true;

        chosenObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        chosenObject.GetComponent<Rigidbody>().isKinematic = false;

        XRBaseInteractable chosenBaseInteractable = chosenObject.GetComponent<XRBaseInteractable>();
        XRBaseInteractable originalBaseInteractable = originalObject.GetComponent<XRBaseInteractable>();

        //Reregisters colliders to correct objects
        StartCoroutine(GameController.RemoveCollidersAndRegisterInteractable(originalBaseInteractable, chosenBaseInteractable) );


        /// JUICE
        chosenObject.GetComponent<Rigidbody>().AddForce(awayVector * 2f, ForceMode.Impulse);

    }

    public void ReplaceBrickOnBase(DeactivateEventArgs eventData)
    {

        /*Transform nfInteractorTransform = eventData.interactorObject.transform;

        List<Collider> colliders = eventData.interactableObject.colliders;

        GameObject chosenObject = GetClosestCollider(colliders, nfInteractorTransform).gameObject;
        if(chosenObject.CompareTag(SOCKET_TAG_FEMALE) || chosenObject.CompareTag(SOCKET_TAG_MALE))
        {
            chosenObject = chosenObject.transform.parent.gameObject;
        }*/

    }

    public void SetHoverData(HoverEnterEventArgs eventData)
    {
        hoverData = eventData;
    }

    public void NullifyHoverData(HoverExitEventArgs eventData)
    {
        hoverData = null;
    }





    private Collider GetClosestCollider(List<Collider> colliders, Transform nearFarInteractor)
    {
        if(colliders.Count <= 0)
        {
            Debug.Log("There were no colliders in this list!");
            return null;
        }


        Collider closestCollider = colliders[0];
        float colliderDistance = (closestCollider.transform.position - nearFarInteractor.position).magnitude;
        for(int i = 1; i < colliders.Count; i++)
        {
            Collider nextCollider = colliders[i];
            float nextColliderDistance = (nextCollider.transform.position - nearFarInteractor.position).magnitude;

            if(nextColliderDistance < colliderDistance)
            {
                colliderDistance = nextColliderDistance;
                closestCollider = nextCollider;
            }
        }

        return closestCollider;

    }
    
    public void ToggleRigidbodyIsKinematic()
    {
        GetComponent<Rigidbody>().isKinematic = !GetComponent<Rigidbody>().isKinematic;

    }


    public void InvokeBrickMethod(string brickMethodName, float time)
    {
        Invoke(brickMethodName, time);
    }


    


    
    void SetTrueScale()
    {

        Mesh objectMesh = GetComponent<MeshFilter>().mesh;


        trueScale = new( objectMesh.bounds.size.x * transform.lossyScale.x,
                         objectMesh.bounds.size.y * transform.lossyScale.y, 
                         objectMesh.bounds.size.z * transform.lossyScale.z);


        /*if(name == "3x3x3Blackbox")
        {
            trueScale.x -= STUD_HEIGHT * 2;
            trueScale.y -= STUD_HEIGHT * 2;
            trueScale.z -= STUD_HEIGHT * 2;

        }*/
    }

}
