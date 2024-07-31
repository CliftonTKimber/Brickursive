using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BrickBehavior : MonoBehaviour
{



    public bool isHeld;

    private RaycastUtils raycastUtils;
    private GridUtils gridUtils;

    private List<RaycastHit> hitList;
    void Start()
    {
        isHeld = true;
        raycastUtils = new RaycastUtils();
        
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
