using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemBehavior : MonoBehaviour
{
    // Start is called before the first frame update

    

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        


        

    }


    public void Move(Transform beltTransform, float beltSpeed, float beltRotateSpeed){

        //Movement
        //GetComponent<Rigidbody>().transform.Translate(beltDirection * beltSpeed * Time.deltaTime);
        //rotation

        //var dirDiff = (GetComponent<Rigidbody>().transform.forward - beltDirection);

        
        GetComponent<Rigidbody>().transform.LookAt(beltTransform);
        //GetComponent<Rigidbody>().transform.Rotate(beltDirection * beltRotateSpeed * Time.deltaTime);
    


    }



}
