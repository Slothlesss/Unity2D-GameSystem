using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "new data", menuName = "Data")]
public class Data : ScriptableObject
{
    [System.Serializable]
    public class ItemData 
    {
        public int ID;
        public ItemInfo item;
        public int amount;
        public ItemData(int _id, ItemInfo _item, int _amount)
        {
            ID = _id;
            item = _item;
            amount = _amount;
        }
    }

    public List<ItemData> itemDatas;
    public int GetAmount(ItemInfo info)
    {
        foreach (ItemData data in itemDatas)
        {
            if (data.item == info) //if already have
            {
                return data.amount;
            }
        }
        return 0;
    }
    public void AddItemData(ItemInfo info, int amount, int id)
    {
        foreach (ItemData data in itemDatas)
        {
            if (data.item == info) //if already have
            {
                if (info.countable) //if item is countable, otherwise we always added item, so I write at below.
                {
                    data.amount += amount;
                    return;
                }
            }
        }
        ItemData newData = new ItemData(id, info, amount);
        itemDatas.Add(newData);
    }

    public void RemoveItemData(int ID)
    {
        foreach (ItemData data in itemDatas)
        {
            if(data.ID == ID)
            {
                itemDatas.Remove(data);
                return;
            }
        }
    }

}
