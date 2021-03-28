﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    [Header("Weapon and Owner")]
    [SerializeField] Transform Owner;
    [SerializeField] private MeleeScriptableObject MeleeOBJ;

    [Header("Player")]
    [SerializeField] private Transform Player;

    [Header("Which character controller?")]
    [SerializeField] private bool HasCharacterController;
    [SerializeField] private bool HasRigibody;

    private void Start()
    {
        //Player = PlayerRefs.instance.Player;
        Player = PlayerRefs.instance.Player.transform;
        Owner = gameObject.transform;
        GetComponent<Collider>().isTrigger = true;
        GetComponent<Collider>().enabled = false;
        Physics.IgnoreCollision(GetComponent<Collider>(), Owner.GetComponent<Collider>());

        if (Player.GetComponent<CharacterController>() != null)
        {
            HasCharacterController = true;
            HasRigibody = false;
        }
        else if (Player.GetComponent<Rigidbody>() != null)
        {
            HasRigibody = true;
            HasCharacterController = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            other.GetComponent<Rigidbody>().AddForce(Owner.transform.forward * MeleeOBJ.KnockbackPower, ForceMode.Impulse);
            other.GetComponent<EnemyBehavior>().TakeDamage(MeleeOBJ.DamageDealt);
        }
        else if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<Player>().TakeDamage(MeleeOBJ.DamageDealt);

            if (HasCharacterController)
            {
                other.GetComponent<CharacterController>().Move(Owner.forward * MeleeOBJ.KnockbackPower);
            }
            else if (HasRigibody)
            {
                other.GetComponent<Rigidbody>().AddForce(Owner.forward * MeleeOBJ.KnockbackPower, ForceMode.VelocityChange);
            }
        }
    }
}