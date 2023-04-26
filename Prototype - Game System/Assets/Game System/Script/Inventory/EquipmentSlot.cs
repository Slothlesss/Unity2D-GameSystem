using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSlot : MonoBehaviour
{
    public ItemManager.ItemType type;
    public GameObject GreenFrame;
    public bool isEquip;
    public void ShowCannotEquip()
    {
        GreenFrame.SetActive(false);
    }

    public void ShowCanEquip()
    {
        GreenFrame.SetActive(true);
    }
}
