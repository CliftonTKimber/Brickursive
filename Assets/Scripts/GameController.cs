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
        

            MOVEMENT:

            #1 Should be able to hold a lego structure from any point.

            #2 Should be able to extract a lego from a structure.

            All around Polish and Bugfix


            STRUCTURES

            Blackbox Raycasts come from wrong place. Fix.

            Because Blackbox has multiple Male sockets, the grid origin can alter placement in unsusal ways. fix.

            Make the Replicator



















    */





    

    

    
}
