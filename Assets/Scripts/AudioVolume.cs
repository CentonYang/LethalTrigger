using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVolume : MonoBehaviour
{
    public enum AudioType { bgm, sfx, voice }
    public AudioType audioType;
    public float scale = 1;

    void Start()
    {
        AudioSource audio = GetComponent<AudioSource>();
        switch (audioType)
        {
            case AudioType.bgm: audio.volume = (float)GameSystem.playerData.bgmVol * scale; break;
            case AudioType.sfx: audio.volume = (float)GameSystem.playerData.sfxVol * scale; break;
            case AudioType.voice: audio.volume = (float)GameSystem.playerData.voiceVol * scale; break;
        }
    }
}
