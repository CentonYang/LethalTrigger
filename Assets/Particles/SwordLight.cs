using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordLight : MonoBehaviour
{
    public TrailRenderer trail;

    void Awake()
    {
        trail.emitting = false;
    }

    public void Trigger(int emit)
    {
        trail.emitting = emit == 1 ? true : false;
    }
}
