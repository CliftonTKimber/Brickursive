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
using System.Linq;



public class GameController : MonoBehaviour
{
    #region params


    private GridUtils gridUtility;

    private RaycastUtils raycastUtils;

    public BrickManager brickManager;

    public Camera gameCamera;


    

    #endregion


    void Start()
    {
        
        
        
        gridUtility = new GridUtils();
        raycastUtils = new RaycastUtils();

        brickManager.gridUtility = gridUtility;
        brickManager.raycastUtils = raycastUtils;
        brickManager.cameraScript = gameCamera.GetComponent<CameraController>();

        gridUtility.Start(this);
        raycastUtils.Start();


        
    }

    

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {



    }



      #region XR



    public static IEnumerator ReregisterInteractable(XRBaseInteractable inter)
    {
        yield return new WaitForEndOfFrame();
        inter.interactionManager.UnregisterInteractable(inter as IXRInteractable);

        yield return new WaitForEndOfFrame();
        inter.interactionManager.RegisterInteractable(inter as IXRInteractable);

        yield return null;
    }

    public static IEnumerator ReregisterInteractableDelayed(XRBaseInteractable inter, float waitSeconds = 0.25f)
    {
        yield return new WaitForSeconds(waitSeconds);
        inter.interactionManager.UnregisterInteractable(inter as IXRInteractable);

        yield return new WaitForSeconds(waitSeconds);
        inter.interactionManager.RegisterInteractable(inter as IXRInteractable);

        yield return null;
    }

    public static IEnumerator RemoveCollidersAndRegisterInteractable(XRBaseInteractable originalInteractable, XRBaseInteractable chosenInteractable)
    {
        //Unregister
        yield return new WaitForEndOfFrame();
        chosenInteractable.interactionManager.UnregisterInteractable(chosenInteractable as IXRInteractable);

        yield return new WaitForEndOfFrame();
        originalInteractable.interactionManager.UnregisterInteractable(originalInteractable as IXRInteractable);

        //Remove child colliders from Original
        yield return new WaitForEndOfFrame();
        RemoveCollidersFromEachParentInHeirarchy(chosenInteractable);

        //Register
        yield return new WaitForEndOfFrame();
        chosenInteractable.interactionManager.RegisterInteractable(chosenInteractable as IXRInteractable);

        yield return new WaitForEndOfFrame();
        originalInteractable.interactionManager.RegisterInteractable(originalInteractable as IXRInteractable);

        
        //Set new Paretns for brick
        yield return new WaitForEndOfFrame();
        SetParentParams(chosenInteractable.gameObject);


        yield return null;
    }

    public static IEnumerator AddCollidersAndRegisterInteractable(XRBaseInteractable originalInteractable, XRBaseInteractable chosenInteractable)
    {
        //Unregister
        yield return new WaitForEndOfFrame();
        originalInteractable.interactionManager.UnregisterInteractable(originalInteractable as IXRInteractable);

        //Add Colliders
        yield return new WaitForEndOfFrame();
        AddCollidersToEachParentInHeirarchy(chosenInteractable);

        //Register
        yield return new WaitForEndOfFrame();
        originalInteractable.interactionManager.RegisterInteractable(originalInteractable as IXRInteractable);

        yield return null;
    }

    public static IEnumerator UnregisterInteractable(XRBaseInteractable inter)
    {
        yield return new WaitForEndOfFrame();
        inter.interactionManager.UnregisterInteractable(inter as IXRInteractable);

        yield return null;
        
    }

    public static void SetParentParams(GameObject chosenObject)
    {

        Transform gameFolder = GameObject.Find(OBJECT_FOLDER_NAME).transform;

        chosenObject.transform.parent = gameFolder;
        chosenObject.GetComponent<BrickBehavior>().newParent = gameFolder;
        chosenObject.GetComponent<BrickBehavior>().highestParent = chosenObject.transform;

    }


    public static void AddCollidersToEachParentInHeirarchy(XRBaseInteractable startingInteractable)
    {

        XRBaseInteractable[] allParents = startingInteractable.GetComponentsInParent<XRBaseInteractable>();
        List<Collider> startingColliders = startingInteractable.colliders;

        for(int i = 1; i <  allParents.Length; i++)
        {
           XRBaseInteractable nextParent = allParents[i];

           nextParent.colliders.AddRange(startingColliders);
        }
    }

    public static void RemoveCollidersFromEachParentInHeirarchy(XRBaseInteractable startingInteractable)
    {

        XRBaseInteractable[] allParents = startingInteractable.GetComponentsInParent<XRBaseInteractable>();
        List<Collider> startingColliders = startingInteractable.colliders;

        for(int i = 1; i <  allParents.Length; i++)
        {
           XRBaseInteractable nextParent = allParents[i];

            for(int j = 0; j < nextParent.colliders.Count; j++)
            {
                if(nextParent.colliders[j] == startingColliders[0])
                {
                    nextParent.colliders.RemoveRange(j, startingColliders.Count);
                    break;   
                }     
            }         
        }
    }

    #endregion


    /*


        1x1Panels -> 1x1Bricks ->

        TODO:


            1: Get Hammer working
                Joining Pieces
                Squishing Pieces

            2: Unlocking Structures. Need a way to get more into the build.

            3: Inventory


            ///DO THE VERY MOST BASIC VERSIONS FOR NOW

            CODE CLEANUP:

                The grabs, and ungrabs nned to be cleaned up.

                Replace unselecting working around with InteractionManager.CancelInteractableSelection()




            Rigidbody LayerMasks needs adjusting -- make all socket colliders triggers?
        

            MOVEMENT:



            All around Polish and Bugfix










        ///Other Notes:
        
        Bugs - Ghost brick flickers down when an axis of rotation increases (doesn't happen when it decreases)

        

        

        Optimization Ideas:

            Raycasts:

                Create a list of rayOrigins that is run thorugh rather than calculating it every time. Only calculate on brick snap. 
                This should also prevent---much of---a few other ghost brick bugs. It shoudl also reduce the total count of rays cast.

                Only cast Rays in the direction that the brick/structure is moving. (Allow for tolerances of generally a direction)

                Cull Rays that would begin too far outside of camera view frustrum. (be careful to include ones that aren't seen, as the brick
                the player is holding may obscure the view. But the ray should still be cast)


            Belts:

                Turn off adjacent belts, and create a single, long BoxCollider for detection 
                --- should reduce collision checks, and other operations

            Convert all Brick BoxColliders with Mesh colliders (Plane)

            Replace Recursive functions with GetComponentInChildren<>().















    */





    

    

    
}
