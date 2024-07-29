using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestGridUtils
{

    private GridUtils gridUtility = new GridUtils();

    private GameController tempSceneCamera;


    // A Test behaves as an ordinary method
    [Test]
    public void TestGridUtilsSimplePasses()
    {
        // Use the Assert class to test conditions
    }



    [Test]

    public void GetBottomLeftCornerOfObjectTest()
    {
        GameObject givenObject = new GameObject();
        Vector3 givenVector = givenObject.transform.position; //This will be middle of the object

        Vector3 cornerVector = gridUtility.GetBottomLeftCornerOfObject(givenObject);


        Assert.AreNotEqual(expected: givenVector, actual: cornerVector);

    }



    [Test]
    [TestCase(1,2,3)]
    [TestCase(-1,-2,-3)]
    public void CompareCornersBasedOnDifferentScales(float first, float second, float third)

    {

        Vector3 givenVector = Vector3.one;

        GameObject givenObject = new GameObject();
        GameObject compareObject = new GameObject();

        compareObject.transform.localScale += new Vector3(first, second, third);
       
        Vector3 rayVector = gridUtility.GetBottomLeftCornerOfObject(givenObject);
        Vector3 rayVector2 = gridUtility.GetBottomLeftCornerOfObject(compareObject);

        

        Assert.AreNotEqual(expected:rayVector2, actual: rayVector);

    }


    [Test]

    public void TestConvertToGrid_BasicFunction()
    {

        Vector3 localVector = Vector3.one;

        Vector3 worldVector = gridUtility.ConvertVectorToGridPosition(localVector, Vector3.one * 1.2f);


        Assert.AreNotEqual(expected: localVector, actual: worldVector);



    }

    [Test]
    [TestCase(0.25f,0.25f,0.25f)]
    [TestCase(1,1,1)]
    [TestCase(10,100,0.25f)]

    public void GridVectorIsNotTooLarge(float first, float second, float third)
    {

        Vector3 localVector = Vector3.one;

        Vector3 cellSize = new Vector3(first,  second,  third);

        Vector3 gridVector = gridUtility.ConvertVectorToGridPosition(localVector, cellSize);

        bool vectorIsTooLarge;

        if(gridVector.x % cellSize.x > 0 || gridVector.y % cellSize.y > 0 || gridVector.z % cellSize.z > 0)
            vectorIsTooLarge = true;
        else
            vectorIsTooLarge = false;



        Assert.IsFalse(vectorIsTooLarge);



    }

    [Test]
    [TestCase(0.25f,0.25f,0.25f)]
    [TestCase(1,1,1)]
    [TestCase(10,100,2)]

    public void GridVectorIsNotTooSmall(float first, float second, float third)
    {

        Vector3 localVector = Vector3.one;

        Vector3 cellSize = new Vector3(first, second, third);

        Vector3 gridVector = gridUtility.ConvertVectorToGridPosition(localVector, cellSize);

        bool vectorIsTooLarge;

        if(gridVector.x % cellSize.x < 0 || gridVector.y % cellSize.y < 0 || gridVector.z % cellSize.z < 0)
            vectorIsTooLarge = true;
        else
            vectorIsTooLarge = false;



        Assert.IsFalse(vectorIsTooLarge);



    }

    [Test]
    [TestCase(0.25f,0.25f,0.25f)]
    [TestCase(1,1,1)]
    [TestCase(10,100,2)]

    public void GridVectorCounterGivesCellCount(float first, float second, float third)
    {

        float travelSteps = 2f;


        Vector3 stepsVector = new Vector3(first,second, third) * travelSteps;
        

        Vector3 cellSize = Vector3.one;

        Vector3 gridVector = gridUtility.CountVectorStepsBasedOnCellSize(stepsVector, cellSize);

        float xCount = Mathf.RoundToInt(stepsVector.x / cellSize.x);
        float yCount = Mathf.RoundToInt(stepsVector.y / cellSize.y);
        float zCount = Mathf.RoundToInt(stepsVector.z / cellSize.z);


        Vector3 countVector = new Vector3(xCount, yCount, zCount);


        Assert.AreEqual(expected: countVector, actual: gridVector);





    }


    [Test]
    [TestCase(0.25f,0.25f,0.25f,0.25f)]
    [TestCase(1,1,1, 1)]
    [TestCase(10.64f,100.48f, 2.98f, 100)]

    public void ConvertsToGridPositionBasedOnStepsAndCellSizes(float first, float second, float third, float travelSteps)
    {


        Vector3 stepsVector =  Vector3.one * travelSteps;
        

        Vector3 cellSize = new Vector3(first,second,third);

        Vector3 gridVector = gridUtility.ConvertVectorToGridPosition(stepsVector, cellSize);

       
     
    

        float projectedX =  Mathf.RoundToInt( stepsVector.x / cellSize.x) * cellSize.x;
        float projectedY =  Mathf.RoundToInt( stepsVector.y / cellSize.y) * cellSize.y;
        float projectedZ =  Mathf.RoundToInt( stepsVector.z / cellSize.z) * cellSize.z;

        Vector3 projectedVector = new Vector3(projectedX, projectedY, projectedZ);

        Assert.AreEqual(expected: projectedVector, actual: gridVector);

    }


    [Test]
    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(-0.25f)]

    public void WhenVectorToStepsCounterIsGivenTooLowNumber_ThrowException(float lowNumber)
    {

        Assert.Throws<ArgumentOutOfRangeException>
        ( 
            delegate {  gridUtility.CountVectorStepsBasedOnCellSize(Vector3.one, Vector3.one * lowNumber); } 
        );


    }

    [Test]
    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(-0.25f)]

    public void WhenVectorToGridConverterIsGivenTooLowNumber_ThrowException(float lowNumber)
    {

        Assert.Throws<ArgumentOutOfRangeException>
        ( 
            delegate {  gridUtility.ConvertVectorToGridPosition(Vector3.one, Vector3.one * lowNumber); } 
        );

    }




    



    


}
