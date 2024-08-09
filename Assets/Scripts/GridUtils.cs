using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GameConfig;

public class GridUtils
{
    // Start is called before the first frame update
    public Vector3 baseCellSize;
    private RaycastUtils raycastUtils;

    private GameController gameController;

    public void Start(GameController passGameController)
    {
        baseCellSize = passGameController.baseCellSize;
        gameController = passGameController;

        raycastUtils = new();
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void SnapObjectToGrid(GameObject targetObject, GameObject movableGrid,  bool objectIsHeld,   float raycastLength = 0.25f)
    {
        if (!objectIsHeld)
        {
            return;
        }

        List<RaycastHitPlus> hitList = raycastUtils.GetRaycstHitsFromEveryGridUnit(targetObject);

        if (hitList.Count <= 0)
        {
            return;
        }
   
        RaycastHitPlus chosenSpecialHit = ChooseClosestHitToObject(targetObject, hitList);
        GameObject hitObject = chosenSpecialHit.raycastHit.collider.gameObject;

        MoveGridToTargetObjectPositionAndOrientation(movableGrid, hitObject);
        PutObjectOntoGrid(targetObject, hitObject, movableGrid, chosenSpecialHit);    
    }

    private static RaycastHitPlus ChooseClosestHitToObject(GameObject targetObject, List<RaycastHitPlus> hitList)
    {
        RaycastHitPlus chosenHit = hitList[0];
        Vector3 oldRayOrigin = chosenHit.rayOrigin;

        Vector3 targetPosition = targetObject.transform.position;
        Quaternion targetRotation = targetObject.transform.rotation;

        Vector3 cellCenterOffset = GetCellCenter(BASE_CELL_SIZE);

        for (int i = 0; i < hitList.Count; i++)
        {
            Vector3 oldHitDestination = chosenHit.raycastHit.point;
            Vector3 oldRotatedRayOriginPosition = GetRayOriginInWorldSpaceRelativeToObject(targetObject, oldRayOrigin);

            float oldDistance = Vector3.Distance(oldRotatedRayOriginPosition, oldHitDestination);


            Vector3 newRayOrigin = hitList[i].rayOrigin;
            Vector3 newHitDestination = hitList[i].raycastHit.point;
            Vector3 newRotatedRayOriginPosition = GetRayOriginInWorldSpaceRelativeToObject(targetObject, newRayOrigin);

            float newDistance = Vector3.Distance(newRotatedRayOriginPosition, newHitDestination);

            //closest hit // Closest number to 0*/
            if (Mathf.Abs(oldDistance) > Mathf.Abs(newDistance))
            {
                chosenHit = hitList[i];
                oldRayOrigin = chosenHit.rayOrigin;

            }
        }

        return chosenHit;
    }


    public void MoveGridToTargetObjectPositionAndOrientation(GameObject movableGrid, GameObject targetObject, bool doGetBottom = false)
    {
        GameObject targetBrick = IfSocketReturnParentBrick(targetObject);

        Vector3 worldPos = targetObject.transform.localToWorldMatrix.GetPosition();
        Vector3 cornerPos = GetTopOfClosestLeftCornerOfObject(targetBrick);

        if(doGetBottom)
        {
            cornerPos = GetBottomOfClosestLeftCornerOfObject(targetBrick);
        }


        Vector3 rotatedCornerPos = targetBrick.transform.rotation * cornerPos;
        Vector3 gridStartPos = rotatedCornerPos + worldPos;

        movableGrid.transform.SetPositionAndRotation(gridStartPos, targetBrick.transform.rotation);
    }


    public void PutObjectOntoGrid(GameObject targetObject, GameObject hitSocket, GameObject movableGrid, RaycastHitPlus rayHitPlus)
    {
        GameObject hitBrick = hitSocket.transform.parent.gameObject;

        Quaternion hitRotation = hitBrick.transform.rotation;

        Vector3 endPos = GetFinalGridPositionIncludingRotation(targetObject, hitSocket, movableGrid, rayHitPlus);

     
        targetObject.transform.SetPositionAndRotation(endPos, hitRotation);
        targetObject.transform.parent = hitBrick.transform;

        FreezeObjectSoItRemainsRelativeToParent(targetObject);
        ReenableColliders(targetObject);

    }

    public Vector3 GetFinalGridPositionIncludingRotation(GameObject targetObject, GameObject hitSocket, GameObject movableGrid, RaycastHitPlus rayHitPlus)
    {
        GameObject hitBrick = hitSocket.transform.parent.gameObject;
        Quaternion hitRotation = hitBrick.transform.rotation;

        Grid grid = movableGrid.GetComponent<Grid>();

        Vector3Int gridCoords = grid.WorldToCell(rayHitPlus.raycastHit.point);

        Vector3 cellCenter = grid.GetCellCenterWorld(gridCoords);

        Vector3 brickOffset = GetTopOfFarthestRightCornerOfObject(targetObject);
        Vector3 rotatedBrickOffset = hitRotation * brickOffset;
        Vector3 rotatedCellOffset  = hitRotation * GetCellCenter(baseCellSize);

        Vector3 gridHitOrigin = rayHitPlus.rayOrigin;
        gridHitOrigin = Vector3.Scale(gridHitOrigin, BASE_CELL_SIZE);
        gridHitOrigin = hitRotation * gridHitOrigin;

        if (hitSocket.CompareTag(SOCKET_TAG_FEMALE) )
        {
            GameObject originBrick = rayHitPlus.originSocket.transform.parent.gameObject;
            Vector3 scaleOffset = new Vector3(0, originBrick.transform.lossyScale.y, 0);
            rotatedBrickOffset -= hitRotation * scaleOffset;
        }


        Vector3 finalPos = cellCenter + rotatedBrickOffset - rotatedCellOffset - gridHitOrigin;


        return finalPos;



    }

    public static Vector3 GetRayOriginInWorldSpaceRelativeToObject(GameObject targetObject, Vector3 rayOrigin)
    {
        Vector3 targetPosition = targetObject.transform.position;
        Quaternion targetRotation = targetObject.transform.rotation;
        Vector3 cellCenterOffset = GetCellCenter(BASE_CELL_SIZE);

        Vector3 worldRayOrigin = targetRotation * (Vector3.Scale(rayOrigin, BASE_CELL_SIZE) + cellCenterOffset);
        Vector3 rotatedRayOriginPosition = targetPosition + worldRayOrigin;

        return rotatedRayOriginPosition;
    }

     private static void FreezeObjectSoItRemainsRelativeToParent(GameObject targetObject)
    {
        targetObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        targetObject.GetComponent<Rigidbody>().excludeLayers = BRICK_LAYER;
    }
   
    private void ReenableColliders(GameObject targetObject)
    {
        //if (targetObject.name != GHOST_BRICK_NAME)
        {
            gameController.SetObjectAndChildrenColliderEnabled(targetObject, true);
        }
    }


    
    private static Vector3 GetTopOfClosestLeftCornerOfObject(GameObject targetObject)
    {
        Vector3 vertexPos = new(-1,1,-1); //BL - closest
        Vector3 cornerPosition = GetCubeVertex(targetObject, vertexPos);

        //Debug.DrawLine(targetObject.transform.position, targetObject.transform.position+ cornerPosition, Color.red, 5f);

        return cornerPosition;

    }

     public static Vector3 GetBottomOfClosestLeftCornerOfObject(GameObject targetObject)
    {
        Vector3 vertexPos = new(-1,-1,-1); //BL - closest
        Vector3 cornerPosition = GetCubeVertex(targetObject, vertexPos);

        //Debug.DrawLine(targetObject.transform.position, targetObject.transform.position+ cornerPosition, Color.red, 5f);

        return cornerPosition;

    }
   
    public static Vector3 GetTopOfFarthestRightCornerOfObject(GameObject targetObject){
        Vector3 vertexPos = new(1, 1, 1); //BL - closest
        Vector3 cornerPosition = GetCubeVertex(targetObject, vertexPos);

        return cornerPosition;
    }
    

    public static Vector3 GetCubeVertex(GameObject targetObject, Vector3 targetVertex)
    {
        //Lossy chosen to give World coords
        float widthOffset  = targetObject.transform.lossyScale.x / 2 * targetVertex.x;
        float heightOffset = targetObject.transform.lossyScale.y / 2 * targetVertex.y;
        float lengthOffset = targetObject.transform.lossyScale.z / 2 * targetVertex.z;

        Vector3 vertexPosition = new Vector3(widthOffset, heightOffset, lengthOffset);

        return vertexPosition;
    }

    public static Vector3 GetCellCenter(Vector3 cellSize)
    {
        Vector3 cellCenter = new(cellSize.x / 2,
                                 cellSize.y / 2,
                                 cellSize.z / 2);

        return cellCenter;
    }

    public static Vector3 ObjectScaleToGridUnits(GameObject targetObject)

    {
        Vector3 unitSize = new();
        Vector3 objectScale = targetObject.transform.lossyScale;

        unitSize.x = Mathf.RoundToInt(objectScale.x / BASE_CELL_SIZE.x);
        unitSize.y = Mathf.RoundToInt(objectScale.y / BASE_CELL_SIZE.y);
        unitSize.z = Mathf.RoundToInt(objectScale.z / BASE_CELL_SIZE.z);

        return unitSize;
    }

    public static Vector3 ReturnVectorAsGridPosition(Vector3 worldPosition)
    {
        Vector3 gridPosition = new();

        gridPosition.x = Mathf.RoundToInt(worldPosition.x / BASE_CELL_SIZE.x);
        gridPosition.y = Mathf.RoundToInt(worldPosition.y / BASE_CELL_SIZE.y);
        gridPosition.z = Mathf.RoundToInt(worldPosition.z / BASE_CELL_SIZE.z);

        return gridPosition;
    }

    public static Vector3 GetGridPositionLocalToObject(GameObject targetObject, GameObject parentObject, Vector3 rayOrigin)
    {
        Vector3 localGridPosition = new();

        Vector3 objectCorner = GetBottomOfClosestLeftCornerOfObject(parentObject);
        Vector3 cellOffset = GetCellCenter(BASE_CELL_SIZE);
        cellOffset.y = 0;

        if(targetObject.CompareTag(SOCKET_TAG_MALE))
        {
            cellOffset.y = targetObject.transform.lossyScale.y;
        }

        Vector3 cornerCell = objectCorner + cellOffset;
        Vector3 objectPosition = parentObject.transform.position;

        objectPosition += cornerCell - rayOrigin;

        localGridPosition = ReturnVectorAsGridPosition(-objectPosition);

        return localGridPosition;
    }



    #region 

    public GameObject IfSocketReturnParentBrick(GameObject targetObject)
    {
        GameObject returnObject = targetObject;
        if(targetObject.CompareTag(SOCKET_TAG_MALE) || targetObject.CompareTag(SOCKET_TAG_MALE))
        {
            returnObject = targetObject.transform.parent.gameObject;
        }

        return returnObject;
    }




    #endregion
    

}
