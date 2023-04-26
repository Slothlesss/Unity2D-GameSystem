using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
[CreateAssetMenu(fileName = "new data", menuName = "Data")]
public class Data : ScriptableObject
{
    public List<ItemBase.ItemData> inventoryData;
    public List<ItemBase.ItemData> equipmentData;
    public int GetAmount(ItemInfo info) //Fix in the future, I want to get amout
    {
        foreach (ItemBase.ItemData data in inventoryData)
        {
            if (data.info == info) //if already have
            {
                return data.amount;
            }
        }
        return 0;
    }
    public void AddInventoryData(ItemBase.ItemData newData)
    {
        foreach (ItemBase.ItemData data in inventoryData)
        {
            if (data.info == newData.info) //if already have
            {
                if (newData.info.prop.countable) //if item is countable, otherwise we always added item, so I write at below.
                {
                    data.amount += newData.amount;
                    return;
                }
            }
        }
        inventoryData.Add(newData);
    }
    public void AddEquipmentData(ItemBase.ItemData data)
    {
        equipmentData.Add(data);
    }
    public void RemoveItemData(List<ItemBase.ItemData> datas, int ID)
    {
        foreach (ItemBase.ItemData data in datas)
        {
            if(data.ID == ID)
            {
                datas.Remove(data);
                return;
            }
        }
    }

    public static void SaveData(Data data)
    {
        SaveData saveData = new SaveData();
        saveData.inventoryData = data.inventoryData;
        saveData.equipmentData = data.equipmentData;

        string json = JsonUtility.ToJson(saveData);
        string path = Application.persistentDataPath + "/savedata.json";
        File.WriteAllText(path, json);
    }
    public static Data LoadData()
    {
        string path = Application.persistentDataPath + "/savedata.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);
            Data data = ScriptableObject.CreateInstance<Data>();
            data.inventoryData = saveData.inventoryData;
            data.equipmentData = saveData.equipmentData;
            return data;
        }
        else
        {
            return null;
        }
    }
}

[System.Serializable]
public class SaveData
{
    public List<ItemBase.ItemData> inventoryData;
    public List<ItemBase.ItemData> equipmentData;
}
