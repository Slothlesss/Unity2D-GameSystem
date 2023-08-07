using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftManager : MonoBehaviour
{
    public GameObject craftMenuPool;
    public CraftRequirement[] craftMenus;
    public int currentMenu;
    public Button craftButton;


    private void Awake()
    {
        craftButton.onClick.AddListener(() =>
        {
            CraftItem();
        });
    }

    public void Start()
    {
        craftMenus = craftMenuPool.GetComponentsInChildren<CraftRequirement>();
        DisplayMenu(0);
        currentMenu = 0;
    }
    public void DisplayMenu(int idx)
    {
        foreach (CraftRequirement craft in craftMenus)
        {
            craft.gameObject.SetActive(false);
        }
        craftMenus[idx].gameObject.SetActive(true);
        currentMenu = idx;
    }

    public void CraftItem()
    {
        if (craftMenus[currentMenu].CanCraft())
        {
            craftMenus[currentMenu].Craft();
            craftMenus[currentMenu].UpdateCraftSlot();
        }
    }

}
