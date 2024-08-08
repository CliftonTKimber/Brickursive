using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastHitPlus 
{
    private Vector3 m_RayOrigin;
    private RaycastHit m_RaycastHit;


    public RaycastHit raycastHit
    {
        get
        {
            return m_RaycastHit;
        }
        set
        {
            m_RaycastHit = value;
        }
    }
    public Vector3 rayOrigin
    {
        get
        {
            return m_RayOrigin;
        }
        set
        {
            m_RayOrigin = value;
        }
    }


    public void SetRaycastHit(RaycastHit raycastHit)
    {
        m_RaycastHit = raycastHit;
    }

     public void SetRayOrigin(Vector3 originVector)
    {
        m_RayOrigin = originVector;
    }
}
