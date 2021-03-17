﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBoss : MonoBehaviour
{
    [SerializeField] private Transform Player;

    [Header("Components")]
    [SerializeField] private ParticleSystem AttackPS;
    [SerializeField] private Transform Head;
    [SerializeField] private GameObject Nozzle;
    private EnemyScripableObject EnemyOBJ;
    private EnemyBehavior EB;
    private TurretBoss TB;

    [Header("SpikeTraps")]
    [SerializeField] private GameObject[] ElectricTraps;
    private int TrapCount = 0;

    [Header("Statistics")]
    public float CurrentHealth;
    private float AttackTimer;
    private float TurnSpeed;
    private Quaternion OriginalRot;

    // States
    [HideInInspector] public enum State {Initial, ChooseAttack, DirectAttack, SpinAttack, Dead}
    [HideInInspector] public State ActiveState = State.Initial;

    [Header("Bools")]
    public bool TestTraps = false;
    public bool AiTest = false;
    private bool IsAlive = true;
    private bool IsAttacking = false;
    private bool ActivateTraps = false;
    private bool TrapsActivated;

    [Header("Boss Door")]
    [SerializeField] private Transform Entrance;
    [SerializeField] private Transform Exit;

    private IEnumerator Start()
    {
        while (IsAlive)
        {
            switch (ActiveState)
            {
                case State.Initial:
                    DoInitial();
                    break;
                case State.ChooseAttack:
                    DoChooseAttack();
                    break;
                case State.DirectAttack:
                    DoDirectAttack();
                    break;
                case State.SpinAttack:
                    DoSpinAttack();
                    break;
                case State.Dead:
                    DoDead();
                    break;
            }
            yield return null;
        }
    }

    private void DoInitial()
    {
        if (!AiTest)
        {
            Player = PlayerRefs.instance.Player;
        }

        EB = GetComponent<EnemyBehavior>();
        EnemyOBJ = EB.EnemyOBJ;

        AttackTimer = EnemyOBJ.AttackRate;
        TurnSpeed = EnemyOBJ.MovementSpeed;

        OriginalRot = transform.rotation;

        TB = this;
    }

    private void DoChooseAttack()
    {
        int AttackChoice = Random.Range(0, 2);

        if(AttackChoice == 0)
        {
            ActiveState = State.DirectAttack;
            StartCoroutine(SetTimer(AttackTimer));
        }
        else if(AttackChoice == 1)
        {
            ActiveState = State.SpinAttack;
            StartCoroutine(SetTimer(AttackTimer));
        }
    }

    private void DoDirectAttack()
    {
        if(ActiveState != State.Dead)
        {
            Vector3 RelativePos = Player.position - transform.position;
            Quaternion Rotation = Quaternion.LookRotation(RelativePos, Vector3.up);

            Head.rotation = Quaternion.Slerp(Head.rotation, Rotation, TurnSpeed * Time.deltaTime);

            AttackPS.Play();

            if (IsAttacking == false)
            {
                AttackPS.Stop();
                ActiveState = State.ChooseAttack;
            }
        }
    }

    private void DoSpinAttack()
    {
        if(ActiveState != State.Dead)
        {
            AttackPS.Play();

            Head.Rotate(Vector3.up * (TurnSpeed * 40) * Time.deltaTime);

            if (IsAttacking == false)
            {
                AttackPS.Stop();
                ActiveState = State.ChooseAttack;
            }
        }
    }

    private void DoDead()
    {
        AttackPS.Stop();

        foreach (GameObject Trap in ElectricTraps)
        {
            Trap.GetComponent<ElectricTrap>().DestroyTrap = true;
        }

        AttackPS.transform.parent = null;
        Destroy(Nozzle);

        TB.enabled = false;
    }

    private IEnumerator SetTimer(float Duration)
    {
        float t = 0;
        IsAttacking = true;

        while(t < Duration)
        {
            t += Time.deltaTime;
            yield return null;
        }

        IsAttacking = false;
    }

    private void Update()
    {
        CurrentHealth = EB.CurrentHealth;

        if (CurrentHealth <= (CurrentHealth * .5) && !ActivateTraps)
        {
            TrapsActivated = true;

            foreach (GameObject Trap in ElectricTraps)
            {
                Trap.GetComponent<ElectricTrap>().ActivateTrap();
            }
        }

        if (TestTraps && !TrapsActivated)
        {
            TrapsActivated = true;

            foreach (GameObject Trap in ElectricTraps)
            {
                Trap.GetComponent<ElectricTrap>().ActivateTrap();
            }
        }

        if(CurrentHealth <= 0)
        {
            ActiveState = State.Dead;
        }

        if (EB.ActivateBoss)
        {
            EB.ActivateBoss = false;
            ActiveState = State.ChooseAttack;
        }
    }
}