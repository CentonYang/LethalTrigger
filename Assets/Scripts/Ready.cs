using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Ready : MonoBehaviour
{
    public bool ignore;
    public UnityEvent finishEvent;

    void Start()
    {
        if (ignore) Finish();
    }

    public void Finish()
    {
        finishEvent.Invoke();
    }
}
