using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;
public class UpgradeManager : MonoBehaviour
{
    public InventoryItem[] upgradeItems;
    public EquipmentSlot[] equipmentSlots;
    public GameObject upgradeInventory;

    [Header("UpgradeFunction")]

    public InventoryItem currentEnhancedItem;
    public TextMeshProUGUI currentStats;

    public InventoryItem afterEnhancedItem;
    public TextMeshProUGUI afterStats;

    [Header("TransferFunction")]
    public InventoryItem transferSlot1; 
    public TextMeshProUGUI slot1Stats;
    public InventoryItem transferSlot2;
    public TextMeshProUGUI slot2Stats;

    [Header("ViewController")]
    public GameObject TogglePool;
    public GameObject ViewPool;
    Dictionary<string, UIUpgradeView> viewsDictionary = new Dictionary<string, UIUpgradeView>(); //Upgrade, Transfer
                                                                                                //Must use UIUpgradeView bc if using UISystemView => SystemPool will get it.


    Toggle ActiveFunction;
    private void Awake()
    {
        //Set inventory
        upgradeItems = upgradeInventory.GetComponentsInChildren<InventoryItem>();
        equipmentSlots = upgradeInventory.GetComponentsInChildren<EquipmentSlot>();
        //SetAcitve all item in upgradetems to False after GetComponent, because if you SetActive(false) before running, GetComponent does not work.
        foreach (InventoryItem item in upgradeItems)
        {
            item.gameObject.SetActive(false);
        }

        //Set UIView
    }

    private void Start()
    {
        if (TogglePool.activeInHierarchy)
        {

            UIUpgradeView[] systemViews = ViewPool.GetComponentsInChildren<UIUpgradeView>();
            //SetAcitve all views to False after GetComponent, because if you SetActive(false) before running, GetComponent does not work.
            foreach (UIUpgradeView view in systemViews)
            {
                viewsDictionary.Add(view.name, view);
                view.gameObject.SetActive(false);
            }
            ActiveFunction = TogglePool.GetComponentsInChildren<Toggle>().Single(i => i.isOn);
        }
    }



    private void Update()
    {
        if(TogglePool.activeInHierarchy)
        {
            ActiveFunction = TogglePool.GetComponentsInChildren<Toggle>().Single(i => i.isOn);

            viewsDictionary[ActiveFunction.name].gameObject.SetActive(true);
        }
        else
        {
            ReleaseItemInView();
        }

        for (int i = 0; i < 6; i++)
        {
            InventoryItem equipItem = null;
            if (InventoryManager.Instance.equipmentSlots[i].GetComponentInChildren<InventoryItem>() != null)
            {
                equipItem = InventoryManager.Instance.equipmentSlots[i].GetComponentInChildren<InventoryItem>();
                Debug.Log(InventoryManager.Instance.equipmentSlots[i].GetComponentInChildren<InventoryItem>());
            }
            else
            {
                upgradeItems[i].data.info = null;
                upgradeItems[i].gameObject.SetActive(false);
                Debug.Log(InventoryManager.Instance.equipmentSlots[i].GetComponentInChildren<InventoryItem>());
            }    
            InventoryItem upgradeItem = upgradeItems[i];

            //------Display equip item in upgrade inventory
            if (equipItem && equipItem.data.info.stat.type == equipmentSlots[i].type) 
            {
                upgradeItem.gameObject.SetActive(true);
                upgradeItem.data.info = equipItem.data.info;

            }
            if (ActiveFunction.name == "UpgradeFunction")
            {

                if (currentEnhancedItem.gameObject.activeInHierarchy && currentEnhancedItem.data.info == upgradeItem.data.info) //update level for item in equip inventory
                {
                    upgradeItem.Level = currentEnhancedItem.Level;
                    equipItem.Level = currentEnhancedItem.Level;
                }
            }
            else if(ActiveFunction.name == "TransferFunction")
            {
                if(transferSlot1.gameObject.activeInHierarchy && transferSlot1.data.info == upgradeItem.data.info)
                {
                    upgradeItem.Level = transferSlot1.Level;
                    equipItem.Level = transferSlot1.Level;
                }
                if (transferSlot2.gameObject.activeInHierarchy && transferSlot2.data.info == upgradeItem.data.info)
                {
                    upgradeItem.Level = transferSlot2.Level;
                    equipItem.Level = transferSlot2.Level;
                }
            }
        }
    }
    public void OnSelectItem(InventoryItem item)
    {
        if(item.gameObject.activeInHierarchy && ActiveFunction.name == "UpgradeFunction")
        {
            currentEnhancedItem.gameObject.SetActive(true);
            currentEnhancedItem.data.info = item.data.info;
            currentEnhancedItem.Level = item.Level;
            afterEnhancedItem.gameObject.SetActive(true);
            afterEnhancedItem.data.info = item.data.info;
            afterEnhancedItem.Level = item.Level + 1;
            currentStats.gameObject.SetActive(true);
            afterStats.gameObject.SetActive(true);
            currentStats.text = StatsText(item) + item.Stats.ToString();
            afterStats.text = StatsText(item) + item.NextStats.ToString();
            Debug.Log(1);
        }
        else if(ActiveFunction.name == "TransferFunction" && !transferSlot1.gameObject.activeInHierarchy)
        {
            if(transferSlot2.gameObject.activeInHierarchy && item.data.info == transferSlot2.data.info) //Deselect Slot 2
            {
                transferSlot2.gameObject.SetActive(false);
                slot2Stats.gameObject.SetActive(false);
            }
            else
            {
                transferSlot1.gameObject.SetActive(true);
                transferSlot1.data.info = item.data.info;
                transferSlot1.Level = item.Level;
                slot1Stats.gameObject.SetActive(true);
                slot1Stats.text = StatsText(item) + item.Stats.ToString();
            }

        }
        else if(ActiveFunction.name == "TransferFunction" )
        {
            if (item.data.info != transferSlot1.data.info && !transferSlot2.gameObject.activeInHierarchy) 
            {
                transferSlot2.gameObject.SetActive(true);
                transferSlot2.data.info = item.data.info;
                transferSlot2.Level = item.Level;
                slot2Stats.gameObject.SetActive(true);
                slot2Stats.text = StatsText(item) + item.Stats.ToString();

            }
            else if(item.data.info == transferSlot1.data.info) //DeselectSlot1
            {
                transferSlot1.gameObject.SetActive(false);
                slot1Stats.gameObject.SetActive(false);
            }
            else if(transferSlot2.gameObject.activeInHierarchy)
            {
                if(item.data.info == transferSlot2.data.info) //DeselectSlot2
                {
                    transferSlot2.gameObject.SetActive(false);
                    slot2Stats.gameObject.SetActive(false);
                }
                else //swap slot 2
                {
                    transferSlot2.data.info = item.data.info;
                    transferSlot2.Level = item.Level;
                    slot2Stats.text = StatsText(item) + item.Stats.ToString();
                }
            }
            slot1Stats.text = StatsText(transferSlot1) + transferSlot1.Stats.ToString(); //reupdate Slot1 stats
        }
    }    

    public void DeselectItem()
    {
        ReleaseItemInView();
    }

    public void ReleaseItemInView()
    {
        if (ActiveFunction.name == "UpgradeFunction")
        {
            currentEnhancedItem.gameObject.SetActive(false);
            afterEnhancedItem.gameObject.SetActive(false);
            currentStats.gameObject.SetActive(false);
            afterStats.gameObject.SetActive(false);
        }
        else if (ActiveFunction.name == "TransferFunction")
        {
            transferSlot1.gameObject.SetActive(false);
            transferSlot2.gameObject.SetActive(false);
            slot1Stats.gameObject.SetActive(false);
            slot2Stats.gameObject.SetActive(false);
        }
    }

    string StatsText(InventoryItem item)
    {
        if (item.data.info.stat.type.ToString() == "Weapon" ||
            item.data.info.stat.type.ToString() == "Ring" ||
            item.data.info.stat.type.ToString() == "Necklace")
        {
            return "Attack: ";
        }
        if (item.data.info.stat.type.ToString() == "Helmet" ||
            item.data.info.stat.type.ToString() == "Armor" ||
            item.data.info.stat.type.ToString() == "Shoes")
        {
            return "Defense: ";
        }
        return "";
    }

    public void Upgrade()
    {
        currentEnhancedItem.Level++;
        afterEnhancedItem.Level++;
        currentStats.text = StatsText(currentEnhancedItem) + currentEnhancedItem.Stats.ToString();
        afterStats.text = StatsText(afterEnhancedItem) + currentEnhancedItem.NextStats.ToString();
    }

    public void Transfer()
    {
        if(transferSlot1.gameObject.activeInHierarchy && transferSlot2.gameObject.activeInHierarchy)
        {
            //Swap Level
            int level;
            level = transferSlot1.Level;
            transferSlot1.Level = transferSlot2.Level;
            transferSlot2.Level = level;
            //Display after swap
            slot1Stats.text = StatsText(transferSlot1) + transferSlot1.Stats.ToString();
            slot2Stats.text = StatsText(transferSlot2) + transferSlot2.Stats.ToString();
        }
    }
                   
    public void DisplayView()
    {
        ReleaseItemInView();
        foreach (KeyValuePair<string, UIUpgradeView> view in viewsDictionary)
        {
            view.Value.gameObject.SetActive(false);
        }

    }

}
