using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamEdge : MonoBehaviour
{
    Cinemachine.CinemachineConfiner ccc;

    void Start()
    {
        ccc = GetComponent<Cinemachine.CinemachineConfiner>();
    }

    void Update()
    {
        if (ccc.m_BoundingVolume == null && GameObject.Find("CameraEdge").GetComponent<BoxCollider>() != null)
            ccc.m_BoundingVolume = GameObject.Find("CameraEdge").GetComponent<BoxCollider>();
    }
}
