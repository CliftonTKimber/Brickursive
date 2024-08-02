using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

//using System.Numerics;

//using System.Numerics;

using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update

    public float cameraMoveSpeed = 5f;

    public float speedH = 2.0f;
    public float speedV = 2.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    private Camera cam;

    private GameObject grabPoint;


    void Start()
    {
        cam = GetMainCamera();
        grabPoint = CreateGrabPoint();

        



        

        
    }

    // Update is called once per frame
    void Update()
    {

      
    
        
    }

    void FixedUpdate()
    {
        ControlCameraWithKeyboard();
        ControlCameraRotationWithMouse();
        //CreateAndUpdateRayAtCursor();

    }



    private void ControlCameraRotationWithMouse(){

        yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);

    }

    private void ControlCameraWithKeyboard(){

        float vAxis = Input.GetAxis("Vertical");
        float hAxis = Input.GetAxis("Horizontal");


        transform.Translate(Vector3.right * hAxis * cameraMoveSpeed * Time.deltaTime);
        transform.Translate(Vector3.forward * vAxis * cameraMoveSpeed * Time.deltaTime);

        if (Input.GetKey("space"))
            transform.Translate(Vector3.up * cameraMoveSpeed * Time.deltaTime, Space.Self);
        
        else if (Input.GetKey("left shift"))
            transform.Translate(-Vector3.up * cameraMoveSpeed * Time.deltaTime, Space.Self);
        


    }


    public Camera GetMainCamera()
    {
        if(Camera.main != null)
        {
            return Camera.main;
        }
        else
        {
            throw new NullReferenceException("Main Camera is missing. Did you forget to tag?");
        }

    }

 
    public Vector3 GetMouseScreenCenter(GameObject sceneCamera)
    {
        float mouseX = sceneCamera.GetComponent<Camera>().pixelWidth  / 2;
        float mouseY = sceneCamera.GetComponent<Camera>().pixelHeight / 2;

        Vector3 mouseScreenCenter = new Vector3(mouseX, mouseY, 0);

        return mouseScreenCenter;


    }

    public Vector3 GetMousePositionMappedToScreen()
    {
        Camera mainCam = GetMainCamera();
        Vector3 screenCenter = GetMouseScreenCenter(GetMainCamera().gameObject);

        float xScalar = screenCenter.x;
        float yScalar = screenCenter.y;


        Vector3 mappedPosX = new Vector3((Input.mousePosition.x - screenCenter.x) / xScalar, 0 ,0);

        Vector3 mappedPosY = new Vector3(0, (Input.mousePosition.y - screenCenter.y)/yScalar, 0);


        Vector3 normalizedPos = mappedPosX + mappedPosY;
        normalizedPos.z = 0f;

        return  normalizedPos;

    }

    private GameObject CreateGrabPoint()
    {


        GameObject newObject = new();
        newObject.name = "Grab Point";
        Vector3 newPos = new(0,0,5f);
        newObject.transform.position = transform.rotation * newPos + transform.position;
        newObject.transform.SetParent(transform);



        return newObject;


    }


    
   

}
