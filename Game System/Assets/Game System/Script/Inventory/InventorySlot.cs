using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    public bool isEmpty;
    public bool isLocked;
    public GameObject lockObject;
    public Toggle Toggle;
    public TextMeshProUGUI countText;
    private void Start()
    {
        countText.gameObject.SetActive(false);
        if (!isLocked)
        {
            lockObject.SetActive(false);
        }
        else
        {
            lockObject.SetActive(true);
        } 
        
        if (Toggle)
        {
            Toggle.group = GetComponentInParent<ToggleGroup>();
        }
    }

    public void DisplayCountText(bool bl)
    {
        countText.gameObject.SetActive(bl);
    }

    public InventoryItem GetItemType()
    {
        return GetComponentInChildren<InventoryItem>();
    }

    public void DestroyItem()
    {
        countText.gameObject.SetActive(false);
        Destroy(GetItemType().gameObject);
        isEmpty = true;
    }
}
