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
            return hitInfo;     
            }
  
        else
            {
            Vector3 scaledDirection = Vector3.Scale(lookDirection, new Vector3(rayLength,rayLength,rayLength) );
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
            Transform childTransform = targetObject.transform.GetChild(i);
            AddRaycastHitIfCorrectChildTag(hitList, targetObject, childTransform, rayLength, shootFromNearCorner);
        }
        return hitList;
    }

    private void AddRaycastHitIfCorrectChildTag(List<RaycastHit> hitList, GameObject targetObject, Transform childTransform,  float rayLength, bool shootFromNearCorner)
    {
        if (childTransform.CompareTag("Male") || childTransform.CompareTag("Female"))
        {
            //Male/Female Colliders MUST NOT take up the same space as the parent collider, or it messes up detection when rotating. Give other box colliders
            //a scale of .97 across the board to fix this.

            Vector3 lookDirection = childTransform.up;
            Vector3 parentScale = targetObject.transform.localScale;
            Quaternion partentRotation = targetObject.transform.rotation;

            Vector3 pos = childTransform.position;
            pos = AdjustRaycastOriginIfTrue(shootFromNearCorner, childTransform, parentScale, partentRotation, pos);

            if (childTransform.CompareTag("Female"))
            {
                Vector3 scaleOffset = new(0, parentScale.y, 0);
                pos -= partentRotation * scaleOffset;
                lookDirection = Vector3.Scale(lookDirection, new Vector3(-1, -1, -1));
            }

            Vector3 rayOrigin = pos + (targetObject.transform.rotation * new Vector3(0, parentScale.y, 0));


            PreventRaycastFromHittingOriginObject(childTransform.gameObject);

            RaycastHit rayHit = GetRaycastHitFromPhysicsRaycast(rayOrigin, lookDirection, rayLength);

            hitList.Add(rayHit);
        }
    }

    private static Vector3 AdjustRaycastOriginIfTrue(bool shootFromNearCorner, Transform childTr, Vector3 parentScale, Quaternion partentRotation, Vector3 pos)
    {
        if (shootFromNearCorner)
        {
            Vector3 nearCorner = new(parentScale.x / 2, parentScale.y / 2, parentScale.z / 2);
            nearCorner = partentRotation * nearCorner;

            Vector3 cellCenter = new(0.78f / 2, 0, 0.78f / 2);
            cellCenter = partentRotation * cellCenter;

            pos = childTr.position - nearCorner + cellCenter;
        }

        return pos;
    }

    public void PreventRaycastFromHittingOriginObject(GameObject targetObject)
    {
        BoxCollider boxCollider =  targetObject.GetComponent<BoxCollider>();
        if(boxCollider != null)
        {
            boxCollider.enabled = false;
        }
    }

      
}
