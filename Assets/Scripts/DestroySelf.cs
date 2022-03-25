using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class DestroySelf : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, GetComponent<VisualEffect>().GetFloat("lifetime"));
    }
}
