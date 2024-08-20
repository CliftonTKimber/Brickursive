using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using static GameConfig;


public class BrickBehavior : MonoBehaviour
{



    public GameObject gameController;

    public Transform newParent;

    public Vector3 trueScale;


    void Start()
    {

        gameController = GameObject.Find("Game Controller");

        Mesh objectMesh = GetComponent<MeshFilter>().mesh;
        trueScale = new( objectMesh.bounds.size.x * transform.lossyScale.x,
                                    objectMesh.bounds.size.y * transform.lossyScale.y, 
                                    objectMesh.bounds.size.z * transform.lossyScale.z);
        
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
        if(gameController == null)
        {
            Start();
        }

        //transform.parent = GameObject.Find(OBJECT_FOLDER_NAME).transform;

        GameObject usedController = eventData.interactorObject.transform.parent.gameObject;

        gameController.GetComponent<GameController>().BeginSnapping(this.gameObject, usedController);


    }

    public void EndSnappingMethods(SelectExitEventArgs eventData)
    {
        if(gameController == null)
        {
            Start();
        }


        GameObject usedController = eventData.interactorObject.transform.gameObject;

        gameController.GetComponent<GameController>().EndSnapping(usedController);

        if(newParent == null)
        {
            transform.parent = GameObject.Find(OBJECT_FOLDER_NAME).transform;
        }
        else
        {
            transform.parent = newParent;
        }

    }


    


    


    private void OnTriggerEnter(Collider other)
    {


        
    }
    private void OnTriggerStay(Collider other)
    {



    }

    private void OnTriggerExit(Collider other){


    }


}
