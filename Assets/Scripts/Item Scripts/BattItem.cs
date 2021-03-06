﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BattItem : ItemBase
{

    [SerializeField] private KeyCode SelectionKey = KeyCode.Alpha1;

    public override GameObject ReturnGO()
    {
        return VisualManager.instace.BattVisual.gameObject;
    }

    public override bool PressSelectKey(KeyCode KeyPressed)
    {
        if (KeyPressed == SelectionKey)
        {
            return true;
        }
        return false;

   
    }

    public override void ActivateObject(GameObject source)
    {
        VisualManager.instace.BattVisual.SetActive(true);
    }

    public override void DeActivateObject(GameObject source)
    {
        VisualManager.instace.BattVisual.SetActive(false);
    }

    public override void UseItem(GameObject source)
    {
        PlayerRefs.instance.Player.GetComponent<Animator>().SetTrigger("Attack");
        //VisualManager.instace.BattVisual.GetComponent<Collider>().enabled = true;
    }

}
