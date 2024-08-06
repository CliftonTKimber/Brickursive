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

    private RaycastUtils raycastUtils = new RaycastUtils();

    private bool isSceneSetup = false;



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

        RaycastHit rayCollision = raycastUtils.GetRaycastHitFromCameraRay(mockMousePos, tempSceneCamera.GetComponent<Camera>());

        
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

        RaycastHit rayCollision = raycastUtils.GetRaycastHitFromCameraRay(mockMousePos,tempSceneCamera.GetComponent<Camera>());

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

        RaycastHit rayCollision = raycastUtils.GetRaycastHitFromCameraRay(mockMousePos,tempSceneCamera.GetComponent<Camera>());

        
        Assert.NotNull(rayCollision);

       

        yield return testUtility.PauseAllTests();

    }

    #endregion

    #region 

    [UnityTest]

    public IEnumerator RayCollisionGetsCollider()
    {
        //Assign
        TrySetupScene();


        GameObject objectToHit = testCube;


        //Act
        tempSceneCamera.transform.position -= tempSceneCamera.transform.forward *2;
        tempSceneCamera.transform.LookAt(objectToHit.transform);

        Collider rayCollider = raycastUtils.GetRaycastHitFromCameraRay(camScript.GetMouseScreenCenter(tempSceneCamera),tempSceneCamera.GetComponent<Camera>()).collider;
        

        //Assert
        Assert.IsInstanceOf(expected: typeof(Collider), actual: rayCollider);


        
        yield return testUtility.PauseAllTests();

    }

    [UnityTest]

    public IEnumerator RayCollisionCanFindMaleCollider()
    {
        //Assign
        TrySetupScene();
        TryTeardown();
        TrySetupScene(false);

        GameObject objectToHit = testCube;
    


        //Act
        tempSceneCamera.transform.position -= tempSceneCamera.transform.forward;
        tempSceneCamera.transform.LookAt(objectToHit.transform);




        testCube.GetComponent<Collider>().tag = "Male";

        Collider rayCollider = raycastUtils.GetRaycastHitFromCameraRay(camScript.GetMouseScreenCenter(tempSceneCamera),tempSceneCamera.GetComponent<Camera>()).collider;
        

        //Assert
        Assert.AreEqual(expected: "Male", actual: rayCollider.tag);


        
        yield return testUtility.PauseAllTests(1);

    }

    [UnityTest]
    public IEnumerator RayCollisionCanFindNotMaleCollider()
    {
        //Assign
        TrySetupScene();
        TryTeardown();
        TrySetupScene(false);

        //Act
        tempSceneCamera.transform.position -= tempSceneCamera.transform.forward;
        tempSceneCamera.transform.LookAt(testCube.transform);

        testCube.tag = "TestCube";


        Collider rayCollider = raycastUtils.GetRaycastHitFromCameraRay(camScript.GetMouseScreenCenter(tempSceneCamera),tempSceneCamera.GetComponent<Camera>()).collider;
        

        //Assert
        Assert.AreNotEqual(expected: "Male", actual: rayCollider.tag);


        
        yield return testUtility.PauseAllTests();

    }

    [UnityTest]

    public IEnumerator RayDoesNotCollide_ReturnColliderAsNull()
    {



        RaycastHit nonHit = raycastUtils.GetRaycastHitFromCameraRay(testUtility.GetCenterOfScreen(tempSceneCamera) * -1f,tempSceneCamera.GetComponent<Camera>());


        Assert.IsNull(nonHit.collider);


        yield return testUtility.PauseAllTests();
    }
   #endregion


    [UnityTest]

    public IEnumerator Test_GetChildSocketsRecursive()
    
    {
        TrySetupScene();

        GameObject newCube = testUtility.CreateTestCubeForScene(true);
        GameObject newCube2 = testUtility.CreateTestCubeForScene(true);
        GameObject newCube3 = testUtility.CreateTestCubeForScene(true);

        newCube3.transform.parent = newCube2.transform;
        newCube2.transform.parent = newCube.transform;
        newCube.transform.parent = testCube.transform;



        List<GameObject> allSockets = raycastUtils.GetChildSocketsRecursive(testCube);


        Assert.Equals(8, allSockets);

        yield return testUtility.PauseAllTests();
    }

    [UnityTest]

    public IEnumerator Test_CastRaycastsFromEachCell()
    
    {
        TrySetupScene();

        GameObject newCube = testUtility.CreateTestCubeForScene(true);
        GameObject newCube2 = testUtility.CreateTestCubeForScene(true);
        GameObject newCube3 = testUtility.CreateTestCubeForScene(true);

        newCube3.transform.parent = newCube2.transform;
        newCube2.transform.parent = newCube.transform;
        newCube.transform.parent = testCube.transform;

        raycastUtils.GetRaycstHitsFromEveryGridUnit(testCube, 1f);




        yield return testUtility.PauseAllTests(2);
    }




    [UnityTest]

    public IEnumerator A_SetupSceneForTests()
    {
        TrySetupScene();
        yield return testUtility.PauseAllTests(1);
    }

   

    #region Testing GetRaycastHitsFromChildrenBasedOnTags

    [UnityTest]

    public IEnumerator GetRaycastHitsFromChildrenBasedOnTags_IsNotEmpty()
    {
        TrySetupScene();

        var possibleList = raycastUtils.GetRaycastHitsFromChildrenBasedOnTags(testCube);

        Assert.IsNotEmpty(possibleList);
        

        yield return testUtility.PauseAllTests();
    }

    [UnityTest]
    public IEnumerator GetRaycastHitsFromChildrenBasedOnTags_CanGetCollisionFromAbove()
    {

        TrySetupScene();

        

        GameObject newCube = testUtility.CreateTestCubeForScene();
        newCube.transform.position += new Vector3(0,2,0);

        yield return testUtility.PauseAllTests(1);


        List<RaycastHit> hitList = raycastUtils.GetRaycastHitsFromChildrenBasedOnTags(testCube);

        int upHits = 0;

        for(int i = 0; i < hitList.Count; i++)
        {
            if(hitList[i].collider != null)
            {
                upHits++;
            }

        }
        
        Assert.Greater(upHits, 0);

        yield return testUtility.PauseAllTests();
    }

    [UnityTest]
    public IEnumerator GetRaycastHitsFromChildrenBasedOnTags_CanGetCollisionFromBelow()
    {

        TrySetupScene();

        

        GameObject newCube = testUtility.CreateTestCubeForScene();
        newCube.transform.position += new Vector3(0,-2,0);

        yield return testUtility.PauseAllTests(1);


        List<RaycastHit> hitList = raycastUtils.GetRaycastHitsFromChildrenBasedOnTags(testCube);

        int hits = 0;

        for(int i = 0; i < hitList.Count; i++)
        {
            if(hitList[i].collider != null)
            {
                hits++;
            }

        }
        
        Assert.Greater(hits, 0);

        yield return testUtility.PauseAllTests();
    }

    [UnityTest]
    public IEnumerator GetRaycastHitsFromChildrenBasedOnTags_CanRetrieveTags()
    {

        TrySetupScene();

        

        GameObject newCube = testUtility.CreateTestCubeForScene();
        newCube.transform.position += new Vector3(0,-2,0);

        yield return testUtility.PauseAllTests(1);


        List<RaycastHit> hitList = raycastUtils.GetRaycastHitsFromChildrenBasedOnTags(testCube);

        string hitTag = "";

        for(int i = 0; i < hitList.Count; i++)
        {
            if(hitList[i].collider != null)
            {
                hitTag = hitList[i].collider.tag;
                break;
            }

        }
        
        Assert.IsNotEmpty(hitTag);

        yield return testUtility.PauseAllTests();
    }


    #endregion
   
        
    #region Methods That Must Be In this File

    private void TrySetupScene(bool includeChildren = true)
    {
        if(!isSceneSetup){
        testUtility  = new UtilsForTests();

        tempSceneCamera = testUtility.SetupCameraForScene();
        testCube = testUtility.CreateTestCubeForScene(includeChildren);
        camScript = tempSceneCamera.GetComponent<CameraController>();

        isSceneSetup = true;

        testUtility.PauseAllTests(1);
        }


    }

    private void TryTeardown()
    {

        if(isSceneSetup){
            testUtility.DestroyAllObjects();
            testUtility.PauseAllTests(1);

            isSceneSetup = false;
        }

    }


    #endregion
}
