using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "new data", menuName = "Data")]
public class Data : ScriptableObject
{

    [System.Serializable]
    public class ItemData
    {
        public ItemInfo info;
        public int amount;
        public int ID;
        public ItemData(ItemInfo i, int a)
        {
            info = i;
            amount = a;
        }
    }

    public List<ItemData> itemDatas;

    public void AddItemData(ItemInfo info, int amount)
    {
        foreach (ItemData data in itemDatas)
        {
            if (data.info == info) //if already have
            {
                if (info.countable) //if item is countable, otherwise we always added item, so I write at below.
                {
                    data.amount += amount;
                    return;
                }
            }
        }
        ItemData newData = new ItemData(info, amount);
        itemDatas.Add(newData);
    }

    void RemoveItemData(ItemInfo info)
    {
        itemDatas.Remove(new ItemData(info, 1));
    }

}
