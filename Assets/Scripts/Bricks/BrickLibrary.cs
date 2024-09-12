using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickLibrary : MonoBehaviour
{

    [SerializeField]
    public List<GameObject> allBricks;

    public int[] brickInventory;

    [SerializeField]
    public List<GameObject> allMachines;

    public int[] machineInventory;

    [NonSerialized]
    public WristToolBehavior wristToolBehavior;

    public void Start()
    {
        brickInventory = new int[allBricks.Count];
        machineInventory = new int[allMachines.Count];

    }


    public void FixedUpdate()
    {
        UpdateMachineInventory();

    }


    void UpdateMachineInventory()
    {

        for(int i = 0; i < machineInventory.Length; i++)
        {
            machineInventory[i] = brickInventory[i] / 2;
        }


    }


}
