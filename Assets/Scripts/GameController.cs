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


    /*
        TODO:

            For two-handed grabs - perhaps access the colliders that XRGrab script uses, and add child colliders to the list?

            Rigidbody LayerMasks needs adjusting -- make all socket colliders triggers?
        

            MOVEMENT:

            #1 Should be able to hold a lego structure from any point.

            #2 Should be able to extract a lego from a structure.

            All around Polish and Bugfix



            STRUCTURES

            Blackbox Raycasts come from wrong place. Fix.

            Because Blackbox has multiple Male sockets, the grid origin can alter placement in unsusal ways. fix.

            Make the Replicator






        ///Other Notes:
        
        Bugs. - Ghost Brick - Disable all scripts that do behavior beyond placement.

        Bugs - Ghost brick flickers down when an axis of rotation increases (doesn't happen when it decreases)

        

        

        Optimization Ideas:

            Raycasts:

                Create a list of rayOrigins that is run thorugh rather than calculating it every time. Only calculate on brick snap. 
                This should also prevent---much of---a few other ghost brick bugs. It shoudl also reduce the total count of rays cast.

                Only cast Rays in the direction that the brick/structure is moving. (Allow for tolerances of generally a direction)

                Cull Rays that would begin too far outside of camera view frustrum. (be careful to include ones that aren't seen, as the brick
                the player is holding may obscure the view. But the ray should still be cast)


            Convert all Brick BoxColliders with Mesh colliders (Plane)















    */





    

    

    
}
