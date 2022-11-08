using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public Transform Tabs;
    public GridLayoutGroup Grid;
    public Transform customCursor;
    public GameObject SlotLocked;
    public GameObject SlotFree;
    public GameObject ItemPrefab;
    public GameObject unlockNoti;
    public int inventorySize;
    public int inventoryLockedSize;
    public bool canExpand = false;
    public bool AddEmptyCells = true;

    public static InventoryManager Instance;
    public List<GameObject> emptySlots = new List<GameObject>();
    public List<GameObject> lockSlots = new List<GameObject>();
    public List<EquipmentSlot> equipmentSlots = new List<EquipmentSlot>();
    public Dictionary<string, int> inventoryItems = new Dictionary<string, int>();

    
    bool isCarryingItem = false;
    Toggle ActiveSlot;

    private void Awake()
    {
        Instance = this;
        for(int i = 0; i < inventorySize; i++)
        {
            emptySlots.Add(Instantiate(SlotFree, Grid.transform));
            
        }
        for (int i = 0; i < inventoryLockedSize; i++)
        {
            lockSlots.Add(Instantiate(SlotLocked, Grid.transform));

        }
    }
    
    private void Start()
    {
        OnSelectTab(true);
    }

    private void Update()
    {
        if (customCursor.gameObject.activeInHierarchy)
        {
            ActiveSlot = Grid.GetComponentsInChildren<Toggle>().Single(i => i.isOn);
            TakeItemFromInventory();
            if (isCarryingItem)
            {
                EquipItem();
            }
        }
    }

    void TakeItemFromInventory()
    {
        
        if (Input.GetMouseButtonDown(0) && Vector3.Distance(customCursor.position, ActiveSlot.transform.position) < 50f)
        {
            if (!isCarryingItem)
            {
                InventorySlot getSlot = ActiveSlot.GetComponent<InventorySlot>();
                if (!getSlot.isEmpty && !getSlot.ItemType().item.countable)
                {
                    InventoryItem getItem = getSlot.ItemType();
                    getItem.transform.SetParent(customCursor);
                    getItem.transform.localPosition = Vector3.zero;
                    isCarryingItem = true;
                }
            }
            
        }
    }

    void EquipItem()
    {

        EquipmentSlot equipmentSlot = null;
        float minDistance = 50f; //Only nearby
        foreach (EquipmentSlot slot in equipmentSlots)
        {
            float dist = Vector2.Distance(customCursor.position, slot.transform.position);
            Debug.Log(equipmentSlot);
            if (dist < minDistance && slot.typeOfSlot.ToString() == customCursor.GetComponentInChildren<InventoryItem>().item.typeOfItem.ToString()) //&& //)
            {
                equipmentSlot = slot;
                slot.ShowCanEquip();
            }
            else
            {
                slot.ShowCannotEquip();
            }
        }
        InventoryItem getItem = customCursor.GetComponentInChildren<InventoryItem>();
        //if (Input.GetMouseButtonDown(0) && equipmentSlot) //Fixing
        //{
        if (Input.GetMouseButtonUp(0))
        {
            if(!equipmentSlot) //Put item back to inventory
            {
                getItem.transform.SetParent(ActiveSlot.transform);
                getItem.transform.localPosition = Vector3.zero;
                isCarryingItem = false;
            }
            else if (equipmentSlot.isEquip) //SwapItem
            {
                InventoryItem itemEquip = equipmentSlot.GetComponentInChildren<InventoryItem>();
                itemEquip.transform.SetParent(ActiveSlot.transform);
                itemEquip.transform.localPosition = Vector3.zero;
                getItem.transform.SetParent(equipmentSlot.transform);
                getItem.transform.localPosition = Vector3.zero;
                equipmentSlot.ShowCannotEquip();
                isCarryingItem = false;
            }
            else //Equip Item
            {
                getItem.transform.SetParent(equipmentSlot.transform);
                getItem.transform.localPosition = Vector3.zero;
                equipmentSlot.isEquip = true;
                ActiveSlot.GetComponent<InventorySlot>().isEmpty = true;
                equipmentSlot.ShowCannotEquip();
                isCarryingItem = false;
            }
        }
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

    public void AddItem(ItemInfo item)
    {
        Debug.Log("long");
        foreach (GameObject emptySlot in emptySlots)
        {
            InventorySlot slot = emptySlot.GetComponent<InventorySlot>();
            if (slot.isEmpty)
            {
                GameObject itemAdd = Instantiate(ItemPrefab, transform.position, Quaternion.identity);
                itemAdd.GetComponent<InventoryItem>().item = item;
                itemAdd.transform.SetParent(emptySlot.transform);
                itemAdd.transform.localPosition = new Vector3(0, 0, 0);
                slot.isEmpty = false;
                if (item.countable)
                {
                    inventoryItems.Add(item.itemName, 1);
                    slot.countText.text = inventoryItems[item.itemName].ToString();
                }
                break;
            }
            else if (item.countable)
            {
                if (slot.ItemType().item.itemName  == item.itemName)
                {
                    inventoryItems[item.itemName]++;
                    slot.countText.text = inventoryItems[item.itemName].ToString();
                    break;
                }
            }
        }
    }

    public void DeleteItem()
    {
        InventorySlot activeSlot = ActiveSlot.GetComponent<InventorySlot>();
        if (!activeSlot.isEmpty)
        {
            inventoryItems.Remove(activeSlot.ItemType().item.itemName);
            activeSlot.DestroyItem();
        }
    }

    public void ArrangeItem()
    {

    }
        

    


}
