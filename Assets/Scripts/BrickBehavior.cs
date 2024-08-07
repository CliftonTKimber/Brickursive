using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GameConfig;


public class BrickBehavior : MonoBehaviour
{



    public bool isHeld;


    void Start()
    {
        isHeld = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {


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
