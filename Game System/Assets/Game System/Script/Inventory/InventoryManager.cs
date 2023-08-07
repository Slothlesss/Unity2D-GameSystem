using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : Singleton<InventoryManager>
{
    [Header("General")]
    public GameObject inventoryRect;
    public GameObject equipSlotRect;
    public Camera mainCamera;
    
    [Header("DataInventory")]
    public DataInventory InventoryData;

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


    [Header("Slots")]
    public List<GameObject> emptySlots = new List<GameObject>();
    public List<GameObject> lockSlots = new List<GameObject>();
    public List<EquipmentSlot> equipmentSlots = new List<EquipmentSlot>();


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
    public GameObject itemViewStatGroup;
    public UIStat[] itemStats;
    public GameObject inventoryField;
    public GameObject equipField;

    public PlayerStat playerStat;

    public Sprite[] statSprites;

    public int startIdx = 0;

    

    private void Awake()
    {
        if (DataInventory.LoadData() != null)
        {
            InventoryData = DataInventory.LoadData();
        }

        //Hide all the item stats in baseStat view
        itemStats = itemViewStatGroup.GetComponentsInChildren<UIStat>();
        foreach (UIStat stat in itemStats)
        {
            stat.gameObject.SetActive(false);
        }
        SpawnSlots();
        SpawnItemsFromData();
    }

    private void Update()
    {
        //Just check that InventorySystem is active or not => I just want to ultilize the customCursor instead of creating new parameter "GameObject inventorySystem"
        if (!customCursor.gameObject.activeInHierarchy) return;

        //Overall:
        //- If there is any slot is clicked => Display the item baseStat
        //- If users use right click to the active slot => They will take the item
        if (Grid.GetComponent<ToggleGroup>().AnyTogglesOn() == true)
        {
            ActiveSlot = Grid.GetComponent<ToggleGroup>().ActiveToggles().Single(i => i.isOn);
            DisplayItemStat();

            //If (User right click to the item AND the distance between that click and the active slot is less than 50f) 
            if (Input.GetMouseButtonDown(0) && Vector3.Distance(customCursor.position, ActiveSlot.transform.position) < 50f)
            {
                TakeItem();
            }
            if (isCarryingItem)
            {
                scrollRect.vertical = false; //Otherwise, your viewport will move together with item.
                OnDragItem();
            }
            else
            {
                inventoryField.SetActive(false);
                equipField.SetActive(false);
                scrollRect.vertical = true;
            }
        }
        //----Display Item Information
        DisplayItemInformation();

    }
    private void DisplayItemInformation()
    {
        if (ActiveSlot.GetComponentInChildren<InventoryItem>() == null) //Item Information disappear
        {
            itemViewPanel.SetActive(false);
        }
        else
        {
            itemViewPanel.SetActive(true);
        }
    }
    private void SpawnSlots()
    {
        for (int i = 0; i < inventorySize; i++)
        {
            GameObject addSlot = Instantiate(SlotFree, Grid.transform);
            addSlot.GetComponent<InventorySlot>().index = startIdx;
            startIdx++;
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
            addSlot.GetComponent<InventorySlot>().index = startIdx;
            startIdx++;
            lockSlots.Add(addSlot);
            addSlot.GetComponent<Toggle>().group = GetComponentInParent<ToggleGroup>();
        }
    }
    private void SpawnItemsFromData()
    {
        if(InventoryData.inventoryData.Count != 0)
        {
            foreach(ItemBase.ItemData data in InventoryData.inventoryData)
            {
                InitializeInventoryItemFromData(data);
            }
        }
        if(InventoryData.equipmentData.Count != 0)
        {
            foreach (ItemBase.ItemData data in InventoryData.equipmentData)
            {
                InitializeEquippedItemFromData(data);
            }
        }
    }
    private void DisplayItemStat()
    {
        InventoryItem currentItem = ActiveSlot.GetComponentInChildren<InventoryItem>();
        if (currentItem == null) return;
        //---Check special condition
        //----1. Check specialStat
        if (currentItem.data.info.prop.specialStat == "")
        {
            itemSpecialStat.gameObject.SetActive(false);
        }
        else
        {
            itemSpecialStat.gameObject.SetActive(true);
        }
        //----2. Check type of item => if (item == Potion, Book, Scroll, Material => only show Image, no level, no rarity)
        // or you can set your own type
        if (currentItem.data.info.baseStat.type == ItemManager.ItemType.Potion
            || currentItem.data.info.baseStat.type == ItemManager.ItemType.Book
            || currentItem.data.info.baseStat.type == ItemManager.ItemType.Scroll
            || currentItem.data.info.baseStat.type == ItemManager.ItemType.Material)
        {
            verticalSlash.SetActive(false);
            itemViewStatGroup.SetActive(false);
        }
        else if (currentItem.data.info.baseStat.type != ItemManager.ItemType.Currency) //----3. If Item is Weapon -> Ring
        {
            verticalSlash.SetActive(true);
            itemViewStatGroup.SetActive(true);
            foreach (UIStat stat in itemStats)
            {
                stat.gameObject.SetActive(false);
            }
            int statLen = currentItem.data.currentStat.Length;
            for (int i = 0; i < statLen; i++)
            {
                itemStats[i].gameObject.SetActive(true);
                itemStats[i].statImage.sprite = CheckStatImage(currentItem.data.currentStat[i]);
                itemStats[i].statText.text = currentItem.data.currentStat[i].value.ToString();
            }
        }

        //---Then, set main features
        itemNameUI.text = currentItem.data.info.prop.itemName;
        itemBackgroundUI.sprite = currentItem.backGround.sprite;
        itemFrameUI.sprite = currentItem.frame.sprite;
        itemImageUI.sprite = currentItem.img.sprite;
        if (itemViewStatGroup.activeInHierarchy)
        {
            itemLevel.text = "Require Level " + currentItem.data.info.baseStat.requiredLevel.ToString();
            itemRarity.text = CheckRarity(currentItem);
        }
        if (itemSpecialStat.gameObject.activeInHierarchy)
        {
            itemSpecialStat.text = currentItem.data.info.prop.specialStat;
        }
        itemDescription.text = currentItem.data.info.prop.itemDescription;


    }
    Sprite CheckStatImage(ItemInfo.ItemStat.Stat stat)
    {
        if(stat.type == ItemManager.StatType.Attack)
        {
            return statSprites[0];
        }
        else if (stat.type == ItemManager.StatType.AttackSpeed)
        {
            return statSprites[1];
        }
        else if (stat.type == ItemManager.StatType.AttackRange)
        {
            return statSprites[2];
        }
        else if(stat.type == ItemManager.StatType.Health)
        {
            return statSprites[3];
        }
        else if(stat.type == ItemManager.StatType.PhysicalDefense)
        {
            return statSprites[4];
        }
        else if (stat.type == ItemManager.StatType.MagicalDefense)
        {
            return statSprites[5];
        }
        return null;
    }
    string CheckRarity(InventoryItem item)
    {
        switch (item.data.info.baseStat.rarity.ToString())
        {
            case "God":
                return "<color=#FF69B4>God</color>";
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
        if (isCarryingItem) return;

        InventoryItem item = ActiveSlot.GetComponentInChildren<InventoryItem>();
        if (item != null && !item.data.info.prop.countable)
        {
            item.transform.SetParent(customCursor);
            item.transform.localPosition = Vector3.zero;
            isCarryingItem = true;
        }
    }
    private void DisplayYellowField(ref EquipmentSlot equipmentSlot, EquipmentSlot equipableSlot)
    {
        //If item is taken from equipment slot => Display inventoryField if in range
        if (ActiveSlot.GetComponentInChildren<EquipmentSlot>() != null)
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
        else //If item is taken from inventory slot => Display equipField if in range
        {
            if (is_rectTransformsOverlap(customCursor.GetComponent<RectTransform>(), equipSlotRect.GetComponent<RectTransform>()))
            {
                equipmentSlot = equipableSlot;
                equipField.SetActive(true);
                equipableSlot.ShowCanEquip();
            }
            else
            {
                equipField.SetActive(false);
                equipableSlot.ShowCannotEquip();
            }
        }
    }
    private void OnDragItem() //Means when holding item
    {
        //Step 1: Check Slot that has similar types with carrying item => Ex: Sword fits Weapon slot
        EquipmentSlot equipmentSlot = null;
        //Check the equip slot that have the same type with item
        EquipmentSlot equipableSlot = equipmentSlots.Single(i => i.type == customCursor.GetComponentInChildren<InventoryItem>().data.info.baseStat.type);
        DisplayYellowField(ref equipmentSlot, equipableSlot);

        //Step 2: Put Item into Slot
        InventoryItem getItem = customCursor.GetComponentInChildren<InventoryItem>();
        if (Input.GetMouseButtonUp(0))
        {
            //Case 1: Item taken from Inventory Slot
            if (ActiveSlot.GetComponentInChildren<InventorySlot>() != null)
            {
                if (!equipmentSlot) // Put item back to inventory (Nothing happen)
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

                    //Update the database
                    InventoryData.RemoveItemData(InventoryData.equipmentData, itemEquip.data.ID);
                    InventoryData.AddInventoryData(itemEquip.data);

                    InventoryData.RemoveItemData(InventoryData.inventoryData, getItem.data.ID);
                    InventoryData.AddEquipmentData(getItem.data);

                    playerStat.RemoveItemStat(itemEquip);
                    playerStat.AddItemStat(getItem);

                    isCarryingItem = false;
                }
                else //Equip Item
                {
                    getItem.transform.SetParent(equipmentSlot.transform);
                    getItem.transform.localPosition = Vector3.zero;
                    equipmentSlot.isEquip = true;
                    ActiveSlot.GetComponent<InventorySlot>().isEmpty = true;
                    equipmentSlot.ShowCannotEquip();

                    InventoryData.RemoveItemData(InventoryData.inventoryData, getItem.data.ID);
                    InventoryData.AddEquipmentData(getItem.data);

                    playerStat.AddItemStat(getItem);

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

                            InventoryData.RemoveItemData(InventoryData.equipmentData, getItem.data.ID);
                            InventoryData.AddInventoryData(getItem.data);

                            playerStat.RemoveItemStat(getItem);

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

    //You should read this part, which I explain how to check the rect transform that is overlapped by other rect transform 
    //Therefore, when I take the item nearby the inventory, I can drop it. 
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
    public void AddAmountOfItem(ItemBase.ItemData data, int amount)
    {
        foreach (GameObject emptySlot in emptySlots)
        {
            InventorySlot slot = emptySlot.GetComponent<InventorySlot>();
            if (slot.isEmpty)
            {
                GameObject itemAdd = Instantiate(ItemPrefab, transform.position, Quaternion.identity);

                itemAdd.transform.SetParent(emptySlot.transform);
                itemAdd.transform.SetSiblingIndex(4); //If it is the last => it will be faded
                itemAdd.transform.localPosition = new Vector3(0, 0, 0);
                slot.isEmpty = false;

                //Initialize data for new item => Fix in the future because it should be init in shop system/craft/gacha.
                //Means it should be initialized when the item was created, not when add to inventory.
                InventoryItem item = itemAdd.GetComponent<InventoryItem>();
                item.data.ID = GenerateID();
                item.data.info = data.info;
                item.data.amount += amount;
                item.data.currentStat = data.info.baseStat.stats;

                //Add data to database
                InventoryData.AddInventoryData(item.data);

                if (data.info.prop.countable)
                {
                    slot.countText.text = InventoryData.GetAmount(data.info).ToString();
                }
                break;
            }
            else if (data.info.prop.countable) //If slot is not empty and item is countable
            {
                InventoryItem item = slot.GetComponentInChildren<InventoryItem>();
                if (item.data.info.prop.itemName == data.info.prop.itemName)
                {
                    item.data.amount += amount;
                    //Add data to database
                    InventoryData.AddInventoryData(item.data);

                    slot.countText.text = item.data.amount.ToString();
                    break;
                }
            }
        }
    }
    public void RemoveAmountOfItem(ItemInfo info, int amount)
    {
        foreach (GameObject emptySlot in emptySlots)
        {
            InventorySlot slot = emptySlot.GetComponent<InventorySlot>();
            InventoryItem item = emptySlot.GetComponentInChildren<InventoryItem>();
            if (item != null)
            {
                if (item.data.info.prop.itemName == info.prop.itemName)
                {
                    item.data.amount -= amount;

                    if (item.data.amount == 0)
                    {
                        InventoryData.RemoveItemData(InventoryData.inventoryData, item.data.ID);
                        slot.DestroyItem();
                    }
                    else if (item.data.amount > 0)
                    {
                        //Update to database
                        InventoryData.AddInventoryData(item.data);
                        slot.countText.text = item.data.amount.ToString();
                    }
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
            InventoryData.RemoveItemData(InventoryData.inventoryData, activeSlot.GetComponentInChildren<InventoryItem>().data.ID);
            activeSlot.DestroyItem();
        }
    }
    public int GenerateID()
    {
        int randNum;
        do
        {
            randNum = Random.Range(1000, 10000);
        } while (InventoryData.inventoryData.Exists(x => x.ID == randNum)); 
        /* Exists is similar to Single:
         * - Exists used for List
         * - Single used for array
         */
        return randNum;
    }

    public int GetAmountOfItem(ItemInfo info)
    {
        foreach (GameObject emptySlot in emptySlots)
        {
            InventoryItem item = emptySlot.GetComponentInChildren<InventoryItem>();
            if (item != null)
            {
                if (item.data.info.prop.itemName == info.prop.itemName)
                {
                    return item.data.amount;
                }
            }
        }
        return 0;
    }
    public void InitializeInventoryItemFromData(ItemBase.ItemData data) //Similar to AddItem, just remove the line AddItemData. 
    {
        foreach (GameObject emptySlot in emptySlots)
        {
            InventorySlot slot = emptySlot.GetComponent<InventorySlot>();
            if (slot.isEmpty)
            {
                GameObject itemAdd = Instantiate(ItemPrefab, transform.position, Quaternion.identity);

                itemAdd.transform.SetParent(emptySlot.transform);
                itemAdd.transform.SetSiblingIndex(4); //If it is the last => it will be faded
                itemAdd.transform.localPosition = new Vector3(0, 0, 0);
                slot.isEmpty = false;

                InventoryItem item = itemAdd.GetComponent<InventoryItem>();
                item.data = data;


                if (data.info.prop.countable)
                {
                    slot.countText.text = InventoryData.GetAmount(data.info).ToString();
                    
                }
                break;
            }
        }
    }
    public void InitializeEquippedItemFromData(ItemBase.ItemData data)
    {
        foreach (EquipmentSlot slot in equipmentSlots)
        {
            if (slot.type == data.info.baseStat.type)
            {
                GameObject itemAdd = Instantiate(ItemPrefab, transform.position, Quaternion.identity);
                itemAdd.transform.SetParent(slot.transform);
                itemAdd.transform.localPosition = Vector3.zero;

                InventoryItem item = itemAdd.GetComponent<InventoryItem>();
                item.data = data;

                slot.isEquip = true;
            }
        }
    }
    public void RearrangeItem()
    {

    }


    public void QuitGame()
    {
        Application.Quit();
    }
    public void OnApplicationQuit()
    {
        // Save any unsaved data here
        DataInventory.SaveData(InventoryData);
    }
}
