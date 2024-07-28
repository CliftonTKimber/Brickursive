using System;
using System.Collections;
using System.Collections.Generic;
//using System.Numerics;

//using System.Numerics;

using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update


    private float mouseSensitivity = 10f;

    public float cameraMoveSpeed = 5f;

    private Camera cam;

    void Start()
    {
        cam = GetMainCamera();

        
    }

    // Update is called once per frame
    void Update()
    {

      ControlCameraWithKeyboard();
    
        
    }

    void FixedUpdate()
    {

        //CreateAndUpdateRayAtCursor();

    }



    void ControlCameraRotationWithMouse(){

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * 100 * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * 100 * Time.deltaTime;

        transform.Rotate(Vector3.up, mouseX);
        transform.Rotate(-Vector3.right, mouseY);

        

    }

    void ControlCameraWithKeyboard(){

        float vAxis = Input.GetAxis("Vertical");
        float hAxis = Input.GetAxis("Horizontal");


        transform.Translate(Vector3.right * hAxis * cameraMoveSpeed * Time.deltaTime);
        transform.Translate(Vector3.forward * vAxis * cameraMoveSpeed * Time.deltaTime);

        if (Input.GetKey("space"))
            transform.Translate(Vector3.up * cameraMoveSpeed * Time.deltaTime, Space.Self);
        
        else if (Input.GetKey("left shift"))
            transform.Translate(-Vector3.up * cameraMoveSpeed * Time.deltaTime, Space.Self);
        


    }




///Raycast stuff
///

public RaycastHit CastPhysicsRay(Vector3 startPos, Vector3 lookDirection)
    {
        Color rayColor = Color.red;
        if(Physics.Raycast(startPos, lookDirection, out RaycastHit hitInfo, 50f))
            {
            Debug.DrawRay(startPos, lookDirection, rayColor);
            //Debug.Log(startPos + " " + lookDirection);
            return hitInfo;    
            
            }
            
        else
            {
            Debug.DrawRay(startPos, lookDirection, rayColor);
            return hitInfo; //empty
           
            }
        
        
        
    }

    public Ray GetRayCastFromCameraTowardsCursor(Vector3 mousePosition){

        if(cam == null)
            cam = GetMainCamera();


        Ray ray = cam.ScreenPointToRay(mousePosition);

        return ray;
       

    }


    public Vector3 GetRaycastHitPosition(Vector3 mousePosition){

        Ray mouseRay = GetRayCastFromCameraTowardsCursor(mousePosition);
        RaycastHit rayHit = CastPhysicsRay(mouseRay.origin, mouseRay.direction);

        return rayHit.point;

    }
    public RaycastHit GetRaycastHit(Vector3 mousePosition){

        Ray mouseRay = GetRayCastFromCameraTowardsCursor(mousePosition);
        RaycastHit rayHit = CastPhysicsRay(mouseRay.origin, mouseRay.direction);


        return rayHit;

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

    public bool IsPositionAvailable(GridUtils gridUtilScript, GameObject targetBrick, RaycastHit rayHit)
    {
        bool boolA = DidRayHitFindViableSurface(rayHit);
        //SHOULD LATER FIND WAYS TO STOP CALLING THIS EXPENSIVE FUNCTION WHEN UNEEDED
        bool boolB = gridUtilScript.IsBrickWithinSurroundingObjects(targetBrick);

        return boolA && !boolB;
    }

    public bool DidRayHitFindViableSurface(RaycastHit rayHit)
    {

        if(rayHit.collider == null)
            return false;

        String colliderTag = rayHit.collider.tag;

        if(colliderTag == "Male")
            return true;
        else if (colliderTag == "Female")
            return true;
        else
            return false;


    }
      


   

}
