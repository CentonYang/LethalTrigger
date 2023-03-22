using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Translater : MonoBehaviour
{
    [Multiline]
    public List<string> contents;

    void Start()
    {
        GetComponent<Text>().text = contents[GameSystem.playerData.language];
    }
}
