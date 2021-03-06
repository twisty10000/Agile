﻿//using HutongGames.PlayMaker.Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

[System.Serializable]
public class Scriptforui : MonoBehaviour
{
    public static Scriptforui instance;
    private Player player;
    public Text displayPlayerHealth;
    public float playerHealth;
    public int totalDashAmount;
    public int currentDashAmount;
    public Text displayTotalDashAmount;
    public UnityEngine.UI.Slider playerHealthSlider;
    public UnityEngine.UI.Image dashRecharge;
    public UnityEngine.UI.Slider shieldRechargeSlider;
    public float shieldRecharge;
    public UnityEngine.UI.Image shieldImage;
    public UnityEngine.UI.Text whipTutorial;
    public UnityEngine.UI.Text currentChapter;
    public UnityEngine.UI.Text currentObjective;


    //Player Items Holder
    public UnityEngine.UI.Image currentItem;
    public UnityEngine.UI.Image firstItem;
    public UnityEngine.UI.Image secondItem;
    public UnityEngine.UI.Image thirdItem;
    public UnityEngine.UI.Image dockedItem1;
    public UnityEngine.UI.Image dockedItem2;

    
    //Player Items
    public UnityEngine.UI.Image playerBaton;
    public UnityEngine.UI.Image playerDisc;
    public UnityEngine.UI.Image playerWhip;
    public UnityEngine.UI.Image playerDash;

    public bool itemInSlot1;
    public bool itemInSlot2;
    public bool itemInSlot3;
    public bool hasDash;
    private ItemPickUp itemPickUp;

    //Sprites
    public Sprite spritePlayerDisc;
    public Sprite spritePlayerWhip;
    public Sprite spritePlayerBat;
    public UnityEngine.UI.Text item1Text;
    public UnityEngine.UI.Text item2Text;
    public UnityEngine.UI.Text currentItemText;

    public ParticleShield uiParticleShield;
    public bool hasShield;
    public int currentObjectiveNumber;

    //public PlayerRefs pRefs;

    private Transform Shield;
    private void Awake()
    {
        if (Scriptforui.instance == null)
        {
            Scriptforui.instance = this;
        }
        else if(Scriptforui.instance != this)
        {
            Destroy(this);
        }
    }
    void Start()
    {
       
        currentDashAmount = 0;
        totalDashAmount = 3;
        playerHealth = PlayerRefs.instance.currentHealth;

        //shieldRecharge = 100;
        Shield = PlayerRefs.instance.Sheild;
        shieldRecharge = Shield.GetComponent<ParticleShield>().CurrentCapacity;
        shieldRechargeSlider.value = shieldRecharge;
        playerHealthSlider.value = playerHealth;
        displayPlayerHealth.text = playerHealth.ToString();
        displayTotalDashAmount.text = currentDashAmount.ToString();

        itemInSlot1 = false;
        itemInSlot2 = false;
        itemInSlot3 = false;
       
        
        itemPickUp = ItemPickUp.FindObjectOfType<ItemPickUp>();

        uiParticleShield = ParticleShield.FindObjectOfType<ParticleShield>();

        //pRefs = PlayerRefs.FindObjectOfType<PlayerRefs>();

        hasShield = false;
        currentObjectiveNumber = 0;
  

    }

    // Update is called once per frame
    void Update()
    {
        playerHealth = playerHealthSlider.value;
        displayPlayerHealth.text = playerHealth.ToString();
        if (hasDash == true)
        {
            if (currentDashAmount < totalDashAmount)
            {
                dashRecharge.fillAmount = (dashRecharge.fillAmount + .004f);

                if (dashRecharge.fillAmount == 1)
                {
                    currentDashAmount = currentDashAmount + 1;
                    displayTotalDashAmount.text = currentDashAmount.ToString();
                    dashRecharge.fillAmount = 0;
                }
            }
        }

        playerPressedOne();
        pullOutWeapons();


       if(hasShield == true)
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                shieldRechargeSlider.value = (shieldRechargeSlider.value - Shield.GetComponent<ParticleShield>().DrainSpeed * Time.deltaTime);
            }
            else
            {
                shieldRechargeSlider.value = (shieldRechargeSlider.value + Shield.GetComponent<ParticleShield>().RechargeSpeed * Time.deltaTime);
            }
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            PlayerRefs.instance.Player.GetComponent<Player>().TakeDamage(20);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            PlayerRefs.instance.Player.GetComponent<Player>().HealPlayer(20);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
           
        }

        if (Input.GetMouseButtonDown(0))
        {
            whipTutorial.gameObject.SetActive(false);
        }



        //OBJECTIVE MARKERS
        if(currentObjectiveNumber == 2)
        {
            currentChapter.text = "Chapter 2: Rage";
            currentObjective.text = "Battle through District 1";
        }

        else if(currentObjectiveNumber == 4)
        {
            currentChapter.text = "Chapter 3: Respite";
            currentObjective.text = "Navigate the tunnels to the factory";
        }

        else if (currentObjectiveNumber == 6)
        {
            currentChapter.text = "Chapter 4: Pursuit";
            currentObjective.text = "Take the mine cart to Rich Town";
        }

        else if (currentObjectiveNumber == 8)
        {
            currentChapter.text = "Chapter 5: Justice";
            currentObjective.text = "Find the betrayer";
        }

        else if (currentObjectiveNumber == 10)
        {
            currentChapter.text = "Chapter 6";
            currentObjective.text = "Obejective 6";
        }

    }

    private void playerPressedOne()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //firstItem.rectTransform.sizeDelta = new Vector2(100, 100);
        }
    }
    private void pullOutWeapons()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //Swapping Weapons
            firstItem.gameObject.SetActive(true);
            secondItem.gameObject.SetActive(false);
            thirdItem.gameObject.SetActive(false);
           

            //Docking Items
            dockedItem1.sprite = spritePlayerDisc;
            dockedItem2.sprite = spritePlayerWhip;
            item1Text.text = "2";
            item2Text.text = "3";
            currentItemText.text = "1";
            dockedItem1.color = Color.cyan;//new Color(0, 144, 229, 255);
            dockedItem2.color = Color.yellow;//new Color(161, 0, 231, 255);
          



        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            //Swapping Weapons
            firstItem.gameObject.SetActive(false);
            secondItem.gameObject.SetActive(true);
            thirdItem.gameObject.SetActive(false);
          

            //Docking Items
            dockedItem1.sprite = spritePlayerBat;
            dockedItem2.sprite = spritePlayerWhip;
            item1Text.text = "1";
            item2Text.text = "3";
            currentItemText.text = "2";
            dockedItem1.color = Color.red;//new Color(0, 144, 229, 255);
            dockedItem2.color = Color.yellow;//new Color(161, 0, 231, 255);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            //Swapping Weapons
            firstItem.gameObject.SetActive(false);
            secondItem.gameObject.SetActive(false);
            thirdItem.gameObject.SetActive(true);
        

            //Docking Items
            dockedItem1.sprite = spritePlayerBat;
            dockedItem2.sprite = spritePlayerDisc;
            item1Text.text = "1";
            item2Text.text = "2";
            currentItemText.text = "3";
            dockedItem1.color = Color.red;//new Color(0, 144, 229, 255);
            dockedItem2.color = Color.cyan;//new Color(161, 0, 231, 255);
        }



    }




}
