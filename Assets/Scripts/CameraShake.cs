using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    public CinemachineVirtualCamera vcam;
    public float shakeTime, shakePower;

    void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
    }

    void FixedUpdate()
    {

        if (shakeTime > 0)
        {
            vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = shakePower;
            shakeTime -= 1;
        }
        else vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
    }

    public void SetShake(float tick, float power)
    {
        shakeTime = tick; shakePower = power;
    }
}
