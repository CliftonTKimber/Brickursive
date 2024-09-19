using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using static GameConfig;

public class SettingsController : MonoBehaviour
{


    private SoundController soundController;

    private GameObject workbenchFolder;
    private GameObject objectFolder;

    void Start()
    {

        soundController = GameObject.Find("Sound Controller").GetComponent<SoundController>();
        workbenchFolder = GameObject.Find("Workbenches");
        objectFolder = GameObject.Find(OBJECT_FOLDER_NAME);

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void IncrementHeightOnSelection(SelectEnterEventArgs eventData)
    {
        Vector3 newPos = workbenchFolder.transform.position;
        newPos.y += HEIGHT_ADJUSTMENT;

        workbenchFolder.transform.position = newPos;

        newPos = objectFolder.transform.position;
        newPos.y += HEIGHT_ADJUSTMENT;

        objectFolder.transform.position = newPos;

    }

    public void DecrementHeightOnSelection(SelectEnterEventArgs eventData)
    {

        Vector3 newPos = workbenchFolder.transform.position;
        newPos.y -= HEIGHT_ADJUSTMENT;

        workbenchFolder.transform.position = newPos;

        newPos = objectFolder.transform.position;
        newPos.y -= HEIGHT_ADJUSTMENT;

        objectFolder.transform.position = newPos;
    }


     public void SetScaleNormalOnSelection(SelectEnterEventArgs eventData)
    {

        transform.localScale = NORMAL_SCALE;

    }

    public void SetScaleTinyOnOnSelection(SelectEnterEventArgs eventData)
    {

        transform.localScale = TINY_SCALE;
    }

    public void ToggleSoundOnSelection(SelectEnterEventArgs eventData)
    {

        soundController.GetComponent<AudioSource>().mute = !soundController.GetComponent<AudioSource>().mute;

    }


}
