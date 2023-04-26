using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Ready : MonoBehaviour
{
    public bool ignore;
    public UnityEvent finishEvent;
    public List<GameObject> maps;

    void Start()
    {
        if (ignore) Finish();
        else maps[GameSystem.map].SetActive(true);
    }

    public void Finish()
    {
        finishEvent.Invoke();
    }
}
