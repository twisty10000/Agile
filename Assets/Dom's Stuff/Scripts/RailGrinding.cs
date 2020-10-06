﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RailGrinding : MonoBehaviour
{
    [SerializeField] private GameObject Hoverboard;
    [SerializeField] private float Speed = 5f;
    [Tooltip("Rail particle system to let player know they can grind")][SerializeField] private ParticleSystem RailParticle;
    private bool CanGrind = false;

    [Header("Tweener")]
    [Tooltip("How the board follows the rail path")][SerializeField] private PathType Rail = PathType.CatmullRom;
    [Tooltip("Put in the rail waypoints to create rail path")] [SerializeField] private Transform[] Paths = new Transform[3];
    private Vector3[] PathValue;

    private void Start()
    {
        PathValue.SetValue(Paths[0].position, 0);
        PathValue.SetValue(Paths[1].position, 1);
        Paths.SetValue(Paths[2].position, 2);
    }

    private void Update()
    {
        if (CanGrind && Input.GetKeyDown("e"))
        {
            Hoverboard.GetComponent<HoverboardInput>().enabled = false;
            Hoverboard.transform.DOPath(PathValue, Speed, Rail);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Hoverboard"))
        {
            RailParticle.Play();
            CanGrind = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Hoverboard"))
        {
            RailParticle.Stop();
            CanGrind = false;
        }
    }
}
