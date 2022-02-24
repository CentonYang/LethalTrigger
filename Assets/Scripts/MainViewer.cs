using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainViewer : MonoBehaviour
{
    public PlayerController player1;
    public float viewAngle;

    void Start()
    {

    }

    void Update()
    {
        transform.position = new Vector3(player1.transform.GetChild(0).position.x + viewAngle * player1.transform.GetChild(0).transform.localScale.z, transform.position.y, transform.position.z);
    }
}
