using System;
using System.Collections;
using System.Collections.Generic;
using log4net.DateFormatter;
using UnityEngine;
using static GameConfig;
using static GameController;

public class RaycastUtils
{

    public List<Vector3> rayGridOrigins;
    public List<Vector3> rayGridOrigins_2;

    public List<RaycastHit> hitList;
    public void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public RaycastHit GetRaycastHitFromPhysicsRaycast(Vector3 startPos, Vector3 lookDirection, float rayLength, bool doDraw = true)
    {
        Color rayColor = Color.green;
        if(Physics.Raycast(startPos, lookDirection, out RaycastHit hitInfo, rayLength))
        {
            if(doDraw)
            {
                Debug.DrawRay(startPos, lookDirection, rayColor);
            }

            return hitInfo;     
        }
  
        else
        {
            if(doDraw)
            {
                Debug.DrawRay(startPos, lookDirection, Color.red);
            }
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
        if (childTransform.CompareTag(SOCKET_TAG_MALE) || childTransform.CompareTag(SOCKET_TAG_FEMALE))
        {
            //Male/Female Colliders MUST NOT take up the same space as the parent collider, or it messes up detection when rotating. Give other box colliders
            //a scale of .97 across the board to fix this.

            Vector3 lookDirection = childTransform.up;
            Vector3 parentScale = targetObject.transform.localScale;
            Quaternion partentRotation = targetObject.transform.rotation;

            Vector3 pos = childTransform.position;
            pos = AdjustRaycastOriginIfTrue(shootFromNearCorner, childTransform, parentScale, partentRotation, pos);

            if (childTransform.CompareTag(SOCKET_TAG_FEMALE))
            {
                Vector3 scaleOffset = new(0, parentScale.y, 0);
                pos -= partentRotation * scaleOffset;
                lookDirection = -lookDirection;
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

            Vector3 cellCenter = new(BASE_CELL_SIZE.x / 2, 0, BASE_CELL_SIZE.z / 2);
            cellCenter = partentRotation * cellCenter;

            pos = childTr.position - nearCorner + cellCenter;
        }

        return pos;
    }

    public void PreventRaycastFromHittingOriginObject(GameObject targetObject)
    {
        Collider collider =  targetObject.GetComponent<Collider>();
        if(collider != null)
        {
            collider.enabled = false;
        }
    }

    public void GetRaycstHitsFromEveryGridUnit(GameObject targetObject)
    {
        hitList = new();
        rayGridOrigins = new();


        List<GameObject> allSockets = GetChildSocketsRecursive(targetObject);


        for(int i = 0; i < allSockets.Count; i++)
        {
            GameObject brickSocket = allSockets[i];


            List<RaycastHit> cellHits = GetRaycastHitFromEachCell(brickSocket);
            List<Vector3> hitOrigins = GetRayOriginFromEachCell(brickSocket);

            for(int j = 0; j < cellHits.Count; j++)
            {

                if (cellHits[j].collider.transform.parent != targetObject.transform.parent && IsRayHitOppositeSocket(brickSocket, cellHits[j]) )
                {
                            hitList.Add(cellHits[j]); 
                            rayGridOrigins.Add(hitOrigins[j]); 
                }
                
            }
            
        }

    }


    public List<GameObject> GetChildSocketsRecursive(GameObject targetObject)
    {
        List<GameObject> allSockets = new();

        for (int i = 0; i < targetObject.transform.childCount; i++)
        {
            GameObject child = targetObject.transform.GetChild(i).gameObject;

            if (child.CompareTag(SOCKET_TAG_MALE) || child.CompareTag(SOCKET_TAG_FEMALE))
            {
                allSockets.Add(child);
            }

            else if (child.CompareTag(BASE_BRICK_TAG))
            {
                List<GameObject> grandChildren = GetChildSocketsRecursive(child);
                for(int j = 0; j < grandChildren.Count; j++)
                {
                    GameObject grandChild = grandChildren[j];
                    allSockets.Add(grandChild);
                }
            }
        }

        return allSockets;
    }     

    public List<RaycastHit> GetRaycastHitFromEachCell(GameObject targetObject)
    {
        List<RaycastHit> allRayHits = new();

        Vector3 []vectorArray = GetLookDirectionAndFirstCellOffset(targetObject);

        Vector3 lookDir = vectorArray[0];
        Vector3 firstCellOffset = vectorArray[1];

        allRayHits = CycleThroughBrickCellsAndReturnHits(allRayHits, targetObject, lookDir, firstCellOffset);

        return allRayHits;
    }

    public List<Vector3> GetRayOriginFromEachCell(GameObject targetObject)
    {
        List<Vector3> allOrigins = new();

        Vector3 []vectorArray = GetLookDirectionAndFirstCellOffset(targetObject);

        Vector3 lookDir = vectorArray[0];
        Vector3 firstCellOffset = vectorArray[1];

        allOrigins = CycleThroughBrickCellsAndReturnRayOrigins(targetObject, lookDir, firstCellOffset);

        return allOrigins;
    }

    public List<RaycastHit> CycleThroughBrickCellsAndReturnHits(List<RaycastHit> allRayHits, GameObject targetObject, Vector3 lookDirection, Vector3 firstCellOffset)
    {

        Vector3 gridCount = GridUtils.ObjectScaleToGridUnits(targetObject);
        
        GameObject highestParent = IfChildReturnUpperMostParentBesidesRoot(targetObject); 
        float raycastLength = GetRaycastLengthForBrickType(highestParent);

        for(int i = 0; i < gridCount.x; i++)
        {
            for(int j = 0; j < gridCount.z; j++)
            {
                
                Vector3 unitOffset = Vector3.Scale(new Vector3(i,0,j), BASE_CELL_SIZE); 
                unitOffset = targetObject.transform.rotation * (unitOffset + firstCellOffset);

                Vector3 newPos = targetObject.transform.position + unitOffset;

                RaycastHit rayHit = GetRaycastHitFromPhysicsRaycast(newPos, lookDirection, raycastLength);

                //do not add hit if null or if self
                if(rayHit.collider != null)
                {
                    GameObject highestHitParent = IfChildReturnUpperMostParentBesidesRoot(rayHit.collider.gameObject).gameObject;

                    if(highestHitParent != highestParent)
                    {
                        allRayHits.Add(rayHit);
                    }
                }
            }
        }

        return allRayHits;
    }
    public List<Vector3> CycleThroughBrickCellsAndReturnRayOrigins(GameObject targetObject, Vector3 lookDirection, Vector3 firstCellOffset)
    {

        Vector3 gridCount = GridUtils.ObjectScaleToGridUnits(targetObject);

        GameObject highestParent = IfChildReturnUpperMostParentBesidesRoot(targetObject); 
        float raycastLength = GetRaycastLengthForBrickType(highestParent);



        List<Vector3> originList = new();

        for(int i = 0; i < gridCount.x; i++)
        {
            for(int j = 0; j < gridCount.z; j++)
            {
                
                Vector3 unitOffset = Vector3.Scale(new Vector3(i,0,j), BASE_CELL_SIZE); 
                unitOffset = targetObject.transform.rotation * (unitOffset + firstCellOffset);

                Vector3 newPos = targetObject.transform.position + unitOffset;

                RaycastHit rayHit = GetRaycastHitFromPhysicsRaycast(newPos, lookDirection, raycastLength);

                //do not add hit if null or if self
                if(rayHit.collider != null)
                {

                    GameObject highestHitParent = IfChildReturnUpperMostParentBesidesRoot(rayHit.collider.gameObject).gameObject;

                    if(highestHitParent != highestParent)
                    {
                        originList.Add(GridUtils.GetGridPositionLocalToObject(targetObject, highestParent, newPos) );
                    }


                }
            }
        }

        return originList;
    }

    public Vector3[] GetLookDirectionAndFirstCellOffset(GameObject targetObject)
    {

        Vector3 lookDir = targetObject.transform.up;
        float clearColliderOffset = 0.025f;
        float heightOffset = targetObject.transform.parent.transform.lossyScale.y;

        if(targetObject.CompareTag(SOCKET_TAG_FEMALE))
        {
            lookDir = -lookDir;
            clearColliderOffset *= -1;
            heightOffset = 0;
        }

        Vector3 cornerOffset = GridUtils.GetBottomOfClosestLeftCornerOfObject(targetObject);
        cornerOffset.y += clearColliderOffset + heightOffset;

        Vector3 centerCellOffset = Vector3.Scale(BASE_CELL_SIZE, new Vector3(0.5f, 0f, 0.5f));
        Vector3 firstCellOffset = centerCellOffset + cornerOffset;

        Vector3 []vectorArray = {lookDir, firstCellOffset};

        return vectorArray;


    }
    public bool IsRayHitOppositeSocket(GameObject originObject, RaycastHit raycastHit)
    {
        bool isOpposite = false;
        if(raycastHit.collider != null){
            if(originObject.CompareTag(SOCKET_TAG_MALE))
            {
                if(raycastHit.collider.CompareTag(SOCKET_TAG_FEMALE))
                {
                    isOpposite = true;
                }
            }
            else if (originObject.CompareTag(SOCKET_TAG_FEMALE))
            {
                if(raycastHit.collider.CompareTag(SOCKET_TAG_MALE))
                {
                    isOpposite = true;
                }
            }
        }

        return isOpposite;
    }

    public float GetRaycastLengthForBrickType(GameObject targetObject)
    {

        float raycastLength = RAY_LENGTH_FOR_SNAPPING;


        string targetName = targetObject.name;

        if(targetObject.transform.parent != null && targetObject.transform.parent.name != OBJECT_FOLDER_NAME)
        {
            targetName = targetObject.transform.parent.name;
        }

        if(targetName == GHOST_BRICK_NAME)
        {
            raycastLength = RAY_LENGTH_FOR_GHOST_SNAPPING;
        }

        return raycastLength;


    }
}
