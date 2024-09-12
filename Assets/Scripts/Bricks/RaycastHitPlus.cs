using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastHitPlus 
{
    private Vector3 m_RayOrigin;
    private RaycastHit m_RaycastHit;

    private GameObject m_OriginSocket;



    public GameObject originSocket
    {
        get
        {
            return m_OriginSocket;
        }
        set
        {
            m_OriginSocket = value;
        }
    }


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

}
