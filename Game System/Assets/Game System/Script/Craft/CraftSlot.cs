using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CraftSlot : MonoBehaviour
{
    public GameObject lockIcon;
    public TextMeshProUGUI amountText;
    public InventoryItem item;
    public void DisplayLock()
    {
        item.gameObject.SetActive(false);
        amountText.gameObject.SetActive(false);
        lockIcon.SetActive(true);
    }

    public void ShowRequirement(int currentAmount, int requireAmount, ItemInfo info)
    {
        item.gameObject.SetActive(true);
        lockIcon.SetActive(false);
        item.data.info = info;

        amountText.text = currentAmount.ToString() + "/" + requireAmount.ToString();
        if (currentAmount < requireAmount)
        {
            amountText.color = Color.red;
        }
        else
        {
            amountText.color = Color.white;
        }
    }
}
