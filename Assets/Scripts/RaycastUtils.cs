using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastUtils
{



    public void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public RaycastHit GetRaycastHitFromPhysicsRaycast(Vector3 startPos, Vector3 lookDirection)
        {
        Color rayColor = Color.red;
        if(Physics.Raycast(startPos, lookDirection, out RaycastHit hitInfo, 50f))
            {
            Debug.DrawRay(startPos, lookDirection, rayColor);
            return hitInfo;     
            }
  
        else
            {
            Debug.DrawRay(startPos, lookDirection, rayColor);
            return hitInfo; //empty   
            }
        
        
        
    }

    public Ray GetRayFromCameraTowardsCursor(Vector3 mousePosition, Camera cam){

        if(cam == null)
        {
            throw new NullReferenceException("Camera cam is null");
        }

        Ray ray = cam.ScreenPointToRay(mousePosition);

        return ray;
       

    }

    public RaycastHit GetRaycastHitFromCameraRay(Vector3 mousePosition, Camera cam){

        Ray mouseRay = GetRayFromCameraTowardsCursor(mousePosition, cam);
        RaycastHit rayHit = GetRaycastHitFromPhysicsRaycast(mouseRay.origin, mouseRay.direction);

        return rayHit;

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

        string colliderTag = rayHit.collider.tag;

        if(colliderTag == "Male")
            return true;
        else if (colliderTag == "Female")
            return true;
        else
            return false;


    }
      
}
