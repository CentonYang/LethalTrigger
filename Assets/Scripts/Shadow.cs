using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour
{
    public GameObject player;
    void Update()
    {
        transform.position = Vector2.right * player.transform.GetChild(0).position.x;
    }
}
