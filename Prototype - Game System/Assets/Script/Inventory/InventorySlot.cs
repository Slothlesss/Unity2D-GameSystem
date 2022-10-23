using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public bool isEmpty;
    public bool isLocked;
    public GameObject lockObject;
    public Toggle Toggle;
    private void Start()
    {
        if(!isLocked)
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

}
