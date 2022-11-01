using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : ItemBase
{
    public override void Start()
    {
        base.Start();
        if (item.countable)
        {
            InventorySlot slot = GetComponentInParent<InventorySlot>();
            if (slot)
            {
                slot.CountText(true);
            }
        }
    }

}
