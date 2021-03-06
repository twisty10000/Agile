﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Plugins")]
    private PlayerRefs playerHealth;

    [Header("UI")]
    public Text HealthText;
    public Image Baton;
    public Image Disc;
    public Image Hoverboard;
    public Image GrapplingHook;
    public Image Shield;
    public Text SheildCpacity;

    private void Start()
    {
        playerHealth = PlayerRefs.instance.Player.GetComponent<PlayerRefs>();
    }

    private void Update()
    {
        HealthText.text = playerHealth.PlayerHealth.ToString();

        SheildCpacity.text = PlayerRefs.instance.Sheild.transform.gameObject.GetComponent<ParticleShield>().CurrentCapacity.ToString();
        if(PlayerRefs.instance.Sheild.transform.gameObject.GetComponent<ParticleShield>().CurrentCapacity <= 0)
        {
            Shield.color = Color.gray;
        }
        else
        {
            Shield.color = Color.blue;
        }
    }
}
