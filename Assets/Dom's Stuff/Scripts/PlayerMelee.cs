﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMelee : MonoBehaviour
{
    [Header("Components")]
    public Animator animator;

    [Header("Weapon")]
    [SerializeField] private Collider WeaponCollider = null;

    private bool CanAttack = true;

    private void Start()
    {
        WeaponCollider.enabled = false;

        //animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (CanAttack)
        {
            if (Input.GetMouseButtonDown(0))
            {
                animator.SetTrigger("Attack");
            }
        }
    }

    private void StartAttack()
    {
        CanAttack = false;
        WeaponCollider.enabled = true;
    }

    private void StopAttack()
    {
        CanAttack = true;
        WeaponCollider.enabled = false;
    }
}
