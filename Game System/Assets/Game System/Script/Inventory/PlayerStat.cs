using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : Singleton<PlayerStat>
{
    public DataPlayer playerData;

    public GameObject statsUIGroup;
    public UIStat[] statsUI;
    public void Awake()
    {
        if (DataPlayer.LoadData() != null)
        {
            playerData = DataPlayer.LoadData();
        }
        //StartCoroutine(SaveDataPeriodically(10.0f)); //The data will be saved automatically once after 10 sec.
        statsUI = statsUIGroup.GetComponentsInChildren<UIStat>();
        InitializePlayerStatFromData();


        string path = Application.persistentDataPath + "/playerData.json";
        print(path);
    }
    void InitializePlayerStatFromData()
    {
        //---Update total stats--- (this one will overlap the previous step)
        for (int i = 0; i < playerData.baseStats.Length; i++)
        {
            ItemManager.StatType statType = playerData.baseStats[i].type;
            if (statType == ItemManager.StatType.AttackRange
                || statType == ItemManager.StatType.AttackSpeed)
            {
                if (playerData.additionalStats[i].value == 0) //Mean player dont equip any item => player normal attack (e.g punch) should have attack range and attack speed.
                {
                    statsUI[i].statText.text = playerData.baseStats[i].value.ToString();
                }
                else
                {
                    statsUI[i].statText.text = playerData.additionalStats[i].value.ToString();
                }
            }
            else
            {
                statsUI[i].statText.text = (playerData.baseStats[i].value + playerData.additionalStats[i].value).ToString();
            }
        }
    }
    public void AddItemStat(InventoryItem item)
    {
        int statLen = item.data.currentStat.Length;
        for (int i = 0; i < statLen; i++)
        {
            ItemManager.StatType itemType = item.data.currentStat[i].type;
            float itemStat = item.data.currentStat[i].value;
            if (itemType == ItemManager.StatType.AttackRange
                || itemType == ItemManager.StatType.AttackSpeed)
            {
                playerData.additionalStats[GetIndex(itemType)].value = itemStat;
            }
            else
            {
                playerData.additionalStats[GetIndex(itemType)].value += itemStat;
            }
        }
        InitializePlayerStatFromData();
    }
    public void RemoveItemStat(InventoryItem item)
    {
        int statLen = item.data.currentStat.Length;
        for (int i = 0; i < statLen; i++)
        {
            ItemManager.StatType itemType = item.data.currentStat[i].type;
            float itemStat = item.data.currentStat[i].value;
            if (itemType == ItemManager.StatType.AttackRange
                || itemType == ItemManager.StatType.AttackSpeed)
            {
                playerData.additionalStats[GetIndex(itemType)].value = 0;
            }
            else
            {
                playerData.additionalStats[GetIndex(itemType)].value -= itemStat;
            }
        }
        InitializePlayerStatFromData();
    }
    public int GetIndex(ItemManager.StatType type)
    {
        for (int i = 0; i < playerData.baseStats.Length; i++)
        {
            if (playerData.baseStats[i].type == type) return i;
        }
        return -1;
    }
    /*IEnumerator SaveDataPeriodically(float interval)
    {
        while (true)
        {
            DataPlayer.SaveData(playerData);
            yield return new WaitForSeconds(interval);
        }
    }
    */
    public void OnApplicationQuit()
    {
        // Save any unsaved data here
        DataPlayer.SaveData(playerData);
    }
}
