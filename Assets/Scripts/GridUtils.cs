using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    public void SnapObjectToGrid(GameObject targetObject, GameObject movableGrid,  bool objectIsHeld,   float raycastLength= 0.25f)
    {
        if (objectIsHeld)
        {
            List<RaycastHit> hitList = raycastUtils.GetRaycastHitsFromChildrenBasedOnTags(targetObject, raycastLength);

            CycleThroughHitsAndPlaceObjectOnMovableGrid(targetObject, movableGrid, hitList);
        }
    }

    private void CycleThroughHitsAndPlaceObjectOnMovableGrid(GameObject targetObject, GameObject movableGrid, List<RaycastHit> hitList)
    {
        for (int i = 0; i < hitList.Count; i++)
        {
            RaycastHit rayHit = hitList[i];
            if (rayHit.collider != null)
            { 
                GameObject hitObject = rayHit.collider.gameObject;

                if (hitObject.CompareTag("Male") || hitObject.CompareTag("Female"))
                {
                    GameObject trueHitObject = hitObject.transform.parent.gameObject;

                    MoveGridToTargetObjectPositionAndOrientation(movableGrid, trueHitObject);
                    PutObjectOntoGrid(targetObject, movableGrid, rayHit, trueHitObject, hitObject.tag);
                }
            }
        }
    }

    private static void MoveGridToTargetObjectPositionAndOrientation(GameObject movableGrid, GameObject targetObject)
    {
        Vector3 worldPos = targetObject.transform.localToWorldMatrix.GetPosition();
        Vector3 rotatedCornerPos = targetObject.transform.rotation * GetTopOfClosestLeftCornerOfObject(targetObject);

        Vector3 gridStartPos = rotatedCornerPos + worldPos;

        movableGrid.transform.SetPositionAndRotation(gridStartPos, targetObject.transform.rotation);
    }

    private void PutObjectOntoGrid(GameObject targetObject, GameObject movableGrid, RaycastHit rayHit, GameObject hitObject, string sideHitTag = "Male")
    {
        Grid grid = movableGrid.GetComponent<Grid>();

        Vector3Int gridCoords = grid.WorldToCell(rayHit.point);

        Vector3 cellCenter = grid.GetCellCenterWorld(gridCoords);

        Vector3 brickOffset = GetTopOfFarthestRightCornerOfObject(targetObject);
        Vector3 rotatedBrickOffset = hitObject.transform.rotation * brickOffset;

        Vector3 rotatedCellOffset = hitObject.transform.rotation * GetCellCenter(baseCellSize);

        if (sideHitTag == "Female")
        {
            Vector3 scaleOffset = new Vector3(0, targetObject.transform.lossyScale.y, 0);
            rotatedBrickOffset -= hitObject.transform.rotation * scaleOffset;
        }

        Vector3 endPos = cellCenter + rotatedBrickOffset - rotatedCellOffset;


        targetObject.transform.SetPositionAndRotation(endPos, hitObject.transform.rotation);
        targetObject.transform.parent = hitObject.transform;

        FreezeObjectSoItRemainsRelativeToParent(targetObject);
        ReenableColliders(targetObject);
    }
   
    private static void FreezeObjectSoItRemainsRelativeToParent(GameObject targetObject)
    {
        targetObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        targetObject.GetComponent<Rigidbody>().excludeLayers = 7; //Bricks
    }
   
    private void ReenableColliders(GameObject targetObject)
    {
        if (targetObject.name != "Ghost Object")
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


}
