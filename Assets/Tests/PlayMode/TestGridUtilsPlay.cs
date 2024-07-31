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
    public IEnumerator BasicTest_GetFinalGridPosition()
    {

        Vector3 mousePos = Vector3.zero;
        Vector3 vector = Vector3.one;

        Camera camera = tempSceneCamera.GetComponent<Camera>();

        RaycastHit rayHit = raycastUtils.GetRaycastHitFromCameraRay(Input.mousePosition, Camera.main);
        Vector3 finalGridPos = gridUtility.GetFinalGridPosition(testCube, rayHit, gridUtility.baseCellSize, raycastUtils);


        Assert.NotNull(finalGridPos);

        


        yield return testUtility.PauseAllTests();

    }

    [UnityTest]

    public IEnumerator CellOffsetHasNoYChange()
    {
        Vector3 cellSize = Vector3.one;

        Vector3 cellOffset = gridUtility.GetBottomMiddleOfCellPosition(cellSize, "Male");


        Assert.AreEqual(expected: 0, actual: cellOffset.y);

        yield return testUtility.PauseAllTests();
    }

    [UnityTest]

    public IEnumerator CellOffsetIsLowered()
    {
        Vector3 cellSize = Vector3.one;

        Vector3 toBeSubtracted = gridUtility.GetBottomMiddleOfCellPosition(cellSize, "Female");


        Assert.Less(-toBeSubtracted.y, 0);

        yield return testUtility.PauseAllTests();
    }


    [UnityTest]

    public IEnumerator IsBrickInsideSurroundings_ReturnFalse ()

    {

        GameObject aimBrick = testCube;
        aimBrick.AddComponent<BrickBehavior>();

        bool aBool = gridUtility.IsBrickWithinSurroundingObjects(aimBrick);

        Assert.IsFalse(aBool);

        yield return testUtility.PauseAllTests(1);
    }

    [UnityTest]

    public IEnumerator IsBrickInsideSurroundingObjects_ReturnTrue()
    {

        GameObject targetBrick = GameObject.CreatePrimitive(PrimitiveType.Cube);
        targetBrick.AddComponent<BrickBehavior>();
        targetBrick.GetComponent<BoxCollider>().isTrigger = true;
        targetBrick.tag = "TestCube";


        GameObject collider = testUtility.CreateTestCubeForScene();

        yield return testUtility.PauseAllTests(1);


        bool aBool = gridUtility.IsBrickWithinSurroundingObjects(targetBrick);

        //NOTE: YOU NEED TO WRITE TESTS FOR ALL THE METHODS YOU MADE! THEN THE ANSWERS WILL BECOME CLEAR!
        //Assert.IsTrue(aBool);

        

        yield return testUtility.PauseAllTests(1);
         
    }

    /*[UnityTest]

    public IEnumerator GetFinalGridPosition_HandlesCorrectOffset()
    {


        Vector3 cellSize = Vector3.one;           


        Vector3 fakeRayHitPosition = Vector3.zero;

        Vector3 gridPosition = gridUtility.GetVectorConvertToGridPosition(fakeRayHitPosition, cellSize);


        Vector3 offsetPosition = gridUtility.GetBottomLeftCornerOfObject(testCube);
        Vector3 cellOffset = gridUtility.GetBottomMiddleOfCellPosition(cellSize);

        Vector3 mockMousePos = tempSceneCamera.transform.forward;


        Vector3 finalPosition = gridUtility.GetFinalGridPosition(testCube, mockMousePos, Vector3.one, camScript) - offsetPosition + cellOffset;

        Assert.AreEqual(expected:gridPosition, actual: finalPosition);

        


        yield return PauseAllTests();

    }

    [UnityTest]

    public IEnumerator GetFinalGridPosition_HandlesCorrectGridPosition()
    {
            SetupCameraForScene();
            CreateTestCubeForScene();

            Vector3 cellSize = Vector3.one;

        

            Vector3 fakeRayHitPosition = Vector3.one;

            Vector3 gridPosition = gridUtility.GetVectorConvertToGridPosition(fakeRayHitPosition, cellSize);




            
            //Vector3 offsetPosition = gridUtility.GetBottomLeftCornerOfObject(testCube);
            
            Vector3 cellOffset = gridUtility.GetBottomMiddleOfCellPosition(cellSize);

            tempSceneCamera.transform.LookAt(testCube.transform);


            Vector3 mockMousePos = GetCenterOfScreen();

            Vector3 finalPosition = gridUtility.GetFinalGridPosition(testCube, mockMousePos, cellSize, camScript) - cellOffset;


            Assert.AreEqual(expected:gridPosition, actual: finalPosition);

            


            yield return PauseAllTests();

    }*/


   
   
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
