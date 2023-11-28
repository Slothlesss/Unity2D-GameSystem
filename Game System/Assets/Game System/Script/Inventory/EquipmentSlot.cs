using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSlot : MonoBehaviour
{
    public ItemManager.ItemType type;
    public GameObject EquipableFrame;
    public bool isEquip;
    public void ShowCannotEquip()
    {
        EquipableFrame.SetActive(false);
    }

    public void ShowCanEquip()
    {
        EquipableFrame.SetActive(true);
    }
}
