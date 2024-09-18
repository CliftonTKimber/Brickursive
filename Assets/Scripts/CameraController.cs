using static GameConfig;

//using System.Numerics;

//using System.Numerics;

using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
   


    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {

      
    
        
    }

    void FixedUpdate()
    {

    }


    void OnTriggerEnter(Collider collider)
    {
        if(!collider.CompareTag(BASE_BRICK_TAG))
            return;

        if(collider.transform.parent != null)
            return;


        GameObject hitBrick = collider.gameObject;

        hitBrick.GetComponent<BrickBehavior>().soundController.PlayDevourBrick(hitBrick.transform.position);

        Destroy(hitBrick);


    }
    
   

}
