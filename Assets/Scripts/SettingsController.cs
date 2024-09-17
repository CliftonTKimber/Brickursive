using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using static GameConfig;

public class SettingsController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void IncrementHeightOnSelection(SelectEnterEventArgs eventData)
    {
        Vector3 newScale = transform.lossyScale;
        newScale.y += HEIGHT_ADJUSTMENT;

        transform.localScale = newScale;

    }

    public void DecrementHeightOnSelection(SelectEnterEventArgs eventData)
    {

        Vector3 newScale = transform.lossyScale;
        newScale.y -= HEIGHT_ADJUSTMENT;

        transform.localScale = newScale;
    }


     public void SetScaleNormalOnSelection(SelectEnterEventArgs eventData)
    {

        transform.localScale = NORMAL_SCALE;

    }

    public void SetScaleTinyOnOnSelection(SelectEnterEventArgs eventData)
    {

        transform.localScale = TINY_SCALE;
    }


}
