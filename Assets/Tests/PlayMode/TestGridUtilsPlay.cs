using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestGridUtilsPlay
{

    private GridUtils gridUtility;

    private UtilsForTests testUtility;

    private RaycastUtils raycastUtils;

    private GameObject testCube;

    private GameObject tempSceneCamera;

    private CameraController camScript;

    static int[] testValues = new int[] { 0, -1, 1, -100, 100 };
    
   
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        //yield return null;

 
   
   [UnityTest]

    public IEnumerator A_SetupSceneForTests()
    {

        gridUtility = new GridUtils();
        testUtility = new UtilsForTests();
        raycastUtils = new RaycastUtils();

        tempSceneCamera = testUtility.SetupCameraForScene();
        testCube = testUtility.CreateTestCubeForScene();
        camScript = tempSceneCamera.GetComponent<CameraController>();


        yield return testUtility.PauseAllTests();

    }

    [UnityTest]

    public IEnumerator ZZ_TeardownSceneForNextSetOfTests()
    {
        testUtility.DestroyAllObjects();

        yield return testUtility.PauseAllTests(3);
    }
   
   
   


}
