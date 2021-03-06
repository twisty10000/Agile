﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaseSequence : MonoBehaviour
{
    public enum State { Fleeing, Taunting, PitFall, Door, ShockTrap, FirePlume, Launcher}
    public State ActiveState;

    [Header("Boss Movement")]
    [SerializeField] private Transform BossFleeTarget;
    private NavMeshAgent agent;
    private Animator animator;

    [Header("Hazards")]
    [SerializeField] private float HazardDamage;

    private void Start()
    {
        switch (ActiveState)
        {
            case State.Fleeing:
                gameObject.tag = "Boss";
                animator = GetComponent<Animator>();
                animator.SetBool("Running", true);
                agent = GetComponent<NavMeshAgent>();
                break;
            case State.Taunting:
                animator = GetComponent<Animator>();
                animator.SetBool("Taunting", true);
                break;
            case State.PitFall:
                rb = Pit.GetComponent<Rigidbody>();
                break;
            case State.Door:
                SecurityDoor.position = OpenPosition.position;
                break;
            case State.Launcher:
                StartCoroutine(ShootLauncher());
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boss"))
        {
            Debug.Log("Boss");
            if(ActiveState == State.Door)
            {
                Debug.Log("Close Door");
                StartCoroutine(securityDoor());
            }
        }

        if (other.CompareTag("Player"))
        {
            switch (ActiveState)
            {
                case State.ShockTrap:
                    other.GetComponent<Player>().TakeDamage(HazardDamage);
                    break;
                case State.FirePlume:
                    other.GetComponent<Player>().TakeDamage(HazardDamage);
                    break;
                case State.PitFall:
                    pitFall();
                    break;
                case State.Fleeing:
                    agent.destination = BossFleeTarget.position;
                    //GetComponent<Collider>().enabled = false;
                    break;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch (ActiveState)
            {
                case State.ShockTrap:
                    other.GetComponent<Player>().TakeDamage(HazardDamage);
                    break;
                case State.FirePlume:
                    other.GetComponent<Player>().TakeDamage(HazardDamage);
                    break;
            }
        }
    }

    #region Security Door
    [Header("Security Door")]
    [SerializeField] private Transform SecurityDoor;
    [SerializeField] private Transform ClosePosition;
    [SerializeField] private Transform OpenPosition;
    [SerializeField] private float DoorMovementSpeed = 10;

    private IEnumerator securityDoor()
    {
        float t = 0;

        Vector3 startPosition = SecurityDoor.position;

        while (t < DoorMovementSpeed)
        {
            t += Time.deltaTime;
            SecurityDoor.position = Vector3.Lerp(startPosition, ClosePosition.position, t / DoorMovementSpeed);
            yield return null;
        }
    }
    #endregion

    #region Pitfall
    [Header("Pit fall")]
    [SerializeField] private GameObject Pit;
    private Rigidbody rb;

    private void pitFall()
    {
        rb.useGravity = true;
    }
    #endregion

    #region Launcher
    [Header("Launcher")]
    [SerializeField] private GameObject Projectile;
    [SerializeField] private Transform FirePoint;
    [SerializeField] private float FireRate;

    private IEnumerator ShootLauncher()
    {
        ObjectPooling.Spawn(Projectile, FirePoint.position, Quaternion.identity);

        yield return new WaitForSeconds(FireRate);

        StartCoroutine(ShootLauncher());
    }
    #endregion

    private void Update()
    {
        if(ActiveState == State.Fleeing)
        {
            if(agent.remainingDistance <= agent.stoppingDistance)
            {
                //Destroy(gameObject);
            }
        }
    }
}
