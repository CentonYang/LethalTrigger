using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : MonoBehaviour
{
    public int rand;

    void Awake()
    {
        rand = Random.Range(50, 100);
        GetComponentInChildren<MeshRenderer>().material.SetFloat("_random", rand);
    }

}
