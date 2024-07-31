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

    public void Start()
    {
        baseCellSize = new Vector3(0.78f, 0.32f, 0.78f);
        raycastUtils = new();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool SnapObjectToGrid(GameObject targetObject,  bool objectIsHeld,  GameController gameController, float raycastLength= 0.25f)
    {
        if (objectIsHeld)
        {
            List<RaycastHit> hitList = raycastUtils.GetRaycastHitsFromChildrenBasedOnTags(targetObject, raycastLength);

            for(int i = 0; i < hitList.Count; i++)
            {
                if(hitList[i].collider != null)
                {
                    RaycastHit rayHit = hitList[i];
                    GameObject hitObject = rayHit.collider.gameObject;
                    Debug.Log(hitObject);
                    if(hitObject.CompareTag("Male"))
                    {
                       

                        PutObjectOntoGrid(targetObject, rayHit, hitObject, gameController);
  
                        return false;
                    }
                    else if(hitObject.CompareTag("Female"))
                    {
                        PutObjectOntoGrid(targetObject, rayHit, hitObject, gameController);
                        

                        return false;
                    }
                }
            }
            return objectIsHeld;   
        }
        return objectIsHeld;
    }

    private void PutObjectOntoGrid(GameObject targetObject, RaycastHit rayHit, GameObject hitObject, GameController gameController)
    {
        hitObject = hitObject.transform.parent.gameObject;
        Vector3 finalPos = GetFinalGridPosition(targetObject, rayHit, baseCellSize, raycastUtils);

        targetObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        targetObject.GetComponent<Rigidbody>().excludeLayers = 7; //Bricks

        targetObject.transform.rotation = hitObject.transform.rotation;
        targetObject.transform.position = finalPos;
        targetObject.transform.parent = hitObject.transform;

        if(targetObject.name != "Ghost Object")
            {gameController.SetObjectAndChildrenColliderEnabled(targetObject, true);}
    }

    public Vector3 GetFinalGridPosition(GameObject targetBrick, RaycastHit rayHit, Vector3 cellSize, RaycastUtils raycastUtils){


            //RaycastHit rayHit = raycastUtils.GetRaycastHitFromCameraRay(mousePosition, camera);
            if(rayHit.collider != null)
            {   Vector3 rayPosition = rayHit.point;

            
                if(raycastUtils.IsPositionAvailable(this, targetBrick,rayHit))
                {


                    Vector3 gridPosition = ConvertVectorToSteppedGridPosition(rayPosition, cellSize);

                    Vector3 hitSideOffset = GetFemaleSideOffset(targetBrick,rayHit.collider.gameObject, cellSize);

                    Vector3 objectDisplacedPos = GetDifferenceBetweenPositionAndGridPosition(rayHit.collider.transform.position, cellSize);

                    Vector3 brickOffset = GetBottomLeftCornerOfObject(targetBrick);



                    Vector3 finalPosition = gridPosition - brickOffset - hitSideOffset  + objectDisplacedPos;

                    //Debug.DrawLine(rayPosition, gridPosition);
                    //Debug.DrawLine(gridPosition, gridPosition - brickOffset);



                    return finalPosition;
                }
                else
                    {
                        //Debug.Log("Position unavailable");
                        return -Vector3.one;
                    }
            }
            else
            {
                //Debug.Log("Ray collided with nothing");
                return -Vector3.one;
            }

    }

    private static Vector3 GetFemaleSideOffset(GameObject targetObject, GameObject hitObject, Vector3 cellSize)
    {
        Vector3 hitSideOffset = new(0, cellSize.y/2, 0);
        if (hitObject.CompareTag("Female"))
        {
            hitSideOffset.y = (targetObject.transform.localScale.y) + cellSize.y/2;
        }

        return hitSideOffset;
    }

    public Vector3 GetBottomLeftCornerOfObject(GameObject targetObject){
        Vector3 vertexPos = new(1,-1,1); //BL - closest
        Vector3 cornerPosition = GetCubeVertex(targetObject, vertexPos);

        //Debug.DrawLine(targetObject.transform.position, targetObject.transform.position+ cornerPosition);

        return cornerPosition;


    }

    public Vector3 GetCubeVertex(GameObject targetObject, Vector3 targetVertex)
    {
        float widthOffset  = targetObject.transform.localScale.x / 2 * targetVertex.x;
        float heightOffset = targetObject.transform.localScale.y / 2 * targetVertex.y;
        float lengthOffset = targetObject.transform.localScale.z / 2 * targetVertex.z;

        Vector3 vertexPosition = new Vector3(widthOffset, heightOffset, lengthOffset);

        return vertexPosition;



    }

    public Vector3 GetBottomMiddleOfCellPosition(Vector3 cellSize, string colliderTag = "Male")
    {
        float heightOffset = 0;
        if (colliderTag == "Female")
        {
            //Debug.Log("Hit the bottom!");
            heightOffset = cellSize.z / 2;
        }

        float widthOffset  = cellSize.x /2;
        float lengthOffset = cellSize.z /2;

        
        Vector3 newPosition = new Vector3(widthOffset, heightOffset, lengthOffset);

        return newPosition;

    }

     public Vector3 GetBottomLeftCornerOfCellPosition(Vector3 cellSize)
    {
        float widthOffset  = cellSize.x / 2;
        float heightOffset = cellSize.y / 2;
        float lengthOffset = cellSize.z / 2;
 
        Vector3 newPosition = new Vector3(widthOffset, heightOffset, lengthOffset);

        return newPosition;

    }

    public Vector3 CountVectorStepsBasedOnCellSize(Vector3 targetVector, Vector3 cellSize)
    
    {
        if(cellSize.x > 0 && cellSize.y > 0 && cellSize.z > 0){


            Vector3 sizeOffset = GetBottomLeftCornerOfCellPosition(cellSize);
            //sizeOffset.y = 0;
            targetVector += sizeOffset;


            targetVector.x = Mathf.RoundToInt(targetVector.x / cellSize.x);
            targetVector.y = Mathf.RoundToInt(targetVector.y / cellSize.y);
            targetVector.z = Mathf.RoundToInt(targetVector.z / cellSize.z);

            return targetVector;
        }
        else
            throw new ArgumentOutOfRangeException("Every part of Vector3 cellSize must be larger than 0");
    }

    public Vector3 ConvertVectorToSteppedGridPosition(Vector3 targetVector, Vector3 cellSize)
    {

        if(cellSize.x > 0 && cellSize.y > 0 && cellSize.z > 0){

            Vector3 countVector = CountVectorStepsBasedOnCellSize(targetVector, cellSize);

            targetVector.x = countVector.x * cellSize.x;
            targetVector.y = countVector.y * cellSize.y;
            targetVector.z = countVector.z * cellSize.z;

            return targetVector;
        }
        else
            throw new ArgumentOutOfRangeException("Every part of Vector3 cellSize must be larger than 0");
    }

    public Vector3 GetDifferenceBetweenPositionAndGridPosition(Vector3 targetVector, Vector3 cellSize)
    {
        if(IsAnyPartOfVectorLessOrEqualToZero(cellSize))
        {
            throw new ArgumentException("No part of Vector3 cellSize may be Less than or Equal to 0.");
        }


        targetVector.x = Modulo(targetVector.x, cellSize.x);
        targetVector.y = Modulo(targetVector.y, cellSize.y);
        targetVector.z = Modulo(targetVector.z, cellSize.z);

        //Debug.Log(targetVector);

        return targetVector;        
    }


    //Broken
    public Vector3 GetCollisionFaceFromRaycast(RaycastUtils raycastUtils, Camera camera, Vector3 mousePosition)
    {
        RaycastHit rayHit = raycastUtils.GetRaycastHitFromCameraRay(mousePosition, camera); // <-- Problem Child.

        
        if (rayHit.collider != null)
        {

            GameObject hitObject = rayHit.collider.gameObject;


            Vector3 objectPos = hitObject.transform.position;
            Vector3 objectScale = ReturnVectorAsPositiveAndNotZero(hitObject.transform.localScale);

            // TODO: grid stuff to convert between world coords, and local coords(faces are local)

            Vector3 rayPos = rayHit.point;
            Vector3 hitFace = rayPos;



            if(rayPos.x == objectPos.x - (objectScale.x / 2))
            {
                hitFace = new Vector3(rayPos.x, 0, 0);
                //Debug.Log("Left!");
            }
            else if(rayPos.x == objectPos.x + (objectScale.x / 2))
            {
                hitFace = new Vector3(rayPos.x, 0, 0);
               // Debug.Log("Right!");
            }
            else if(rayPos.y == objectPos.y + (objectScale.y / 2))
            {
                hitFace = new Vector3(0, rayPos.y, 0);
                //Debug.Log("Top!");
            }
            else if(rayPos.y == objectPos.y - (objectScale.y / 2))
            {
                hitFace = new Vector3(0, rayPos.y, 0);
               // Debug.Log("Bottom!");
            }
            else if(rayPos.z == objectPos.z - (objectScale.z / 2))
            {
                hitFace = new Vector3(0, 0, rayPos.z);
               // Debug.Log("Back!");
            }
            else if(rayPos.z == objectPos.z + (objectScale.z / 2))
            {
                hitFace = new Vector3(0, 0, rayPos.z);
                //Debug.Log("Front!");
            }
            else
                Debug.Log("No Face?");




            return hitFace;
        }
        else
        {
            Debug.Log("rayHit.collider is null");
            return Vector3.zero;

        }
    }


    public Vector3 ReturnVectorAsPositiveAndNotZero(Vector3 targetVector)
    {

        if (targetVector.x == 0)
            targetVector.x = 1f;
        else if (targetVector.x < 0)
            targetVector.x *= -1; 

        if (targetVector.y == 0)
            targetVector.y = 1f;
        else if (targetVector.y < 0)
            targetVector.y *= -1;  

        if (targetVector.z == 0)
            targetVector.z = 1f;
        else if (targetVector.z < 0)
            targetVector.z *= -1;   


        return targetVector;

    }


    public float GetLargestPartOfTransformScale(Vector3 scale){

        float x = scale.x;
        float y = scale.y;
        float z = scale.z;

        float choiceFloat = x;


        if (choiceFloat < y)
            choiceFloat = y;
        if (choiceFloat < z)
            choiceFloat = z;


        return choiceFloat;

    }

    public bool IsAnyPartOfVectorLessOrEqualToZero(Vector3 targetVector)
    {
        bool isBool = false;

        if(targetVector.x <= 0f)
        {
            isBool = true;
        }
        if(targetVector.y <= 0f)
        {
             isBool = true;
        }
        if(targetVector.z <= 0f)
        {
            isBool = true;
        }

        return isBool;


    }

#region BrickCollision

    //Broken
    public bool IsBrickWithinSurroundingObjects(GameObject targetBrick)
    {

        Vector3 brickPos = targetBrick.transform.position;
        float largestScale = GetLargestPartOfTransformScale(targetBrick.transform.localScale);

        Collider[] objectsColliders = GetCollidersForBricksSurroundingVector(brickPos, largestScale * 1.5f);

        List<Bounds> objectsBounds = ReturnBoundsFromGivenColliders(objectsColliders);

        bool isInside = IsBrickVerticesWithinAnyBounds(targetBrick, objectsBounds);


        return isInside;
    }

    public List<Bounds> ReturnBoundsFromGivenColliders(Collider[] givenColliders)
    {
        List<Bounds> allBounds = new List<Bounds>();

        for(int i = 0; i < givenColliders.Length; i++)
        {
            allBounds.Append(givenColliders[i].bounds);
        }

        return allBounds;


    }

    public Collider[] GetCollidersForBricksSurroundingVector(Vector3 targetPosition, float sphereRadius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(targetPosition, sphereRadius);

        return hitColliders;
    }


    public bool IsBrickVerticesWithinAnyBounds(GameObject targetBrick, List<Bounds> bounds)
    {
        ///Not strict bounds, reduce by part of cellSize?
        Vector3[] vertices = GetAllCubeVertices(targetBrick);
        bool isOverlapping = false;


        for (int i = 0; i < bounds.Count; i++)
        {
            for (int j = 0; j < vertices.Length; j++ )
            {
                if (bounds[i].Contains(vertices[j]))
                {
                    isOverlapping = true;
                    break;
                }
            }

        }

        return isOverlapping;

    }

#endregion
#region 

    public Vector3[] GetAllCubeVertices(GameObject targetObject)
    {
        int vertCount = 8;
        Vector3[] vertices = new Vector3[vertCount];
        Vector3 tempV;

        for(int i = 0; i < vertCount; i++)
        {
            tempV = Vector3.zero;
            string binaryNumber = Convert.ToString(i, toBase: 2);

            MakeBitStringFitRequiredLength(binaryNumber, 3);

            for (int j = 0; j < binaryNumber.Length; j++)
            {
                int n = 1;
                if(binaryNumber[j] == '1')
                    n = -1;
                    
                tempV[j] = n;                      
            }

            vertices.Append(GetCubeVertex(targetObject, tempV));
      
            }

        return vertices;

    }


    public string MakeBitStringFitRequiredLength(string bitString, int reqLength)
    {
        for(int i = 0; i < reqLength - bitString.Length; i++)
        {
            bitString = "0" + bitString;
        }

        return bitString;


    }


    public float Modulo(float number, float divider)
    {
        float remainder = number % divider;
        if ((remainder<0 && divider>0) || (remainder>0 && divider<0)) {
            remainder += divider;
        }
        return remainder;
    }
#endregion




}
