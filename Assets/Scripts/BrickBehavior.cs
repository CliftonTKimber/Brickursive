using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickBehavior : MonoBehaviour
{
    // Start is called before the first frame update

    public bool isColliding;
    void Start()
    {
        isColliding = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        
      //  isColliding = true;
    }
    private void OnTriggerStay(Collider other)
    {
        //Requires RigidBody
        isColliding = true;


    }

    private void OnTriggerExit(Collider other){

        isColliding = false;

    }
}
