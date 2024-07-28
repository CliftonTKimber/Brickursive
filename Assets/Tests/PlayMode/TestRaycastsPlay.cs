using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestRaycastsPlay
{

    public GameObject tempSceneCamera;


    public GameObject testCube;
    public CameraController camScript;

    private UtilsForTests testUtility;

    private GridUtils gridUtility = new GridUtils();



    static int[] testValues = new int[] { 0, -1, 1, 100, -100 };

    static string[] testTagStrings = new string[] {"Male", "Female"};

    #region 

    [UnityTest]
    public IEnumerator RayCollisionHitsCubeAtDifferentPositions([ValueSource("testValues")] int x)
    {

        GameObject objectToBeHit = testCube;

        Transform objectTr = objectToBeHit.GetComponent<BoxCollider>().transform;
        Vector3 newPosition = new Vector3(x,x,x);
        objectTr.position = newPosition;



        tempSceneCamera.transform.LookAt(objectTr);

        //get center of screen
        float mouseX = tempSceneCamera.GetComponent<Camera>().pixelWidth / 2;
        float mouseY = tempSceneCamera.GetComponent<Camera>().pixelHeight / 2;


        Vector3 mockMousePos = new Vector3(mouseX, mouseY, 0);

        RaycastHit rayCollision = camScript.GetRaycastHit(mockMousePos);

        
        Assert.NotNull(rayCollision);

        yield return testUtility.PauseAllTests();

        
    }

    [UnityTest]
    public IEnumerator RayCollisionHitsCubeOfDifferentSizes([ValueSource("testValues")] int x)
    {
 
        GameObject objectToBeHit = testCube;

        Transform objectTr = objectToBeHit.GetComponent<BoxCollider>().transform;
        Vector3 newSize = new Vector3(x,x,x);
        objectTr.localScale = newSize;



        tempSceneCamera.transform.LookAt(objectTr);

        //get center of screen
        float mouseX = tempSceneCamera.GetComponent<Camera>().pixelWidth / 2;
        float mouseY = tempSceneCamera.GetComponent<Camera>().pixelHeight / 2;


        Vector3 mockMousePos = new Vector3(mouseX, mouseY, 0);

        RaycastHit rayCollision = camScript.GetRaycastHit(mockMousePos);

        Assert.NotNull(rayCollision);

        yield return testUtility.PauseAllTests();

       
        
    }

      [UnityTest]
    public IEnumerator RayCollisionHitsCubeOfDifferentSizesAndPositions([ValueSource("testValues")] int x)
    {


        GameObject objectToBeHit = testCube;

        Transform objectTr = objectToBeHit.GetComponent<BoxCollider>().transform;
        Vector3 newVector = new Vector3(x,x,x);
        objectTr.localScale = newVector;
        objectTr.position = newVector;



        tempSceneCamera.transform.LookAt(objectTr);

        //get center of screen
        float mouseX = tempSceneCamera.GetComponent<Camera>().pixelWidth / 2;
        float mouseY = tempSceneCamera.GetComponent<Camera>().pixelHeight / 2;


        Vector3 mockMousePos = new Vector3(mouseX, mouseY, 0);

        RaycastHit rayCollision = camScript.GetRaycastHit(mockMousePos);

        
        Assert.NotNull(rayCollision);

       

        yield return testUtility.PauseAllTests();

    }

    #endregion

    #region 

    [UnityTest]

    public IEnumerator RayCollisionGetsCollider()
    {
        //Assign


        GameObject objectToHit = testUtility.CreateTestCubeForScene(testCube);


        //Act
        tempSceneCamera.transform.position -= tempSceneCamera.transform.forward *2;
        tempSceneCamera.transform.LookAt(objectToHit.transform);

        Collider rayCollider = camScript.GetRaycastHit(camScript.GetMouseScreenCenter(tempSceneCamera)).collider;
        

        //Assert
        Assert.IsInstanceOf(expected: typeof(Collider), actual: rayCollider);


        
        yield return testUtility.PauseAllTests();

    }

    [UnityTest]

    public IEnumerator RayCollisionCanFindMaleCollider()
    {
        //Assign


        GameObject objectToHit = testCube;
    


        //Act
        tempSceneCamera.transform.position -= tempSceneCamera.transform.forward;
        tempSceneCamera.transform.LookAt(objectToHit.transform);

        testCube.GetComponent<Collider>().tag = "Male";

        Collider rayCollider = camScript.GetRaycastHit(camScript.GetMouseScreenCenter(tempSceneCamera)).collider;
        

        //Assert
        Assert.AreEqual(expected: "Male", actual: rayCollider.tag);


        
        yield return testUtility.PauseAllTests();

    }

    [UnityTest]
    public IEnumerator RayCollisionCanFindNotMaleCollider()
    {
        //Assign

        //Act
        tempSceneCamera.transform.position -= tempSceneCamera.transform.forward;
        tempSceneCamera.transform.LookAt(testCube.transform);

        testCube.tag = "TestCube";


        Collider rayCollider = camScript.GetRaycastHit(camScript.GetMouseScreenCenter(tempSceneCamera)).collider;
        

        //Assert
        Assert.AreNotEqual(expected: "Male", actual: rayCollider.tag);


        
        yield return testUtility.PauseAllTests();

    }

    [UnityTest]

    public IEnumerator RayDoesNotCollide_ReturnColliderAsNull()
    {



        RaycastHit nonHit = camScript.GetRaycastHit(testUtility.GetCenterOfScreen(tempSceneCamera) * -1f);


        Assert.IsNull(nonHit.collider);


        yield return testUtility.PauseAllTests();
    }
   #endregion

   #region 

    [UnityTest]

    public IEnumerator DidRayHitViableSurfaceIsTrue([ValueSource("testTagStrings")] string tagString)
    {
        //Assign

        testCube.GetComponent<Collider>().tag = tagString;
    
        //Act
        tempSceneCamera.transform.position -= tempSceneCamera.transform.forward;
        tempSceneCamera.transform.LookAt(testCube.transform);


        RaycastHit raycastHit = camScript.GetRaycastHit(camScript.GetMouseScreenCenter(tempSceneCamera));
        bool canPlace = camScript.DidRayHitFindViableSurface(raycastHit);

        //Assert
        Assert.IsTrue(canPlace);

        
        yield return testUtility.PauseAllTests();
    }

    [UnityTest]

    public IEnumerator A_SetupSceneForTests()
    {
        testUtility  = new UtilsForTests();

        tempSceneCamera = testUtility.SetupCameraForScene(tempSceneCamera);
        testCube = testUtility.CreateTestCubeForScene(testCube);
        camScript = tempSceneCamera.GetComponent<CameraController>();

        yield return testUtility.PauseAllTests(2);

    }

   #endregion


   
        
}
