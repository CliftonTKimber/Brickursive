using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameConfig;

public class PowerBehavior : MonoBehaviour
{
    public enum ComponentType 
    
    {
        Transmitter,

        Battery
    }

    public int powerLevel = 1;

    public GameObject otherHalf = null;

    public ComponentType powerComponent = ComponentType.Transmitter;

    private List<GameObject> structures = new();
    void Start()
    {
        
    }

    void FixedUpdate()
    {
         Invoke(nameof(RunBehaviorByComponentType), ANIMATION_UPDATE_TIME);
    }


    private void RunBehaviorByComponentType()
    {

        if(otherHalf == null)
            return;

        if(transform.parent == null)
            return;   
        
        if(!transform.parent.CompareTag(BASE_BRICK_TAG))
            return;

        if(transform.parent.GetComponent<BlackboxBehavior>() == null)
            return;


        switch(powerComponent)
        {

            case ComponentType.Transmitter:
                RunTransmitterBehavior();

                break;

            case ComponentType.Battery:
                RunBatteryBehavior();
                break;



            default:
                return;
        }



    }


    private void  RunTransmitterBehavior()
    {
        /// NOTE: Replace Sphere collider with a hemisphere.


        /*for(int i = 0; i < structures.Count; i++)
        {
            GameObject machine = structures[i];

            if(machine.GetComponent<PowerBehavior>() != null && 
            machine.GetComponent<PowerBehavior>().powerComponent != ComponentType.Battery)
            {
                machine.GetComponent<PowerBehavior>().powerLevel = powerLevel;
            }
            if(machine.GetComponent<BlackboxBehavior>() != null)
            {
                machine.GetComponent<BlackboxBehavior>().powerLevel = powerLevel;
            }


            

        }*/
        

    }

     

     private void RunBatteryBehavior()
    {
        if(transform.childCount <= 6) // number of children including sockets
            return;  
    }




    public void OnTriggerEnter(Collider collider)
    {

        if(collider.gameObject == null)
            return;

        GameObject foundObject = collider.gameObject;

        if(foundObject.GetComponent<BlackboxBehavior>() != null)
        {
            foundObject.GetComponent<BlackboxBehavior>().powerLevel += powerLevel;
        }
        
        
        else if(foundObject.GetComponent<PowerBehavior>() != null)
        {
            foundObject.GetComponent<PowerBehavior>().powerLevel += powerLevel;
        }
        else
        {
            return;
        }



        structures.Add(foundObject);
    }

    public void OnTriggerExit(Collider collider)
    {

        if(collider.gameObject == null)
            return;

        GameObject foundObject = collider.gameObject;

        if(foundObject.GetComponent<BlackboxBehavior>() != null)
        {
            foundObject.GetComponent<BlackboxBehavior>().powerLevel -= powerLevel;
        }
        
        
        else if(foundObject.GetComponent<PowerBehavior>() != null)
        {
            foundObject.GetComponent<PowerBehavior>().powerLevel -= powerLevel;
        }
        else
        {
            return;
        }

        structures.Remove(foundObject);

    }
}
