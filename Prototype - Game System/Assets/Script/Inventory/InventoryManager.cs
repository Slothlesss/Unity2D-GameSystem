using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public Transform Tabs;
    public GridLayoutGroup Grid;
    public GameObject ItemPrefab;
    public GameObject CellPrefab;
    public bool AddEmptyCells = true;

    private Toggle ActiveTab => Tabs.GetComponentsInChildren<Toggle>().Single(i => i.isOn);
    private void Awake()
    {
        for(int i = 0; i < 100; i++)
        {
            Instantiate(CellPrefab, Grid.transform);
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
}
