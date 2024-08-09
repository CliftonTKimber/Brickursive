using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerScript : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject collidingObject;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        collidingObject = other.gameObject;

    }

    public void OnTriggerExit()
    {

        collidingObject = null;
    }
}
