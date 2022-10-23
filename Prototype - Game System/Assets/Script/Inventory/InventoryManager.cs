using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public Transform Tabs;
    public GridLayoutGroup Grid;
    public GameObject SlotLocked;
    public GameObject SlotFree;
    public GameObject unlockNoti;
    public int inventorySize;
    public int inventoryLockedSize;
    public bool canExpand = false;
    public bool AddEmptyCells = true;
    public static InventoryManager Instance;
    List<GameObject> emptySlots = new List<GameObject>();

    private Toggle ActiveTab => Tabs.GetComponentsInChildren<Toggle>().Single(i => i.isOn);
    private void Awake()
    {
        for(int i = 0; i < inventorySize; i++)
        {
            emptySlots.Add(Instantiate(SlotFree, Grid.transform));
            
        }
        for (int i = 0; i < inventoryLockedSize; i++)
        {
            emptySlots.Add(Instantiate(SlotLocked, Grid.transform));

        }
    }
    private void Start()
    {
        OnSelectTab(true);
    }
    public void OnSelectTab(bool value)
    {

        var tab = Tabs.GetComponentsInChildren<Toggle>().Single(i => i.isOn);
        switch(tab.name)
        {
            case "Weapon":
                break;
        }    
    }
    public bool WantToExpand(bool decision)
    {
        canExpand = decision;
        return decision;
    }

}
