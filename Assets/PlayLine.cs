﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayLine : MonoBehaviour
{

    [SerializeField] public BossNagging BN;
    public int LineToPlay;

    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            BN.PlayIntLine(LineToPlay);
            Destroy(gameObject);
        }
    }

}
