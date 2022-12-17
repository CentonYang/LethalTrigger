using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sounder : MonoBehaviour
{
    AudioSource ad, tad;

    void Start()
    {
        ad = GetComponent<AudioSource>();
        tad = transform.parent.GetComponentInParent<AudioSource>();
        if (tad.clip == null) Destroy(gameObject);
        ad.clip = tad.clip;
        ad.Play();
    }

    void Update()
    {
        if (ad.clip != null && !ad.isPlaying)
            Destroy(gameObject);
    }
}
