using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using static GameConfig;
using TMPro;
public class WristToolBehavior : MonoBehaviour
{
    private BrickLibrary brickLibrary;

    private SoundController soundController;

    public enum DisplayType
    {

        MachineDisplay,

        BrickDisplay

        
    }


    public DisplayType wristDisplay = DisplayType.MachineDisplay;


    public List<Material> displayMaterials;

    private GameObject mainCamera;

    public int rowNum = 0;

    void Start()
    {
        brickLibrary = GameObject.Find("Brick Library").GetComponent<BrickLibrary>();
        brickLibrary.wristToolBehavior = this;

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

        soundController = GameObject.Find("Sound Controller").GetComponent<SoundController>();

        SetScaleBasedOnXRScale();
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(wristDisplay == DisplayType.MachineDisplay)
        {DisplayMachineInventory();}
        else
            DisplayBrickInventory();
        

    }


    void DisplayMachineInventory()
    {
       
        for(int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);


            //Label
            Transform childTextTransform = child.GetChild(0).GetChild(0); //Canvas -> Text

            TMP_Text childText = childTextTransform.GetComponent<TMP_Text>();

            childText.text = brickLibrary.machineInventory[i].ToString();




            //Machine
            if(brickLibrary.machineInventory[i] <= 0)
            {
                child.GetComponent<MeshRenderer>().material = displayMaterials[0];
                continue;
            }

            child.GetComponent<MeshRenderer>().material = displayMaterials[1];

        }
    }
    void DisplayBrickInventory()
    {

        int steppedChildCount = (rowNum * 4) + 4;
        if(steppedChildCount > transform.childCount)
        {
            steppedChildCount = transform.childCount;
        }

       //Make sure child count is in line with these numbers.
        for(int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);


            //Label
            Transform childTextTransform = child.GetChild(0).GetChild(0); //Canvas -> Text

            TMP_Text childText = childTextTransform.GetComponent<TMP_Text>();

            childText.text = brickLibrary.brickInventory[i].ToString();
            
            //Brick
            if(brickLibrary.brickInventory[i] <= 0)
                child.GetComponent<MeshRenderer>().material = displayMaterials[0];
            else
                child.GetComponent<MeshRenderer>().material = displayMaterials[1];

            if(i >= rowNum * 4 && i < steppedChildCount)
                child.gameObject.SetActive(true);
            else
                child.gameObject.SetActive(false);

        }
    }


    void InstantiateMachine(int inventorySlot, Pose spawnPose)
    {
        GameObject machine = brickLibrary.allMachines[inventorySlot];

        Transform parent = GameObject.Find(OBJECT_FOLDER_NAME).transform;

        GameObject machineInstance = Instantiate(machine, spawnPose.position, spawnPose.rotation, parent);

        machineInstance.GetComponent<Rigidbody>().AddForce(machineInstance.transform.forward * 2f, ForceMode.Impulse);

        machineInstance.name = machine.name;
    }

    void InstantiateBrick(int inventorySlot, Pose spawnPose)
    {
        GameObject brick = brickLibrary.allBricks[inventorySlot];

        Transform parent = GameObject.Find(OBJECT_FOLDER_NAME).transform;

        GameObject brickInstance = Instantiate(brick, spawnPose.position, spawnPose.rotation, parent);

        brickInstance.GetComponent<Rigidbody>().AddForce(brickInstance.transform.forward * 2f, ForceMode.Impulse);

        brickInstance.name = brick.name;
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


    public void SpawnMachineOnSelection(SelectEnterEventArgs eventData)
    {

        int siblingIndex = eventData.interactableObject.transform.GetSiblingIndex();

        Pose spawnPose = eventData.interactableObject.transform.GetWorldPose();
        spawnPose.position = transform.parent.position;



        if(brickLibrary.machineInventory[siblingIndex] <= 0)
            return;

        InstantiateMachine(siblingIndex, spawnPose);
        brickLibrary.brickInventory[siblingIndex] -= 2;        

        soundController.PlayMakeBrick(eventData.interactableObject.transform.position);      

    }

    public void SpawnBrickOnSelection(SelectEnterEventArgs eventData)
    {

        int siblingIndex = eventData.interactableObject.transform.GetSiblingIndex();

        Pose spawnPose = eventData.interactableObject.transform.GetWorldPose();
        spawnPose.position = transform.parent.position;



        if(brickLibrary.brickInventory[siblingIndex] <= 0)
            return;

        InstantiateBrick(siblingIndex, spawnPose);
        brickLibrary.brickInventory[siblingIndex] -= 1;  

        soundController.PlayMakeBrick(eventData.interactableObject.transform.position);      
    }

    public void IncrementOnSelection(SelectEnterEventArgs eventData)
    {
        rowNum += 1;

        if(rowNum > 2)
            rowNum = 2;

    }

    public void DecrementOnSelection(SelectEnterEventArgs eventData)
    {
        rowNum -= 1;

        if(rowNum < 0)
            rowNum = 0;
    }


#endregion


}

