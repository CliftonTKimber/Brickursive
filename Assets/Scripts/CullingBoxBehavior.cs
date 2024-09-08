using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CullingBoxBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider collider)
    {

        if(collider.gameObject == null)
        {
            return;
        }

        Destroy(collider.gameObject);
    }
}
