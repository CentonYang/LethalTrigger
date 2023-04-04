using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterColor : MonoBehaviour
{
    public List<Material> materials;

    public void ColorChange(int pc)
    {
        GetComponent<Renderer>().material = materials[pc == 0 ? GameSystem.p1Color : GameSystem.p2Color];
    }
}
