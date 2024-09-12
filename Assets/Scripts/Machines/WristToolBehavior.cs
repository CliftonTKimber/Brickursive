using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameConfig;
public class WristToolBehavior : MonoBehaviour
{
    private BrickLibrary brickLibrary;

    void Start()
    {
        brickLibrary = GameObject.Find("Brick Library").GetComponent<BrickLibrary>();
        brickLibrary.wristToolBehavior = this;

        SetScaleBasedOnXRScale();
        CreateInventoryDisplay();
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckChildrenAndAddMachineToWrist();
        DisplayInventory();

    }


    void DisplayInventory()
    {

        for(int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);

            child.gameObject.SetActive(true);
        }
    }


    void CreateInventoryDisplay()
    {
        for(int i = 0; i < brickLibrary.allMachines.Count; i++)
        {
            
            StartCoroutine(InstantiateBrick(i));
            
        }


    }

    IEnumerator InstantiateBrick(int inventorySlot)
    {

        yield return new WaitForSeconds(PICKUP_GRACE_TIME);


        GameObject machine = brickLibrary.allMachines[inventorySlot];

        Debug.Log("Pop!");

        Vector3 newPos = transform.position;
        newPos.z -= 5;
        newPos.z -= 3 * (inventorySlot + 1);

        GameObject machineInstance = Instantiate(machine, newPos, transform.rotation, transform);
        machineInstance.transform.SetSiblingIndex(inventorySlot);

        newPos.z -= machineInstance.GetComponent<BrickBehavior>().trueScale.z;

        machineInstance.GetComponent<BrickBehavior>().isOnWrist = true;

        machineInstance.transform.position = newPos;

        machineInstance.SetActive(false);

        if(machineInstance.GetComponent<BlackboxBehavior>() != null)
            machineInstance.GetComponent<BlackboxBehavior>().enabled = false;

        if(machineInstance.GetComponent<PowerBehavior>() != null)
            machineInstance.GetComponent<PowerBehavior>().enabled = false;

        if(machineInstance.GetComponent<BrickBehavior>() != null)
            machineInstance.GetComponent<BrickBehavior>().enabled = false;

        /*if(machineInstance.GetComponent<PowerBehavior>() != null)
            machineInstance.GetComponent<PowerBehavior>().enabled = false;*/

        yield return new WaitForEndOfFrame();

        yield return null;



    }

    public void DecrementInventoryOnSelect(GameObject brick)
    {

        for(int i = 0; i < brickLibrary.allMachines.Count; i++)
        {
            string libBrickName = brickLibrary.allMachines[i].name;
            libBrickName += "(Clone)";

            string brickName = brick.name;
            if(brickName != libBrickName)
                continue;

            if(brickLibrary.machineInventory[i] <= 0)
                continue;

        }
    }

    void CheckChildrenAndAddMachineToWrist()
    {
        if(transform.childCount == brickLibrary.allMachines.Count)
            return;

        for(int i = 0; i < brickLibrary.allMachines.Count; i++)
        {
            string libBrickName = brickLibrary.allMachines[i].name;
            libBrickName += "(Clone)";

            if(i <= transform.childCount - 1 && transform.childCount > 0)
            {
                string brickName = transform.GetChild(0).name;
                if(libBrickName == brickName)
                    continue;

                brickName = transform.GetChild(1).name;
                if(libBrickName == brickName)
                    continue;

                brickName = transform.GetChild(2).name;
                if(libBrickName == brickName)
                    continue;
            }

            if(brickLibrary.machineInventory[i] > 0)
            {   
                StartCoroutine(InstantiateBrick(i));
                brickLibrary.brickInventory[i] -= 2;
            }

            

            
        }

        
    }
    void SetScaleBasedOnXRScale()
    {
        Vector3 xrScale = GameObject.Find("XR Origin (XR Rig)").transform.localScale;

        Vector3 newScale = new (1 / xrScale.x,
        1 / xrScale.y,
        1 / xrScale.z);
        transform.localScale = newScale;
    }
}
