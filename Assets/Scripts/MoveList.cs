using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MoveList : MonoBehaviour
{
    public Data[] data;

    [System.Serializable]
    public struct Data
    {
        public string name;
        public string staLost;
        public string blzLost;
    }
}