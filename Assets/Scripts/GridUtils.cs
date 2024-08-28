using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using static GameConfig;

public class GridUtils
{
    private RaycastUtils raycastUtils;

    private GameController gameController;

    public void Start(GameController passGameController)
    {
        gameController = passGameController;

        raycastUtils = new();
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void SnapObjectToGrid(GameObject targetObject, GameObject movableGrid,  bool objectIsHeld)
    {
        if (!objectIsHeld || targetObject == null)
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


        for (int i = 1; i < hitList.Count; i++)
        {
            Vector3 oldHitDestination = chosenHit.raycastHit.point;
            Vector3 oldRotatedRayOriginPosition = GetRayOriginInWorldSpaceRelativeToObject(targetObject, oldRayOrigin);

            double oldDistance = Vector3.Distance(oldRotatedRayOriginPosition, oldHitDestination);


            Vector3 newRayOrigin = hitList[i].rayOrigin;
            Vector3 newHitDestination = hitList[i].raycastHit.point;
            Vector3 newRotatedRayOriginPosition = GetRayOriginInWorldSpaceRelativeToObject(targetObject, newRayOrigin);


            double newDistance = Vector3.Distance(newRotatedRayOriginPosition, newHitDestination);


            //Tolerances
            oldDistance = Math.Round(oldDistance, 2);
            newDistance = Math.Round(newDistance, 2);


            //closest hit // Closest number to 0*/
            if ( Math.Abs(newDistance) < Math.Abs(oldDistance))
            {
                chosenHit = hitList[i];
                oldRayOrigin = chosenHit.rayOrigin;
            }

        }

        RaycastUtils.GetRaycastHitFromPhysicsRaycast(chosenHit.raycastHit.point, Vector3.up, 5f, true);


        return chosenHit;
    }


    public void MoveGridToTargetObjectPositionAndOrientation(GameObject movableGrid, GameObject targetObject, bool doGetBottom = false)
    {
        GameObject targetBrick = targetObject;//IfSocketReturnParentBrick(targetObject);

        Vector3 worldPos = targetObject.transform.localToWorldMatrix.GetPosition();
        Vector3 cornerPos = GetTopOfClosestLeftCornerOfObject(targetBrick);
        

        Quaternion targetRotation = targetBrick.transform.rotation;

        /*if(targetObject.CompareTag(SOCKET_TAG_FEMALE))
        {
            Vector3 rotationEulers = targetRotation.eulerAngles;

            rotationEulers.y -= 180;
            rotationEulers.z -= 180;

            targetRotation = Quaternion.Euler(rotationEulers);
        }*/

        cornerPos.y = 0f;


        Vector3 rotatedCornerPos = targetRotation * cornerPos;
        Vector3 gridStartPos = rotatedCornerPos + worldPos;

        movableGrid.transform.SetPositionAndRotation(gridStartPos, targetRotation);
    }


    public void PutObjectOntoGrid(GameObject targetObject, GameObject hitSocket, GameObject movableGrid, RaycastHitPlus rayHitPlus)
    {
        GameObject hitBrick = hitSocket.transform.parent.gameObject;

        
        Quaternion hitRotation = GetFinalRotation(targetObject, hitSocket, hitBrick, rayHitPlus);
        Vector3 endPos = GetRotationAcountedGridPosition(targetObject, hitSocket, movableGrid, rayHitPlus, hitRotation);




        targetObject.GetComponent<XRGrabInteractable>().interactionLayers = 1 << LAYER_MASK_ONLY_PLUCKABLE;

        targetObject.transform.SetPositionAndRotation(endPos, hitRotation);
        targetObject.GetComponent<BrickBehavior>().newParent = hitBrick.transform;
        targetObject.transform.SetParent(hitBrick.transform);

        FreezeObjectSoItRemainsRelativeToParent(targetObject);
        ReenableColliders(targetObject);

    }

    public Quaternion GetFinalRotation(GameObject targetObject, GameObject hitSocket, GameObject hitBrick, RaycastHitPlus rayHitPlus)
    {
        GameObject originSocket = rayHitPlus.originSocket;

        Quaternion originRotation = targetObject.transform.rotation;


        Quaternion hitRotation = hitSocket.transform.rotation;  

        
        if (hitSocket.CompareTag(SOCKET_TAG_FEMALE))
        {
            Vector3 hitEuler = hitRotation.eulerAngles;

            hitEuler.y -= 180;
            hitEuler.z -= 180;

            hitEuler.y *= -1;

            hitRotation = Quaternion.Euler(-hitEuler);
        }

       hitRotation *= GetGridRotationOfObject(originRotation, hitRotation);

        

        return hitRotation;

    }

    public Quaternion GetGridRotationOfObject(Quaternion targetBrickRotation, Quaternion hitBrickRotation)
    {
        Quaternion gridRotation = Quaternion.Inverse(hitBrickRotation) * targetBrickRotation;

    
        Vector3 eulerRotation = new( (gridRotation.eulerAngles.x - 0) / 90f,
                                     (gridRotation.eulerAngles.y - 0) / 90f,
                                     (gridRotation.eulerAngles.z - 0) / 90f);

        //To deal wil extreme angles
        if(eulerRotation.x >= 0.5f)   
        {
            eulerRotation.x -= eulerRotation.x;
        } 
        if(eulerRotation.y >= 0.5f)   
        {
            //eulerRotation.y -= eulerRotation.y;
        } 
        if(eulerRotation.z >= 0.5f)   
        {
            eulerRotation.z -= eulerRotation.z;
        } 

        eulerRotation.x = Mathf.Round(eulerRotation.x);
        eulerRotation.y = Mathf.Round(eulerRotation.y);
        eulerRotation.z = Mathf.Round(eulerRotation.z);


        Quaternion finalGridRotation = Quaternion.Euler( new Vector3( eulerRotation.x * 90f,
                                                                      eulerRotation.y * 90f,
                                                                      eulerRotation.z * 90f));

        return finalGridRotation;
    }

    public Vector3 GetRotationAcountedGridPosition(GameObject targetObject, GameObject hitSocket, GameObject movableGrid, RaycastHitPlus rayHitPlus, Quaternion hitRotation)
    {
        GameObject hitBrick = hitSocket.transform.parent.gameObject;
        GameObject originSocket = rayHitPlus.originSocket;

        Grid grid = movableGrid.GetComponent<Grid>();

        Vector3Int gridCoords = grid.WorldToCell(rayHitPlus.raycastHit.point);

        Vector3 cellCenter = grid.GetCellCenterWorld(gridCoords);


        //Offset by which cell of target brick should connect to hit brick

        Vector3 originPos = rayHitPlus.rayOrigin;
        Vector3 objectPos = targetObject.transform.position;


        Vector3 gridHitOrigin =  objectPos - originPos; 
        gridHitOrigin = Quaternion.Inverse(originSocket.transform.rotation) * gridHitOrigin;

        Debug.Log(gridHitOrigin.y);
        
        if(hitSocket.CompareTag(SOCKET_TAG_FEMALE))
        {
            gridHitOrigin.y += 0.5f;
            gridHitOrigin.y *= -1; 

            
        }
        gridHitOrigin.y += 0.1f;

        gridHitOrigin.y *= -1;

        gridHitOrigin = hitRotation * gridHitOrigin;
        

        
        Vector3 finalPos = cellCenter + gridHitOrigin;

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
        Rigidbody targetRb = targetObject.GetComponent<Rigidbody>();
        targetRb.angularVelocity = Vector3.zero;
        targetRb.velocity = Vector3.zero; 
        targetRb.constraints = RigidbodyConstraints.FreezeAll;
        targetRb.excludeLayers = 1 << BRICK_LAYER;

    }
   
    private void ReenableColliders(GameObject targetObject)
    {
        //if (targetObject.name != GHOST_BRICK_NAME)
        {
            //gameController.SetObjectAndChildrenColliderEnabled(targetObject, true);
        }

        if(targetObject.name == GHOST_BRICK_NAME)
        {
            targetObject.GetComponent<MeshRenderer>().enabled = true;

        }
    }


    
    private static Vector3 GetTopOfClosestLeftCornerOfObject(GameObject targetObject)
    {
        Vector3 vertexPos = new(-1,1,-1); //BL - closest
        Vector3 targetScale = new();

        if(targetObject.GetComponent<BrickBehavior>() != null)
        {
            targetScale = targetObject.GetComponent<BrickBehavior>().trueScale;
        }
        else
        {
            targetScale = targetObject.transform.lossyScale;
            //Debug.Log("BrickBehavior is Missing from " + targetObject.name);
        }
        

        Vector3 cornerPosition = GetCubeVertex(targetObject, vertexPos, targetScale);

        return cornerPosition;

    }

     public static Vector3 GetBottomOfClosestLeftCornerOfObject(GameObject targetObject)
    {
        Vector3 vertexPos = new(-1,-1,-1); //BL - closest

        Vector3 targetScale = new();

        if(targetObject.GetComponent<BrickBehavior>() != null)
        {
            targetScale = targetObject.GetComponent<BrickBehavior>().trueScale;
        }
        else
        {
            targetScale = targetObject.transform.lossyScale;
            //Debug.Log("BrickBehavior is Missing from " + targetObject.name);
        }

        Vector3 cornerPosition = GetCubeVertex(targetObject, vertexPos, targetScale);

        //Debug.DrawLine(targetObject.transform.position, targetObject.transform.position+ cornerPosition, Color.red, 5f);

        return cornerPosition;

    }
   
    public static Vector3 GetTopOfFarthestRightCornerOfObject(GameObject targetObject){
        Vector3 vertexPos = new(1, 1, 1); //BL - closest

        Vector3 targetScale = new();

        if(targetObject.GetComponent<BrickBehavior>() != null)
        {
            targetScale = targetObject.GetComponent<BrickBehavior>().trueScale;
        }
        else
        {
            targetScale = targetObject.transform.lossyScale;
            //Debug.Log("BrickBehavior is Missing from " + targetObject.name);
        }

        Vector3 cornerPosition = GetCubeVertex(targetObject, vertexPos, targetScale);

        return cornerPosition;
    }


    public static Vector3 GetCubeVertex(GameObject targetObject, Vector3 targetVertex, Vector3 targetScale)
    {

        if(targetScale == Vector3.zero)
        {
            targetScale = targetObject.transform.lossyScale;

        }

        //Lossy chosen to give World coords
        float widthOffset  = targetScale.x / 2 * targetVertex.x;
        float heightOffset = targetScale.y / 2 * targetVertex.y;
        float lengthOffset = targetScale.z / 2 * targetVertex.z;

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

    public static Vector3 ScaleToGridUnits(Vector3 objectScale)

    {
        Vector3 unitSize = new();

        unitSize.x = Mathf.RoundToInt(objectScale.x / BASE_CELL_SIZE.x);
        unitSize.y = Mathf.RoundToInt(objectScale.y / BASE_CELL_SIZE.y);
        unitSize.z = Mathf.RoundToInt(objectScale.z / BASE_CELL_SIZE.z);


        return unitSize; //Because of cell size, vectors will be weird. a 1x1 brick will be (1,3,1)
    }

    public static Vector3 ObjectMeshSizeToLossyScale(GameObject targetObject)
    {
        Vector3 meshSize = new();

        if (targetObject.GetComponent<MeshFilter>() == null)
        {
            //Debug.Log("MeshFilter was Null");
            meshSize =  Vector3.one;
        }
        else
        {
            meshSize = targetObject.GetComponent<MeshFilter>().mesh.bounds.size;
        }

        Vector3 scaledVector = new( meshSize.x * targetObject.transform.lossyScale.x,
                                    meshSize.y * targetObject.transform.lossyScale.y, 
                                    meshSize.z * targetObject.transform.lossyScale.z);


        return scaledVector;
    }

    public static Vector3 ReturnVectorAsGridPosition(Vector3 worldPosition)
    {
        Vector3 gridPosition = new();

        gridPosition.x = Mathf.RoundToInt(worldPosition.x / BASE_CELL_SIZE.x);
        gridPosition.y = Mathf.RoundToInt(worldPosition.y / BASE_CELL_SIZE.y);
        gridPosition.z = Mathf.RoundToInt(worldPosition.z / BASE_CELL_SIZE.z);

        return gridPosition;
    }

    public static Vector3 GetGridPositionLocalToSocket(GameObject targetObject, Vector3 rayOrigin)
    {
        Vector3 socketCorner = GetBottomOfClosestLeftCornerOfObject(targetObject); 
        Vector3 cellOffset = GetCellCenter(BASE_CELL_SIZE); 


        Quaternion rotationCancel = Quaternion.Inverse(targetObject.transform.rotation);

        
        Vector3 objectPosition =  rotationCancel * targetObject.transform.position;
        Vector3 cornerCell = socketCorner + cellOffset;
        

        rayOrigin = rotationCancel * rayOrigin;
        rayOrigin -= objectPosition + cornerCell;   
        rayOrigin.y = 0;


        Vector3 localGridPosition = ReturnVectorAsGridPosition(rayOrigin);

        return localGridPosition;
    }



    #region 

    public GameObject IfSocketReturnParentBrick(GameObject targetObject)
    {
        GameObject returnObject = targetObject;
        if(targetObject.CompareTag(SOCKET_TAG_MALE) || targetObject.CompareTag(SOCKET_TAG_FEMALE))
        {
            returnObject = targetObject.transform.parent.gameObject;
        }

        return returnObject;
    }




    #endregion
    

}
