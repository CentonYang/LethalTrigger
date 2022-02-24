using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceWall : MonoBehaviour
{
    public Transform char1, char2;
    float distance;

    void Start()
    {
        char1 = GameObject.Find("Player1").transform.GetChild(0).transform;
        char2 = GameObject.Find("Player2").transform.GetChild(0).transform;
    }

    // Update is called once per frame
    void Update()
    {
        //print(Vector2.Distance(Vector2.right * char1.position.x, Vector2.right * char2.position.x));
        transform.position = Vector2.Lerp(transform.position, (char1.position + char2.position) / 2, 0.05f);
    }

}
