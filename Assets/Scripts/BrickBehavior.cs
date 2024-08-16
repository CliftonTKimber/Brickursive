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


    void Start()
    {

        gameController = GameObject.Find("Game Controller");
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {

        PreventShearing();
    }


    void PreventShearing()
    {
        Matrix4x4 worldMatrix = transform.localToWorldMatrix;

        Vector3 translation = worldMatrix.GetPosition();
        Vector3 scale = worldMatrix.lossyScale;
        Quaternion rotation = worldMatrix.rotation;

        


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
