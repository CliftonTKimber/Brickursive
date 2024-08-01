using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastUtils
{


    private GridUtils gridUtils;
    public void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public RaycastHit GetRaycastHitFromPhysicsRaycast(Vector3 startPos, Vector3 lookDirection, float rayLength = 10f)
        {
        Color rayColor = Color.green;
        if(Physics.Raycast(startPos, lookDirection, out RaycastHit hitInfo, rayLength))
            {
            Vector3 scaledDirection = Vector3.Scale(lookDirection, new Vector3(rayLength,rayLength,rayLength) );
            Debug.DrawRay(startPos, scaledDirection, rayColor);
            return hitInfo;     
            }
  
        else
            {
            Vector3 scaledDirection = Vector3.Scale(lookDirection, new Vector3(rayLength,rayLength,rayLength) );
            Debug.DrawRay(startPos, scaledDirection, Color.red, rayLength);
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

    public RaycastHit GetRaycastHitFromCameraRay(Vector3 mousePosition, Camera cam, float rayLength = 10f){

        Ray mouseRay = GetRayFromCameraTowardsCursor(mousePosition, cam);
        RaycastHit rayHit = GetRaycastHitFromPhysicsRaycast(mouseRay.origin, mouseRay.direction, rayLength);

        return rayHit;

    }


    public List<RaycastHit> GetRaycastHitsFromChildrenBasedOnTags(GameObject targetObject, float rayLength=2f, bool shootFromNearCorner = true)
    {
        List<RaycastHit> hitList = new List<RaycastHit>();
        for (int i = 0; i < targetObject.transform.childCount; i++)
        {
            Transform childTr = targetObject.transform.GetChild(i);

            if(childTr.CompareTag("Male") || childTr.CompareTag("Female"))
            {

                Vector3 lookDirection = childTr.up;
                Vector3 parentScale = targetObject.transform.localScale;

                Vector3 pos = childTr.position;

                if (shootFromNearCorner)
                {
                    Vector3 nearCorner = new(parentScale.x / 2, parentScale.y/2, parentScale.z / 2);
                    Vector3 cellCenter = new(0.78f / 2, 0, 0.78f / 2);
                    pos = childTr.position - nearCorner + cellCenter;
                }


                if(childTr.CompareTag("Female"))
                {
                    pos -= new Vector3(0,parentScale.y,0);
                    lookDirection = Vector3.Scale(lookDirection, new Vector3(-1,-1,-1)); 
                }

                Vector3 rayOrigin = pos + (targetObject.transform.rotation * new Vector3(0, parentScale.y , 0));


                PreventRaycastFromHittingOriginObject(childTr.gameObject);

                RaycastHit rayHit = GetRaycastHitFromPhysicsRaycast(rayOrigin, lookDirection, rayLength);

                hitList.Add(rayHit);  
            }
        }
        return hitList;
    }

    private void ResetBoxColliders(GameObject targetObject)
    {
        BoxCollider boxCollider =  targetObject.GetComponent<BoxCollider>();
        if(boxCollider != null)
        {
            boxCollider.enabled = true;
        }
    }

    public void PreventRaycastFromHittingOriginObject(GameObject targetObject)
    {
        BoxCollider boxCollider =  targetObject.GetComponent<BoxCollider>();
        if(boxCollider != null)
        {
            boxCollider.enabled = false;
        }

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
