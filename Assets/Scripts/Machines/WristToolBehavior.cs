using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using static GameConfig;
public class WristToolBehavior : MonoBehaviour
{
    private BrickLibrary brickLibrary;


    public List<Material> displayMaterials;

    private GameObject mainCamera;

    void Start()
    {
        brickLibrary = GameObject.Find("Brick Library").GetComponent<BrickLibrary>();
        brickLibrary.wristToolBehavior = this;

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

        SetScaleBasedOnXRScale();
        CreateInventoryDisplay();
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        DisplayInventory();

    }


    void DisplayInventory()
    {
       
        for(int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);


            if(brickLibrary.machineInventory[i] <= 0)
            {
                child.GetComponent<MeshRenderer>().material = displayMaterials[0];
                continue;
            }

            child.GetComponent<MeshRenderer>().material = displayMaterials[1];



        }
    }


    void CreateInventoryDisplay()
    {
        for(int i = 0; i < brickLibrary.allMachines.Count; i++)
        {
            
            ///StartCoroutine(InstantiateBrick(i));
            
        }


    }

    void InstantiateMachine(int inventorySlot, Pose spawnPose)
    {
        GameObject machine = brickLibrary.allMachines[inventorySlot];

        Transform parent = GameObject.Find(OBJECT_FOLDER_NAME).transform;

        GameObject machineInstance = Instantiate(machine, spawnPose.position, spawnPose.rotation, parent);



    }


    void SetScaleBasedOnXRScale()
    {
        Vector3 xrScale = GameObject.Find("XR Origin (XR Rig)").transform.localScale;

        Vector3 newScale = new (1 / xrScale.x,
        1 / xrScale.y,
        1 / xrScale.z);
        transform.localScale = newScale;
    }

#region XR

    public void HoverStuff(HoverEnterEventArgs eventData)
    {
        Debug.Log("I see you!");
    }
    public void SpawnMachineOnSelection(SelectEnterEventArgs eventData)
    {

        int siblingIndex = eventData.interactableObject.transform.GetSiblingIndex();

        Pose spawnPose = eventData.interactableObject.transform.GetWorldPose();



        if(brickLibrary.machineInventory[siblingIndex] <= 0)
            return;

        InstantiateMachine(siblingIndex, spawnPose);
        brickLibrary.brickInventory[siblingIndex] -= 2;
            
        


    }

#endregion


}

