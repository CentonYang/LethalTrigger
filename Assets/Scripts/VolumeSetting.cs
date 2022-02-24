using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VolumeSetting : MonoBehaviour
{
    ChromaticAberration ca;
    DepthOfField dof;
    public float timeSC;

    void Awake()
    {
        GetComponent<Volume>().profile.TryGet<ChromaticAberration>(out ca);
        GetComponent<Volume>().profile.TryGet<DepthOfField>(out dof);
    }

    void FixedUpdate()
    {
        if (Time.timeScale < 1)
        { ca.active = true; dof.active = true; }
        else
        { ca.active = false; dof.active = false; }
        Time.timeScale = timeSC;
    }
}
