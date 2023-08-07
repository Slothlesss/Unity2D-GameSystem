using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;
public class UpgradeManager : MonoBehaviour
{
    [SerializeField] Toggle upgradeSystemButton;
    [SerializeField] private List<InventoryItem> equipItems;
    [SerializeField] private GameObject equipItemPool;

    [SerializeField] private InventoryItem upgradeItem;
    [SerializeField] private InventoryItem previewItem;

    [SerializeField] private TextMeshProUGUI[] currentStatTexts;
    [SerializeField] private TextMeshProUGUI[] previewStatTexts;

    private int currentIdx;

    private void Awake()
    {
        equipItems = equipItemPool.GetComponentsInChildren<InventoryItem>().ToList();
        foreach(InventoryItem item in equipItems)
        {
            item.gameObject.SetActive(false);
        }

        upgradeSystemButton.onValueChanged.AddListener(delegate {
            GetEquipItem();
            UndisplayItem();
        });

    }

    private void GetEquipItem()
    {
        for(int i = 0; i < 6; i++)
        {
            EquipmentSlot slot = InventoryManager.Instance.equipmentSlots[i];
            if (InventoryManager.Instance.equipmentSlots[i].isEquip)
            {
                equipItems[i].gameObject.SetActive(true);
                equipItems[i].data = slot.GetComponentInChildren<InventoryItem>().data;
            }
            else
            {
                equipItems[i].gameObject.SetActive(false);
            }
        }

        // Question: Why we need upgradeItem.data become null when starting?
        // Answer: If you upgrade the first sword, then you swap another sword
        //         Then you go to upgrade system, but not click to any item, click to "upgrade button" => it keeps upgrading the old item
        //         Because the upgradeItem didn't reset.
        // You can see in the UpgradeItem(), the second condition is if (upgradeItem.data == null) return;
        upgradeItem.data = null;
    }

    public void DisplayItem(int idx)
    {
        currentIdx = idx;
        if (!equipItems[idx].gameObject.activeInHierarchy )
        {
            UndisplayItem();
            return;
        }

        upgradeItem.gameObject.SetActive(true);
        previewItem.gameObject.SetActive(true);

        upgradeItem.data = equipItems[idx].data;

        previewItem.data.info = upgradeItem.data.info;

        DisplayUpgradeCurrentStat();
        DisplayUpgradeStatPreview();
    }

    public void UpgradeItem()
    {
        //+ Step 1: Go to Player Stat scripts and read two functions RemoveItemStat() and AddItemStat() 
        //          RemoveItemStat(): Remove current stats of current equipped item
        //          AddItemStat(): Add the stat of the equipped item
        //          => You will understand why.
        //+ Step 2: Firstly, I removed all the stats of the equipped item from player stat
        //          Then, we upgrade the item => the stats of item will be increased
        //          Finally, we add the new stat to player stat.
        //
        // Question: Why don't you update the stats of equipped item?
        // Answer: It will be more complex because we need to add one more function. Just ultilize all functions we have.

        if (!equipItems[currentIdx].gameObject.activeInHierarchy || upgradeItem.data == null)
        {
            return;
        }

        PlayerStat.Instance.RemoveItemStat(upgradeItem);

        upgradeItem.data.currentLevel++;
        foreach(ItemInfo.ItemStat.Stat stat in upgradeItem.data.currentStat)
        {
            stat.value = stat.GetNextValue();
        }

        PlayerStat.Instance.AddItemStat(upgradeItem);
        DisplayUpgradeCurrentStat();
        DisplayUpgradeStatPreview();
    }
    private void DisplayUpgradeCurrentStat()
    {
        int len = upgradeItem.data.currentStat.Length;
        for(int i = 0; i < len; i++)
        {
            currentStatTexts[i].gameObject.SetActive(true);
            currentStatTexts[i].text = upgradeItem.data.currentStat[i].type.ToString() + ": " 
                + upgradeItem.data.currentStat[i].value.ToString();
        }
        for(int i = len; i < 3; i++)
        {
            currentStatTexts[i].gameObject.SetActive(false);
        }
    }
    private void DisplayUpgradeStatPreview()
    {
        previewItem.data.currentLevel = upgradeItem.data.currentLevel + 1;

        previewItem.levelText.color = Color.green;

        int len = upgradeItem.data.currentStat.Length;
        for (int i = 0; i < len; i++)
        {
            previewStatTexts[i].gameObject.SetActive(true);

            string coloredStatText = "<color=green>" + upgradeItem.data.currentStat[i].GetNextValue().ToString() + "</color>";
            string normalStatText = upgradeItem.data.currentStat[i].GetNextValue().ToString();
            string typeText = upgradeItem.data.currentStat[i].type.ToString();

            if (upgradeItem.data.currentStat[i].GetNextValue() == upgradeItem.data.currentStat[i].value)
            {
                previewStatTexts[i].text = typeText + ": " + normalStatText;
            }
            else
            {
                previewStatTexts[i].text = typeText + ": " + coloredStatText;
            }

        }
        for (int i = len; i < 3; i++)
        {
            previewStatTexts[i].gameObject.SetActive(false);
        }
    }

    private void UndisplayItem()
    {
        upgradeItem.gameObject.SetActive(false);
        previewItem.gameObject.SetActive(false);
        foreach (TextMeshProUGUI text in currentStatTexts)
        {
            text.gameObject.SetActive(false);
        }
        foreach (TextMeshProUGUI text in previewStatTexts)
        {
            text.gameObject.SetActive(false);
        }
    }
}
