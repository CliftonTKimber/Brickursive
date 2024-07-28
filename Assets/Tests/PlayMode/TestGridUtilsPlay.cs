using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestGridUtilsPlay
{

    private GridUtils gridUtility = new GridUtils();

    private UtilsForTests testUtility = new UtilsForTests();

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
 
        Assert.NotNull(gridUtility.GetFinalGridPosition(testCube, mousePos, vector, camScript));

        


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


        GameObject collider = testUtility.CreateTestCubeForScene(testCube);

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

    }


    [UnityTest]
    public IEnumerator RayCollisionGetsTopOfCubeOfDifferentSizes([ValueSource("testValues")] int x)
    {



        SetupCameraForScene();
        CameraController camScript = tempSceneCamera.GetComponent<CameraController>();

        GameObject objectToBeHit = GameObject.Instantiate( CreateTestCubeForScene());;

        Transform objectTr = objectToBeHit.GetComponent<BoxCollider>().transform;
        Vector3 newScale = new Vector3(x,x,x);
        objectTr.localScale = gridUtility.ReturnVectorAsPositiveAndNotZero(newScale);


        Vector3 objectFace = new Vector3(0, objectTr.position.y + (objectTr.localScale.y / 2), 0 );


        //Reposition camera so that it looks at the object from the top
        float tempTargetX = objectTr.position.x;
        float tempTargetY = objectTr.position.y + objectTr.localScale.y + 5f;
        float tempTargetZ = objectTr.position.z;

        tempSceneCamera.transform.position = new Vector3(tempTargetX, tempTargetY, tempTargetZ);

        //Vector3 lookDir = new Vector3(90,0,0);

        //tempSceneCamera.transform.eulerAngles = lookDir;
        tempSceneCamera.transform.LookAt(objectTr);

        //get center of screen
        float mouseX = tempSceneCamera.GetComponent<Camera>().pixelWidth / 2;
        float mouseY = tempSceneCamera.GetComponent<Camera>().pixelHeight / 2;


        Vector3 mockMousePos = new Vector3(mouseX, mouseY, 0);

        Vector3 collisionFace = gridUtility.GetCollisionFaceFromRaycast(camScript, mockMousePos);

        
        
        GameObject.Destroy(tempSceneCamera);
        GameObject.Destroy(objectToBeHit);

        Assert.AreEqual(expected: objectFace, actual:collisionFace);

        
        yield return new WaitForSeconds(1);//null;

        
    }

    [UnityTest]
    public IEnumerator RayCollisionGetsTopOfCubeOfDifferentPositions([ValueSource("testValues")] int x)
    {



        SetupCameraForScene();
        CameraController camScript = tempSceneCamera.GetComponent<CameraController>();

        GameObject objectToBeHit = GameObject.Instantiate( CreateTestCubeForScene());;

        Transform objectTr = objectToBeHit.GetComponent<BoxCollider>().transform;
        Vector3 newPosition = new Vector3(x,x,x);
        objectTr.position = newPosition;

        Vector3 objectFace = new Vector3(0, objectTr.position.y + (objectTr.localScale.y / 2), 0 );


        //Reposition camera so that it looks at the object from the top
        float tempTargetX = objectTr.position.x;
        float tempTargetY = objectTr.position.y + objectTr.localScale.y + 5f;
        float tempTargetZ = objectTr.position.z;

        tempSceneCamera.transform.position = new Vector3(tempTargetX, tempTargetY, tempTargetZ);

        Vector3 lookDir = new Vector3(90,0,0);

        tempSceneCamera.transform.eulerAngles = lookDir;

        //get center of screen
        float mouseX = tempSceneCamera.GetComponent<Camera>().pixelWidth / 2;
        float mouseY = tempSceneCamera.GetComponent<Camera>().pixelHeight / 2;


        Vector3 mockMousePos = new Vector3(mouseX, mouseY, 0);

        Vector3 collisionFace = gridUtility.GetCollisionFaceFromRaycast(camScript, mockMousePos);

        
        
        GameObject.Destroy(tempSceneCamera);
        GameObject.Destroy(objectToBeHit);

        Assert.AreEqual(expected: objectFace, actual:collisionFace);

        
        yield return null;

        
    }
    [UnityTest]
    public IEnumerator RayCollisionGetsTopOfCube([ValueSource("testValues")] int x)
    {



        SetupCameraForScene();
        CameraController camScript = tempSceneCamera.GetComponent<CameraController>();

        GameObject objectToBeHit = GameObject.Instantiate( CreateTestCubeForScene());;

        Transform objectTr = objectToBeHit.GetComponent<BoxCollider>().transform;


        Vector3 objectFace = new Vector3(0, objectTr.position.y + (objectTr.localScale.y / 2), 0 );


        //Reposition camera so that it looks at the object from the top
        float tempTargetX = objectTr.position.x;
        float tempTargetY = objectTr.position.y + objectTr.localScale.y + 5f;
        float tempTargetZ = objectTr.position.z;

        tempSceneCamera.transform.position = new Vector3(tempTargetX, tempTargetY, tempTargetZ);

        Vector3 lookDir = new Vector3(90,0,0);

        tempSceneCamera.transform.eulerAngles = lookDir;

        //get center of screen
        float mouseX = tempSceneCamera.GetComponent<Camera>().pixelWidth / 2;
        float mouseY = tempSceneCamera.GetComponent<Camera>().pixelHeight / 2;


        Vector3 mockMousePos = new Vector3(mouseX, mouseY, 0);

        Vector3 collisionFace = gridUtility.GetCollisionFaceFromRaycast(camScript, mockMousePos);

        
        
        GameObject.Destroy(tempSceneCamera);
        GameObject.Destroy(objectToBeHit);

        Assert.AreEqual(expected: objectFace, actual:collisionFace);

        
        yield return null;

        
    }


    [UnityTest]

    public IEnumerator RayCollisionGetsBottomOfCube()
    {

        SetupCameraForScene();
        CameraController camScript = tempSceneCamera.GetComponent<CameraController>();

        GameObject objectToBeHit = CreateTestCubeForScene();
        objectToBeHit.AddComponent<BoxCollider>();

        Transform objectTr = objectToBeHit.GetComponent<BoxCollider>().transform;

        Vector3 objectFace = new Vector3(0, objectTr.position.y - (objectTr.localScale.y / 2), 0 );


        //Reposition camera so that it looks at the object from the top
        float tempTargetX = objectTr.position.x;
        float tempTargetY = objectTr.position.y - objectTr.localScale.y - 0.5f;
        float tempTargetZ = objectTr.position.z;

        tempSceneCamera.transform.position = new Vector3(tempTargetX, tempTargetY, tempTargetZ);

        Vector3 lookDir = new Vector3(-90,0,0);

        tempSceneCamera.transform.eulerAngles = lookDir;

        //get center of screen
        float mouseX = tempSceneCamera.GetComponent<Camera>().pixelWidth /2;
        float mouseY = tempSceneCamera.GetComponent<Camera>().pixelHeight /2;


        Vector3 mockMousePos = new Vector3(mouseX, mouseY, 0);

        Vector3 collisionFace = gridUtility.GetCollisionFaceFromRaycast(camScript, mockMousePos);

        

        Assert.AreEqual(expected: objectFace, actual:collisionFace);

        GameObject.Destroy(tempSceneCamera);

        yield return null;
        
    }

   [UnityTest]

    public IEnumerator RayCollisionGetsLeftSideOfCube()
    {

        SetupCameraForScene();
        CameraController camScript = tempSceneCamera.GetComponent<CameraController>();

        GameObject objectToBeHit = CreateTestCubeForScene();
        objectToBeHit.AddComponent<BoxCollider>();

        Transform objectTr = objectToBeHit.GetComponent<BoxCollider>().transform;

        Vector3 objectFace = new Vector3(objectTr.position.x - (objectTr.localScale.x / 2), 0, 0 );


        //Reposition camera so that it looks at the object from the top
        float tempTargetX = objectTr.position.x - objectTr.localScale.x - 0.5f;
        float tempTargetY = objectTr.position.y;
        float tempTargetZ = objectTr.position.z;

        tempSceneCamera.transform.position = new Vector3(tempTargetX, tempTargetY, tempTargetZ);

        Vector3 lookDir = new Vector3(0,90,0);

        tempSceneCamera.transform.eulerAngles = lookDir;

        //get center of screen
        float mouseX = tempSceneCamera.GetComponent<Camera>().pixelWidth /2;
        float mouseY = tempSceneCamera.GetComponent<Camera>().pixelHeight /2;


        Vector3 mockMousePos = new Vector3(mouseX, mouseY, 0);

        Vector3 collisionFace = gridUtility.GetCollisionFaceFromRaycast(camScript, mockMousePos);

        

        Assert.AreEqual(expected: objectFace, actual:collisionFace);

        GameObject.Destroy(tempSceneCamera);

        yield return null;
        
    }

    [UnityTest]

    public IEnumerator RayCollisionGetsRightSideOfCube()
    {

        SetupCameraForScene();
        CameraController camScript = tempSceneCamera.GetComponent<CameraController>();

        GameObject objectToBeHit = CreateTestCubeForScene();
        objectToBeHit.AddComponent<BoxCollider>();

        Transform objectTr = objectToBeHit.GetComponent<BoxCollider>().transform;

        Vector3 objectFace = new Vector3(objectTr.position.x + (objectTr.localScale.x / 2), 0, 0 );


        //Reposition camera so that it looks at the object from the top
        float tempTargetX = objectTr.position.x + objectTr.localScale.x + 0.5f;
        float tempTargetY = objectTr.position.y;
        float tempTargetZ = objectTr.position.z;

        tempSceneCamera.transform.position = new Vector3(tempTargetX, tempTargetY, tempTargetZ);

        Vector3 lookDir = new Vector3(0,-90,0);

        tempSceneCamera.transform.eulerAngles = lookDir;

        //get center of screen
        float mouseX = tempSceneCamera.GetComponent<Camera>().pixelWidth /2;
        float mouseY = tempSceneCamera.GetComponent<Camera>().pixelHeight /2;


        Vector3 mockMousePos = new Vector3(mouseX, mouseY, 0);

        Vector3 collisionFace = gridUtility.GetCollisionFaceFromRaycast(camScript, mockMousePos);

        

        Assert.AreEqual(expected: objectFace, actual:collisionFace);

        GameObject.Destroy(tempSceneCamera);

        yield return null;
        
    }


    [UnityTest]

    public IEnumerator RayCollisionGetsFrontOfCube()
    {
        //CLarification the Cube's front, not the face nearest camera.

        SetupCameraForScene();
        CameraController camScript = tempSceneCamera.GetComponent<CameraController>();

        GameObject objectToBeHit = CreateTestCubeForScene();
        objectToBeHit.AddComponent<BoxCollider>();

        Transform objectTr = objectToBeHit.GetComponent<BoxCollider>().transform;

        Vector3 objectFace = new Vector3(0, 0, objectTr.position.z + (objectTr.localScale.z / 2));


        //Reposition camera so that it looks at the object from the top
        float tempTargetX = objectTr.position.x;
        float tempTargetY = objectTr.position.y;
        float tempTargetZ = objectTr.position.z + objectTr.localScale.z + 0.5f;

        tempSceneCamera.transform.position = new Vector3(tempTargetX, tempTargetY, tempTargetZ);

        Vector3 lookDir = new Vector3(0,180,0);

        tempSceneCamera.transform.eulerAngles = lookDir;

        //get center of screen
        float mouseX = tempSceneCamera.GetComponent<Camera>().pixelWidth /2;
        float mouseY = tempSceneCamera.GetComponent<Camera>().pixelHeight /2;


        Vector3 mockMousePos = new Vector3(mouseX, mouseY, 0);

        Vector3 collisionFace = gridUtility.GetCollisionFaceFromRaycast(camScript, mockMousePos);

        
        Assert.AreEqual(expected: objectFace, actual:collisionFace);

        GameObject.Destroy(tempSceneCamera);

        yield return null;
        
    }

    public IEnumerator RayCollisionGetsBackOfCube()
    {
        //CLarification the Cube's front, not the face nearest camera.

        SetupCameraForScene();
        CameraController camScript = tempSceneCamera.GetComponent<CameraController>();

        GameObject objectToBeHit = CreateTestCubeForScene();
        objectToBeHit.AddComponent<BoxCollider>();

        Transform objectTr = objectToBeHit.GetComponent<BoxCollider>().transform;

        Vector3 objectFace = new Vector3(0, 0, objectTr.position.z - (objectTr.localScale.z / 2) );


        //Reposition camera so that it looks at the object from the top
        float tempTargetX = objectTr.position.x;
        float tempTargetY = objectTr.position.y;
        float tempTargetZ = objectTr.position.z - objectTr.localScale.z - 0.5f;

        tempSceneCamera.transform.position = new Vector3(tempTargetX, tempTargetY, tempTargetZ);

        Vector3 lookDir = new Vector3(0,0,0);

        tempSceneCamera.transform.eulerAngles = lookDir;

        //get center of screen
        float mouseX = tempSceneCamera.GetComponent<Camera>().pixelWidth /2;
        float mouseY = tempSceneCamera.GetComponent<Camera>().pixelHeight /2;


        Vector3 mockMousePos = new Vector3(mouseX, mouseY, 0);

        Vector3 collisionFace = gridUtility.GetCollisionFaceFromRaycast(camScript, mockMousePos);

        
        Assert.AreEqual(expected: objectFace, actual:collisionFace);

        GameObject.Destroy(tempSceneCamera);

        yield return null;
        
    }

    */
   
   
   [UnityTest]

    public IEnumerator A_SetupSceneForTests()
    {

        tempSceneCamera = testUtility.SetupCameraForScene(tempSceneCamera);
        testCube = testUtility.CreateTestCubeForScene(testCube);
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
