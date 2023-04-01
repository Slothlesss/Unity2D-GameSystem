using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    [Header("General")]
    public GameObject inventoryRect;
    public Camera mainCamera;
    
    [Header("Data")]
    public Data InventoryData;

    [Header("Inventory")]
    public Transform Tabs;
    public GridLayoutGroup Grid;
    public ScrollRect scrollRect;
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

    [Header("Slots")]
    public List<GameObject> emptySlots = new List<GameObject>();
    public List<GameObject> lockSlots = new List<GameObject>();
    public List<EquipmentSlot> equipmentSlots = new List<EquipmentSlot>();
    public Dictionary<string, int> inventoryItems = new Dictionary<string, int>(); //name, amount
    


    
    bool isCarryingItem = false;
    Toggle ActiveSlot;


    [Header("Item View")]
    public GameObject itemViewPanel;
    public TextMeshProUGUI itemNameUI;
    public Image itemBackgroundUI;
    public Image itemFrameUI;
    public Image itemImageUI;
    public TextMeshProUGUI itemLevel;
    public TextMeshProUGUI itemRarity;
    public TextMeshProUGUI itemSpecialStat;
    public TextMeshProUGUI itemDescription;

    public GameObject verticalSlash;
    public GameObject statGroup;
    public GameObject inventoryField;

    
    private void Awake()
    {
        Instance = this;
        SpawnSlots();
        SpawnItemsFromData();
    }
    
    private void Start()
    {
        OnSelectTab(true);
    }

    private void Update()
    {
        if (customCursor.gameObject.activeInHierarchy) //Just check that InventorySystem is active or not => I just want to ultilize the customCursor instead of creating new parameter "GameObject inventorySystem"
        {
            //----Drag and Drop Item
            if (Grid.GetComponent<ToggleGroup>().AnyTogglesOn() == true)
            {
                ActiveSlot = Grid.GetComponent<ToggleGroup>().ActiveToggles().Single(i => i.isOn);
                ChooseItem();
                TakeItem();
                if (isCarryingItem)
                {
                    scrollRect.vertical = false; //Otherwise, your viewport will move together with item.
                    EquipItem();
                }
                else
                {
                    scrollRect.vertical = true;
                    inventoryField.SetActive(false);
                }
            }
            //----Display Item Information
            if(ActiveSlot.GetComponentInChildren<InventoryItem>() == null) //Item Information disappear
            {
                itemViewPanel.SetActive(false);
            }
            else
            {
                itemViewPanel.SetActive(true);
            }
        }
    }
    void SpawnSlots()
    {
        for (int i = 0; i < inventorySize; i++)
        {
            GameObject addSlot = Instantiate(SlotFree, Grid.transform);
            emptySlots.Add(addSlot);
            addSlot.GetComponent<Toggle>().group = GetComponentInParent<ToggleGroup>();
            if (i == 0)
            {
                addSlot.GetComponent<Toggle>().isOn = true;
            }
        }
        for (int i = 0; i < inventoryLockedSize; i++)
        {
            GameObject addSlot = Instantiate(SlotLocked, Grid.transform);
            lockSlots.Add(addSlot);
            addSlot.GetComponent<Toggle>().group = GetComponentInParent<ToggleGroup>();
        }
    }
    void SpawnItemsFromData()
    {
        if(InventoryData.itemDatas.Count != 0)
        {
            foreach(Data.ItemData data in InventoryData.itemDatas)
            {
                InitializeItemFromData(data.info);
            }
        }
    }
    void ChooseItem()
    {
        InventoryItem currentItem = ActiveSlot.GetComponentInChildren<InventoryItem>();
        DisplayStat(currentItem);
    }
    void DisplayStat(InventoryItem currentItem)
    {
        if (currentItem != null)
        {   //---Check special condition
            //----1. Check specialStat
            if (currentItem.item.specialStat == "")
            {
                itemSpecialStat.gameObject.SetActive(false);
            }
            else
            {
                itemSpecialStat.gameObject.SetActive(true);
            }
            //----2. Check type of item => if (item == Potion, Book, Scroll => only show Image, no level, no rarity) || or you can set your own type
            if (currentItem.item.typeOfItem == ItemManager.ItemType.Potion
                || currentItem.item.typeOfItem == ItemManager.ItemType.Book
                || currentItem.item.typeOfItem == ItemManager.ItemType.Scroll)
            {
                verticalSlash.SetActive(false);
                statGroup.SetActive(false);
            }
            else
            {
                verticalSlash.SetActive(true);
                statGroup.SetActive(true);
            }

            //---Then, set main features
            itemNameUI.text = currentItem.item.itemName;
            itemBackgroundUI.sprite = currentItem.backGround.sprite;
            itemFrameUI.sprite = currentItem.frame.sprite;
            itemImageUI.sprite = currentItem.img.sprite;
            if (statGroup.activeInHierarchy)
            {
                itemLevel.text = "Level " + currentItem.item.itemLevel.ToString();
                itemRarity.text = checkRarity(currentItem);
            }
            if (itemSpecialStat.gameObject.activeInHierarchy)
            {
                itemSpecialStat.text = currentItem.item.specialStat;
            }
            itemDescription.text = currentItem.item.itemDescription;

        }
    }
    string checkRarity(InventoryItem item)
    {
        switch (item.item.rarity.ToString())
        {
            case "Legendary":
                return "<color=red>Legendary</color>";
            case "Mythical":
                return "<color=yellow>Mythical</color>";
            case "Epic":
                return "<color=purple>Epic</color>";
            case "Rare":
                return "<color=blue>Rare</color>";
            case "Uncommon":
                return "<color=green>Uncommon</color>";
            case "Common":
                return "<color=white>Common</color>";
        }
        return "";
    }
    void TakeItem()
    {
        
        if (Input.GetMouseButtonDown(0) && Vector3.Distance(customCursor.position, ActiveSlot.transform.position) < 50f)
        {
            if (!isCarryingItem)
            {
                InventoryItem item = ActiveSlot.GetComponentInChildren<InventoryItem>();
                if (item != null && !item.item.countable)
                {
                    item.transform.SetParent(customCursor);
                    item.transform.localPosition = Vector3.zero;
                    isCarryingItem = true;
                }
            }
            
        }
    }
    void EquipItem()
    {
        // Step 1: Check Slot that has similar types with carrying item => Ex: Sword fits Weapon slot
        EquipmentSlot equipmentSlot = null;
        float minDistance = 50f; //Only nearby
        EquipmentSlot equipableSlot = equipmentSlots.Single(i => i.typeOfSlot == customCursor.GetComponentInChildren<InventoryItem>().item.typeOfItem);
        float dist = Vector2.Distance(customCursor.position, equipableSlot.transform.position);
        if (dist < minDistance)
        { 
            equipmentSlot = equipableSlot;
            equipmentSlot.ShowCanEquip();
        }

        //Display inventoryField
        if (ActiveSlot.GetComponentInChildren<EquipmentSlot>() != null) //if item is taken from equipment slot
        {
            if (is_rectTransformsOverlap(customCursor.GetComponent<RectTransform>(), inventoryRect.GetComponent<RectTransform>()))
            {
                inventoryField.SetActive(true);
            }
            else
            {
                inventoryField.SetActive(false);
            }
        }
        else //Display equiptableField => I will update this one in the future.
             //Intially, I want it nearly the slot, but after researching, I think it's quite hard to equip item in the right slot.
             //Therefore, using the same way when unequip item I think it would be better <3
        {
            //Display and Undisplay the equipable field
        }

        //Step 2: Put Item into Slot

        InventoryItem getItem = customCursor.GetComponentInChildren<InventoryItem>();
        if (Input.GetMouseButtonUp(0))
        {
            //Case 1: Item taken from Inventory Slot
            if (ActiveSlot.GetComponentInChildren<InventorySlot>() != null)
            {
                if (!equipmentSlot) // Put item back to inventory
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
            //Case 2: Item taken from Equipment Slot 
            else
            {
                if(is_rectTransformsOverlap(customCursor.GetComponent<RectTransform>(), inventoryRect.GetComponent<RectTransform>()))
                {

                    //-----Step 1: Put item into nearest Slot that is Empty
                    foreach (GameObject emptySlot in emptySlots)
                    {
                        InventorySlot slot = emptySlot.GetComponent<InventorySlot>();
                        if (slot.isEmpty)
                        {
                            getItem.transform.SetParent(slot.transform);
                            getItem.transform.localPosition = Vector3.zero;
                            isCarryingItem = false;
                            slot.isEmpty = false;
                            equipableSlot.isEquip = false; //We cannot use "equipmentSlot" because it becomes null 
                            equipableSlot.ShowCannotEquip();
                            break;
                        }
                    }
                    
                    //-----Step 2: Rearrange the inventory => The item will go to the last Empty Slot then we rearrange the Inventory based on Type => Rarity => Name
                    //// Later Update
                }
                else
                {
                    //----The Item will go back to the Equipment Slot
                    getItem.transform.SetParent(equipableSlot.transform);
                    getItem.transform.localPosition = Vector3.zero;
                    isCarryingItem = false;
                    equipableSlot.isEquip = true; //We cannot use "equipmentSlot" because it becomes null 
                    equipableSlot.ShowCannotEquip();
                }

            }
        }
    }
    public bool is_rectTransformsOverlap(RectTransform elem, RectTransform viewport)
    {
        Vector2 viewportMinCorner;
        Vector2 viewportMaxCorner;


        Vector3[] v_wcorners = new Vector3[4];
        viewport.GetWorldCorners(v_wcorners); //bot left, top left, top right, bot right

        /*
            
            B************C  <--- Rect Transform => A is bot left point
            *            *                         B is top left point
            *            *                         C is top right point
            *            *                         D is bot right point
            *            *                        
            A************D 

         */

        viewportMinCorner = v_wcorners[0]; //Bot Left
        viewportMaxCorner = v_wcorners[2]; //Top Right

        // Each Rect will have 4 corners => use positions of these corners to address the problem
        
        
        /* 
            Debug.Log(v_wcorners[0].x + " " + v_wcorners[0].y); // Bot Left Point of Inventory Rect
            Debug.Log(v_wcorners[1].x + " " + v_wcorners[1].y); // Top Left Point of Inventory Rect
            Debug.Log(v_wcorners[2].x + " " + v_wcorners[2].y); // Top Right Point of Inventory Rect
            Debug.Log(v_wcorners[3].x + " " + v_wcorners[3].y); // Bot Right Point of Inventory Rect
        */

        //give 1 pixel border to avoid numeric issues:
        viewportMinCorner += Vector2.one;
        viewportMaxCorner -= Vector2.one;

        //do a similar procedure, to get the "element's" corners relative to screen:
        Vector3[] e_wcorners = new Vector3[4];
        elem.GetWorldCorners(e_wcorners);

        Vector2 elem_minCorner = e_wcorners[0]; //Bot Left 
        Vector2 elem_maxCorner = e_wcorners[2]; //Top Right
        /* 
            Debug.Log(e_wcorners[0].y); // Bot Left Point of CustomCursor Rect
            Debug.Log(e_wcorners[1].y); // Top Left Point of CustomCursor Rect
            Debug.Log(e_wcorners[2].y); // Top Right Point of CustomCursor Rect
            Debug.Log(e_wcorners[3].y); // Bot Right Point of CustomCursor Rect
        */

        //----------Perform comparison-------------------
        if (elem_minCorner.x > viewportMaxCorner.x) { return false; }//completelly outside (to the right)
        /* It gonna be like this: 
          
                             *****
        *************I1      *   *    <-- CustomCursor
        *            *      C1****
        *            *
        *            *
        **************

        Inventory 

        => C1.x > I1.x => CustomCursor is always outside to the right with any C1.y

        */

        if (elem_minCorner.y > viewportMaxCorner.y) { return false; }//completelly outside (is above)

        /* It gonna be like this: 
          
                            
                ******
                *    *   <-- CustomCursor
                C*****
         
         
        *************I      
        *            *
        *            * <-- Inventory
        *            *
        **************

        => C.y > I.y => CustomCursor is always above with any C.x

        */

        if (elem_maxCorner.x < viewportMinCorner.x) { return false; }//completelly outside (to the left)

        /* It gonna be like this: 
    
                       **************      
           ****C       *            *
           *   *       *            * <-- Inventory
           *****       *            *
             ^         I*************
             |
        CustomCursor

                 
   
        => C.x < I.x => CustomCursor is always outside (to the left) with any C.y

        */


        if (elem_maxCorner.y < viewportMinCorner.y) { return false; }//completelly outside (is below)
        /* It gonna be like this: 
         
    
             **************      
             *            *
             *            * <-- Inventory
             *            *
             I*************

         *****C
         *    * <-- CustomCursor
         ******
                 
   
        => C.y < I.y => CustomCursor is always below with any C.x

        */

        /*
            -----If you need to check if element is completely inside-----

            Vector2 minDif = viewportMinCorner - elem_minCorner;
            Vector2 maxDif = viewportMaxCorner - elem_maxCorner;
            if(minDif.x < 0  &&  minDif.y < 0  &&  maxDif.x > 0  &&maxDif.y > 0) { //return "is completely inside" }

        */

        return true;
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
        InventoryData.AddItemData(item, 1);
        foreach (GameObject emptySlot in emptySlots)
        {
            InventorySlot slot = emptySlot.GetComponent<InventorySlot>();
            if (slot.isEmpty)
            {
                GameObject itemAdd = Instantiate(ItemPrefab, transform.position, Quaternion.identity);
                itemAdd.GetComponent<InventoryItem>().item = item;
                itemAdd.transform.SetParent(emptySlot.transform);
                itemAdd.transform.SetSiblingIndex(4); //If it is the last => it will be faded
                itemAdd.transform.localPosition = new Vector3(0, 0, 0);
                slot.isEmpty = false;


                if (item.countable) //Add item to dictionary to check whether it contains nor not?
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
    public void InitializeItemFromData(ItemInfo item) //Similar to AddItem, just remove the line AddItemData. 
    {
        foreach (GameObject emptySlot in emptySlots)
        {
            InventorySlot slot = emptySlot.GetComponent<InventorySlot>();
            if (slot.isEmpty)
            {
                GameObject itemAdd = Instantiate(ItemPrefab, transform.position, Quaternion.identity);
                itemAdd.GetComponent<InventoryItem>().item = item;
                itemAdd.transform.SetParent(emptySlot.transform);
                itemAdd.transform.SetSiblingIndex(4); //If it is the last => it will be faded
                itemAdd.transform.localPosition = new Vector3(0, 0, 0);
                slot.isEmpty = false;


                if (item.countable) //Add item to dictionary to check whether it contains nor not?
                {
                    inventoryItems.Add(item.itemName, 1);
                    slot.countText.text = inventoryItems[item.itemName].ToString();
                }
                break;
            }
            else if (item.countable)
            {
                if (slot.ItemType().item.itemName == item.itemName)
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
    public void RearrangeItem()
    {

    }
}
