using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;

public class BeltBehavior : MonoBehaviour
{
    // Start is called before the first frame update

    

    [SerializeField]
    private float beltSpeed, rotateSpeed;
    [SerializeField]
    private Vector3 direction;
    [SerializeField]
    private List<GameObject> onBelt;




    void Start()
    {
      
        rotateSpeed = 1f;
        //direction = GetComponent<Rigidbody>().transform.forward;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        

       for (int i = 0; i <= onBelt.Count - 1; i++)
        {
            

           onBelt[i].GetComponent<ItemBehavior>().Move(GetComponent<Rigidbody>().transform, beltSpeed, rotateSpeed);
            

           
        }


        
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Item")){
            onBelt.Add(other.gameObject);
        }

    }

    private void OnTriggerExit(Collider other){

        //onBelt.Remove(other.gameObject);
    }

    
}
