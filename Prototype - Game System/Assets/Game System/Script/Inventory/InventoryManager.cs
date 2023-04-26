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
    public StatUI[] itemStats;
    public GameObject inventoryField;
    public PlayerStat playerStat;

    public Sprite[] statSprites;

    public int startIdx = 0;
    
    private void Awake()
    {
        if (Data.LoadData() != null)
        {
            InventoryData = Data.LoadData();
        }
        //StartCoroutine(SaveDataPeriodically(10.0f)); //The data will be saved automatically once after 10 sec.
        Instance = this;
        itemStats = itemViewStatGroup.GetComponentsInChildren<StatUI>();
        foreach (StatUI stat in itemStats)
        {
            stat.gameObject.SetActive(false);
        }
        SpawnSlots();
        SpawnItemsFromData();
    }
    /*IEnumerator SaveDataPeriodically(float interval)
    {
        while (true)
        {
            Data.SaveData(InventoryData);
            yield return new WaitForSeconds(interval);
        }
    } */
    public void OnApplicationQuit()
    {
        // Save any unsaved data here
        Data.SaveData(InventoryData);
    }
    public void QuitGame()
    {
        Application.Quit();
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
    void SpawnItemsFromData()
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
    void ChooseItem()
    {
        InventoryItem currentItem = ActiveSlot.GetComponentInChildren<InventoryItem>();
        DisplayItemStat(currentItem);
    }
    void DisplayItemStat(InventoryItem currentItem)
    {
        if (currentItem != null)
        {   //---Check special condition
            //----1. Check specialStat
            if (currentItem.data.info.prop.specialStat == "")
            {
                itemSpecialStat.gameObject.SetActive(false);
            }
            else
            {
                itemSpecialStat.gameObject.SetActive(true);
            }
            //----2. Check type of item => if (item == Potion, Book, Scroll => only show Image, no level, no rarity) || or you can set your own type
            if (currentItem.data.info.stat.type == ItemManager.ItemType.Potion
                || currentItem.data.info.stat.type == ItemManager.ItemType.Book
                || currentItem.data.info.stat.type == ItemManager.ItemType.Scroll)
            {
                verticalSlash.SetActive(false);
                itemViewStatGroup.SetActive(false);
            }
            else if(currentItem.data.info.stat.type != ItemManager.ItemType.Currency) //----3. If Item is Weapon -> Ring
            {
                verticalSlash.SetActive(true);
                itemViewStatGroup.SetActive(true);
                foreach(StatUI stat in itemStats)
                {
                    stat.gameObject.SetActive(false);
                }
                int statLen = currentItem.data.stat.stats.Length;
                for(int i = 0; i < statLen; i++)
                {
                    itemStats[i].gameObject.SetActive(true);
                    itemStats[i].statImage.sprite = checkStatImage(currentItem.data.stat.stats[i]);
                    itemStats[i].statText.text = currentItem.data.stat.stats[i].value.ToString();
                }
            }

            //---Then, set main features
            itemNameUI.text = currentItem.data.info.prop.itemName;
            itemBackgroundUI.sprite = currentItem.backGround.sprite;
            itemFrameUI.sprite = currentItem.frame.sprite;
            itemImageUI.sprite = currentItem.img.sprite;
            if (itemViewStatGroup.activeInHierarchy)
            {
                itemLevel.text = "Level " + currentItem.data.info.stat.itemLevel.ToString();
                itemRarity.text = checkRarity(currentItem);
            }
            if (itemSpecialStat.gameObject.activeInHierarchy)
            {
                itemSpecialStat.text = currentItem.data.info.prop.specialStat;
            }
            itemDescription.text = currentItem.data.info.prop.itemDescription;

        }
    }
    Sprite checkStatImage(ItemInfo.ItemStat.Stat stat)
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
    string checkRarity(InventoryItem item)
    {
        switch (item.data.info.stat.rarity.ToString())
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
        
        if (Input.GetMouseButtonDown(0) && Vector3.Distance(customCursor.position, ActiveSlot.transform.position) < 50f)
        {
            if (!isCarryingItem)
            {
                InventoryItem item = ActiveSlot.GetComponentInChildren<InventoryItem>();
                if (item != null && !item.data.info.prop.countable)
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
        EquipmentSlot equipableSlot = equipmentSlots.Single(i => i.type == customCursor.GetComponentInChildren<InventoryItem>().data.info.stat.type);
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
        else //Display equiptableField
             //Using the same way when unequip item 
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
    public void AddItem(ItemBase.ItemData data)
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

                //Initialize data for new item => Fix in the future because it should be init in shop system
                InventoryItem item = itemAdd.GetComponent<InventoryItem>();
                item.data.ID = GenerateID();
                item.data.info = data.info;
                item.data.amount += 1; //Will change to amount instead of 1
                item.data.stat = data.info.stat;

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
                    item.data.amount += 1;
                    //Add data to database
                    InventoryData.AddInventoryData(data);

                    slot.countText.text = item.data.amount.ToString();
                    break;
                }
            }
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
            if (slot.type == data.info.stat.type)
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
    public void DeleteItem()
    {
        InventorySlot activeSlot = ActiveSlot.GetComponent<InventorySlot>();
        if (!activeSlot.isEmpty)
        {
            InventoryData.RemoveItemData(InventoryData.inventoryData, activeSlot.GetComponentInChildren<InventoryItem>().data.ID);
            activeSlot.DestroyItem();
        }
    }
    public void RearrangeItem()
    {

    }
}
