﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyBehavior))]
[RequireComponent(typeof(Rigidbody))]
public class ShootingAI : MonoBehaviour
{
    [Header("Testing")]
    [Tooltip("Toggle when wanted to test AI without loading whole game")] public bool AiTest = false;

    [Header("Player info")]
    [SerializeField] private Transform Player;
    private float PlayerDistance = 0;

    [Header("Enemy Statistics")]
    private EnemyBehavior EB;
    private EnemyScripableObject EnemyOBJ;
    private float ChasePlayerRange;
    private float AttackRange;
    [Tooltip("The enemy's face, where they look")] public Transform Face = null;
    private float Health;

    [Header("Behavior")]
    private bool Idling = false;

    [Header("Components")]
    private Animator animator;
    private Collider MainCollider;
    private Rigidbody MainRigibody;

    [Header("Movement")]
    [Tooltip("If you want the enemy to patrol place the transforms here. Leave empty to have enemy idle")] public Transform[] points;
    private int DestinationPoint = 0;
    private NavMeshAgent Agent;
    private Vector3 SpawnLocation;

    [Header("Shooter")]
    [Tooltip("Where the projectiles come from")] public Transform FirePoint = null;
    private float AttackCooldown = 0;
    private float BulletsPerShot;
    private float Bullets = 0;

    [Header("Ragdoll")]
    [SerializeField] private List<Rigidbody> RagdollBodies = new List<Rigidbody>();
    [SerializeField] private List<Collider> RagdollColliders = new List<Collider>();

    // States
    private enum State {Initial, Idle, Patrol, Chase, Attack, Dead};
    private State ActiveState = State.Initial;
    private bool IsAlive = true;

    private IEnumerator Start()
    {
        while (IsAlive == true)
        {
            switch (ActiveState)
            {
                case State.Initial:
                    Initial();
                    break;
                case State.Idle:
                    Idle();
                    break;
                case State.Patrol:
                    Patrol();
                    break;
                case State.Chase:
                    Chase();
                    break;
                case State.Attack:
                    StartAttack();
                    break;
                case State.Dead:
                    DoDeath();
                    break;
            }
            yield return null;
        }
    }

    private void Initial()
    {
        if (!AiTest)
        {
            Player = PlayerRefs.instance.Player;
        }

        gameObject.tag = "Enemy";

        EB = GetComponent<EnemyBehavior>();
        EnemyOBJ = EB.EnemyOBJ;

        Health = EB.CurrentHealth;

        animator = gameObject.GetComponentInChildren<Animator>();

        Agent = GetComponent<NavMeshAgent>();
        Agent.speed = EnemyOBJ.MovementSpeed;

        SpawnLocation = transform.position;

        ChasePlayerRange = EnemyOBJ.ChaseRange;
        AttackRange = EnemyOBJ.AttackRange;

        MainRigibody = GetComponent<Rigidbody>();
        MainCollider = GetComponent<Collider>();

        RagdollBodies = GetComponentsInChildren<Rigidbody>().ToList();
        RagdollBodies.Remove(MainRigibody);

        RagdollColliders = GetComponentsInChildren<Collider>().ToList();
        RagdollColliders.Remove(MainCollider);

        ToggleRagdoll(false);

        if(ChasePlayerRange <= AttackRange)
        {
            ChasePlayerRange = 1 + AttackRange;
        }

        BulletsPerShot = EnemyOBJ.BulletsPerShot;

        if(points.Length == 0)
        {
            ActiveState = State.Idle;
            Idling = true;
        }
        else
        {
            ActiveState = State.Patrol;
        }
    }

    private void Idle()
    {

    }

    private void Patrol()
    {
        if (points.Length == 0)
            return;
        Agent.destination = points[DestinationPoint].position;
        DestinationPoint = (DestinationPoint + 1) % points.Length;
    }

    private void Chase()
    {
        Agent.isStopped = false;
        animator.SetBool("Running", true);
        transform.LookAt(Player.position);
        Agent.destination = Player.position;
    }

    private void StartAttack()
    {
        Agent.isStopped = true;
        animator.SetBool("Running", false);
        Vector3 LookPos = Player.position - transform.position;
        Quaternion LookRotation = Quaternion.LookRotation(LookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, LookRotation, 5 * Time.deltaTime);

        if (AttackCooldown <= 0)
        {
            Shoot();
            AttackCooldown = 1 / EnemyOBJ.AttackRate;
        }
        AttackCooldown -= Time.deltaTime;
    }

    private void Shoot()
    {
        while(Bullets < BulletsPerShot)
        {
            FirePoint.LookAt(Player);
            animator.SetTrigger("Shoot");
            ObjectPooling.Spawn(EnemyOBJ.ProjectileOBJ.Projectile, FirePoint.position, FirePoint.rotation);
            Bullets += 1;
        }
        if(Bullets >= BulletsPerShot)
        {
            Bullets = 0;
        }
    }

    private void DoDeath()
    {
        IsAlive = false;
        Agent.isStopped = true;
        int RandomNumber = Random.Range(0, 100);
        if (RandomNumber <= EnemyOBJ.PickupOBJ.DropChance)
        {
            ObjectPooling.Spawn(EnemyOBJ.PickupOBJ.PickupGameObject, transform.position + new Vector3(0f, 0.5f, 0f), transform.rotation);
        }
        ToggleRagdoll(true);
        foreach (Rigidbody Bodies in RagdollBodies)
        {
            Bodies.AddExplosionForce(107, Player.position, 5, 0.5f, ForceMode.Impulse);
        }
    }

    private void Update()
    {
        PlayerDistance = Vector3.Distance(transform.position, Player.position);

        Health = EB.CurrentHealth;

        Vector3 LookPos = Player.position - transform.position;

        Quaternion LookRotation = Quaternion.LookRotation(LookPos);

        if (Health <= 0)
        {
            ActiveState = State.Dead;
        }

        else if (PlayerDistance <= ChasePlayerRange && PlayerDistance > AttackRange)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, LookRotation, 5 * Time.deltaTime);
            if (Physics.Raycast(Face.position, transform.forward, out RaycastHit hit, Mathf.Infinity))
            {
                if (hit.transform.CompareTag("Player"))
                {
                    ActiveState = State.Chase;
                }
            }
        }
        else if (PlayerDistance <= AttackRange)
        {
            ActiveState = State.Attack;
        }

        else if (PlayerDistance > ChasePlayerRange && Idling)
        {
            ActiveState = State.Idle;
            if (transform.position != SpawnLocation)
            {
                Agent.destination = SpawnLocation;
            }
        }
    }

    private void ToggleRagdoll(bool state)
    {
        animator.enabled = !state;

        foreach (Rigidbody RB in RagdollBodies)
        {
            RB.isKinematic = !state;
            RB.useGravity = state;

            MainRigibody.isKinematic = state;
            MainRigibody.useGravity = !state;
        }

        foreach (Collider collider in RagdollColliders)
        {
            collider.enabled = state;
            MainCollider.enabled = !state;
        }
    }
}
