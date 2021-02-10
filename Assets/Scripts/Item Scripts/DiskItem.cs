﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiskItem : ItemBase
{
    public override void ActivateObject(GameObject source)
    {
        VisualManager.instace.DiskVisual.SetActive(true);
    }

    public override void DeActivateObject(GameObject source)
    {
        VisualManager.instace.DiskVisual.SetActive(false);
    }
}
