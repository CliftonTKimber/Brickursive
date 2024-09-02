using System;
using System.Collections;
using System.Collections.Generic;
using log4net.DateFormatter;
using UnityEngine;
using static GameConfig;
using static RaycastHitPlus;

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


    public static RaycastHit GetRaycastHitFromPhysicsRaycast(Vector3 startPos, Vector3 lookDirection, float rayLength = 5f, bool doDraw = false, int rayColorInt = 0)
    {
        Color rayColor = ChooseColor(rayColorInt);

        if (Physics.Raycast(startPos, lookDirection, out RaycastHit hitInfo, rayLength))
        {
            if (doDraw)
            {
                Debug.DrawRay(startPos, lookDirection, rayColor);
            }

            return hitInfo;
        }


        else
        {
            if (doDraw)
            {
                rayColor = Color.red;
                Debug.DrawRay(startPos, lookDirection, rayColor);
            }
            return hitInfo; //empty   
        }
    }

    private static Color ChooseColor(int rayColorInt)
    {
        Color rayColor;
        switch (rayColorInt)
        {
            case 0:
                rayColor = Color.white;
                break;
            case 1:
                rayColor = Color.magenta;
                break;
            case 2:
                rayColor = Color.cyan;
                break;
            case 3:
                rayColor = Color.yellow;
                break;

            default:
                rayColor = Color.white;
                break;

        };
        return rayColor;
    }



    public List<RaycastHitPlus> GetRaycstHitsFromEveryGridUnit(GameObject targetObject)
    {
        List<RaycastHitPlus> hitList = new();

        List<GameObject> allSockets = GetChildSocketsRecursive(targetObject);
        List<RaycastHitPlus> cellHits = new();
    
        for(int i = 0; i < allSockets.Count; i++)
        {
            GameObject brickSocket = allSockets[i];
            cellHits = GetRaycastHitFromEachCellOfChild(brickSocket);



            for(int j = 0; j < cellHits.Count; j++)
            {
                //Debug.Log(IsRayHitOppositeSocket(brickSocket, cellHits[j].raycastHit));
                if (cellHits[j].raycastHit.collider.transform.parent != targetObject.transform.parent && IsRayHitOppositeSocket(brickSocket, cellHits[j].raycastHit) )
                {
                    hitList.Add(cellHits[j]); 
                }  
            }     
        }
        
        return hitList;
    }


    public List<GameObject> GetChildSocketsRecursive(GameObject targetObject)
    {
        List<GameObject> allSockets = new();

        for (int i = 0; i < targetObject.transform.childCount; i++)
        {
            GameObject child = targetObject.transform.GetChild(i).gameObject;

            if(!child.activeSelf)
            {
                continue;
            }

            if (child.CompareTag(SOCKET_TAG_MALE) || child.CompareTag(SOCKET_TAG_FEMALE))
            {
                allSockets.Add(child);
            }

            else if (child.CompareTag(BASE_BRICK_TAG))
            {
                Debug.Log("Getting child");
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

    public List<RaycastHitPlus> GetRaycastHitFromEachCellOfChild(GameObject targetObject)
    {

        Vector3 []vectorArray = GetLookDirectionAndFirstCellOffset(targetObject);

        Vector3 lookDir = vectorArray[0];
        Vector3 firstCellOffset = vectorArray[1];

        List<RaycastHitPlus> allRayHits = CycleThroughBrickCellsAndReturnHits(targetObject, lookDir, firstCellOffset);

        return allRayHits;
    }


    public List<RaycastHitPlus> CycleThroughBrickCellsAndReturnHits(GameObject targetSocket, Vector3 lookDirection, Vector3 firstCellOffset)
    {
        List<RaycastHitPlus> allRayHits = new();
        
        GameObject highestParent = BrickManager.IfChildReturnUpperMostParentBesidesRoot(targetSocket); 

        Vector3 gridCount = GridUtils.ScaleToGridUnits(GridUtils.ObjectMeshSizeToLossyScale(targetSocket));

        for(int i = 0; i < gridCount.x; i++)
        {
            for(int j = 0; j < gridCount.z; j++)
            {
                
                Vector3 unitOffset = Vector3.Scale(new Vector3(i,0,j), BASE_CELL_SIZE); 
                unitOffset = targetSocket.transform.rotation * (unitOffset + firstCellOffset);

                Vector3 newPos = targetSocket.transform.position + unitOffset;

                RaycastHit rayHit = GetRaycastHitFromPhysicsRaycast(newPos, lookDirection, RAY_LENGTH_FOR_GHOST_SNAPPING, true);

                Debug.DrawRay(rayHit.point, rayHit.normal);

                if(rayHit.collider == null)
                {
                    continue;
                }

                GameObject highestHitParent = BrickManager.IfChildReturnUpperMostParentBesidesRoot(rayHit.collider.gameObject).gameObject;

                if(highestHitParent == highestParent)
                {
                    Debug.Log("Raycast is hitting " + highestHitParent.name + ", which the socket is a child of. Are" + 
                    " your BoxColliders overlapping?");
                    continue;
                }

                if(!rayHit.collider.CompareTag(SOCKET_TAG_MALE) && !rayHit.collider.CompareTag(SOCKET_TAG_FEMALE))
                {
                    Debug.Log("Raycast hit a non-socket!");
                    continue;
                }

                RaycastHitPlus hitPlus = new();
                //Vector3 gridPos = GridUtils.GetGridPositionLocalToSocket(targetSocket, newPos);

                hitPlus.raycastHit = rayHit;
                hitPlus.rayOrigin = newPos;
                hitPlus.originSocket = targetSocket;

                allRayHits.Add(hitPlus);  
            }
        }


        return allRayHits;
    }
 

    public static Vector3[] GetLookDirectionAndFirstCellOffset(GameObject targetObject)
    {

        Vector3 lookDir = targetObject.transform.up;
        //GameObject theBrick = targetObject.transform.parent.gameObject;
        Vector3 cornerOffset = GridUtils.GetBottomOfClosestLeftCornerOfObject(targetObject);
        cornerOffset.y = STUD_HEIGHT;


        if(targetObject.CompareTag(SOCKET_TAG_FEMALE))
        {
            lookDir = -lookDir;
            cornerOffset.y = 0;
            cornerOffset.y -= 0.1f;

        }

        cornerOffset.y += 0.05f; //So the ray orgin begins outside of a collider.
        
        Vector3 centerCellOffset = Vector3.Scale(BASE_CELL_SIZE, new Vector3(0.5f, 0f, 0.5f));
        Vector3 firstCellOffset = centerCellOffset + cornerOffset;

        Vector3 []vectorArray = {lookDir, firstCellOffset};


        return vectorArray;


    }
   
   
    public bool IsRayHitOppositeSocket(GameObject originObject, RaycastHit raycastHit)
    {
        bool isOpposite = false;

        if(raycastHit.collider == null){
            return isOpposite;
        }

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
