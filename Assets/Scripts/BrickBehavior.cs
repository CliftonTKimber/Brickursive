using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using static GameConfig;


public class BrickBehavior : MonoBehaviour
{


    [NonSerialized]
    public GameObject gameController;

    public Transform newParent;

    public Vector3 trueScale;


    void Start()
    {

        gameController = GameObject.Find("Game Controller");

        SetTrueScale();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {

        

    }




    public void CallSnappingMethods(SelectEnterEventArgs eventData)
    {
        //NOTE: Cannot grab by child bricks yet. On OnlyPluckable Interaction Mask.
        if(gameController == null)
        {
            Start();
        }

        //transform.parent = GameObject.Find(OBJECT_FOLDER_NAME).transform;

        Transform nfInteractorTransform = eventData.interactorObject.transform;
        GameObject usedController = nfInteractorTransform.parent.gameObject;

        GameObject chosenObject = this.gameObject;

        BrickManager brickManager = gameController.GetComponent<GameController>().brickManager;

        brickManager.BeginSnapping(chosenObject, usedController);


    }

    public void EndSnappingMethods(SelectExitEventArgs eventData)
    {
        if(gameController == null)
        {
            Start();
        }

        Transform nfInteractorTransform = eventData.interactorObject.transform;
        GameObject usedController = nfInteractorTransform.parent.gameObject;

        BrickManager brickManager = gameController.GetComponent<GameController>().brickManager;


        brickManager.EndSnapping(usedController);

        if(newParent == null)
        {
            transform.parent = GameObject.Find(OBJECT_FOLDER_NAME).transform;
        }
        else
        {
            transform.parent = newParent;
        }


        ///The reason that the structure cannot be thrown is because this main piece, and its constituants
        ///have IsKinematic set to true; Disable to allow throwing.

    }


    public void ToggleRigidbodyIsKinematic()
    {
        GetComponent<Rigidbody>().isKinematic = !GetComponent<Rigidbody>().isKinematic;

    }


    public void InvokeBrickMethod(string brickMethodName, float time)
    {
        Invoke(brickMethodName, time);
    }


    


    
    void SetTrueScale()
    {

        Mesh objectMesh = GetComponent<MeshFilter>().mesh;


        trueScale = new( objectMesh.bounds.size.x * transform.lossyScale.x,
                         objectMesh.bounds.size.y * transform.lossyScale.y, 
                         objectMesh.bounds.size.z * transform.lossyScale.z);

        if(name == "3x3x3Blackbox")
        {
            trueScale.x -= STUD_HEIGHT * 2;
            trueScale.y -= STUD_HEIGHT * 2;
            trueScale.z -= STUD_HEIGHT * 2;

        }
    }


}
