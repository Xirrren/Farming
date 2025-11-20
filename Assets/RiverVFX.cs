using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverVFX : MonoBehaviour
{
    public static RiverVFX instance{get; private set;}
    [SerializeField] private ParticleSystem ps;

    private void Awake()
    {
        instance = this;
    }

    public void PsPause()
    {
        ps.Pause();
    }
    
    public void PsPlay()
    {
        ps.Play();
    }
}
