using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilsForTests : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public GameObject SetupCameraForScene()
    {

            GameObject tempSceneCamera = new GameObject();
            
            tempSceneCamera.AddComponent<AudioListener>();
            tempSceneCamera.AddComponent<Camera>();
            tempSceneCamera.AddComponent<CameraController>();

            tempSceneCamera.GetComponent<Camera>().tag = "MainCamera";
            tempSceneCamera.GetComponent<Camera>().name = "Temp Camera";

            return tempSceneCamera;
            

    }


    public GameObject CreateTestCubeForScene(bool includeChildren = true)
    {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.tag = "TestCube";

            if(includeChildren)
            {
                GameObject maleSocket = new GameObject();
                maleSocket.tag = "Male";
                maleSocket.AddComponent<BoxCollider>();
                maleSocket.transform.SetParent(cube.transform);

                GameObject femaleSocket = new GameObject();
                femaleSocket.tag = "Female";
                femaleSocket.AddComponent<BoxCollider>();
                femaleSocket.transform.SetParent(cube.transform);
            }

            

            return cube;

    }

    public void DestroyAllObjects(){


        foreach (GameObject testCube in GameObject.FindGameObjectsWithTag("TestCube"))
        {
            GameObject.Destroy(testCube);

        }
        foreach (GameObject testCube in GameObject.FindGameObjectsWithTag("Female"))
        {
            GameObject.Destroy(testCube);

        }
        foreach (GameObject testCube in GameObject.FindGameObjectsWithTag("Male"))
        {
            GameObject.Destroy(testCube);

        }
        foreach (GameObject testCamera in GameObject.FindGameObjectsWithTag("MainCamera"))
        {
            GameObject.Destroy(testCamera);

        }
        
    

    }

    public void CleanUpScene(GameObject tempSceneCamera)
    {

        GameObject.Destroy(tempSceneCamera);

        DestroyAllObjects();


    }

    public Vector3 GetCenterOfScreen(GameObject tempSceneCamera)
    {

        float mouseX = tempSceneCamera.GetComponent<Camera>().pixelWidth / 2;
        float mouseY = tempSceneCamera.GetComponent<Camera>().pixelHeight / 2;


        Vector3 screenCenter = new Vector3(mouseX, mouseY, 0);

        return screenCenter;

    }

    public WaitForSeconds PauseAllTests(int secondsToWait = 0)
    {

        if (secondsToWait <= 0)
            return null;
        else
            return new WaitForSeconds(secondsToWait);

    }



}
