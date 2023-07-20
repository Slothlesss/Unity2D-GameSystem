using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


[System.Serializable]
public class ItemBase : MonoBehaviour
{

    [System.Serializable]
    public class ItemData
    {
        public int ID;
        public ItemInfo info; //Item Info will have base stats of item => when create Item GameObject, the gameObject will have base stat and can fix it.
        public int amount = 0;
        public ItemInfo.ItemStat stat;

        public ItemData(ItemData data)
        {
            ID = data.ID;
            info = data.info;
            amount = data.amount;
            stat = data.stat;
        }
        public int GetAmount()
        {
            return amount;
        }
        public int GetID()
        {
            return ID;
        }

    }
    public ItemData data;

    [SerializeField] public Image img;
    [SerializeField] public Image backGround;
    [SerializeField] public Image frame;

    // Start is called before the first frame update
    public virtual void Start()
    {

        if (data.info != null)
        {
            img.sprite = data.info.prop.image;
            img.color = Color.white;
            CheckRarity();
        }


    }
    // Update is called once per frame
    public virtual void Update()
    {
        Start();
    }

    public int SetPriceByRarity()
    {
        if (data.info != null)
        {
            if (data.info.stat.rarity == ItemManager.Rarity.Common)
            {
                return 1;
            }
            else if (data.info.stat.rarity == ItemManager.Rarity.Uncommon)
            {
                return 5;
            }
            else if (data.info.stat.rarity == ItemManager.Rarity.Rare)
            {
                return 20;
            }
            else if (data.info.stat.rarity == ItemManager.Rarity.Epic)
            {
                return 100;
            }
            else if (data.info.stat.rarity == ItemManager.Rarity.Mythical)
            {
                return 500;
            }
            else if (data.info.stat.rarity == ItemManager.Rarity.Legendary)
            {
                return 2000;
            }
            else if (data.info.stat.rarity == ItemManager.Rarity.God)
            {
                return 10000;
            }
        }
        return 0;
    }



    public void CheckRarity()
    {
        if (data.info != null)
        {
            if (data.info.stat.rarity == ItemManager.Rarity.Common)
            {
                backGround.sprite = ItemManager.Instance.rarityBG[0].background;
                frame.sprite = ItemManager.Instance.rarityBG[0].frame;
            }
            else if (data.info.stat.rarity == ItemManager.Rarity.Uncommon)
            {
                backGround.sprite = ItemManager.Instance.rarityBG[1].background;
                frame.sprite = ItemManager.Instance.rarityBG[1].frame;
            }
            else if (data.info.stat.rarity == ItemManager.Rarity.Rare)
            {
                backGround.sprite = ItemManager.Instance.rarityBG[2].background;
                frame.sprite = ItemManager.Instance.rarityBG[2].frame;
            }
            else if (data.info.stat.rarity == ItemManager.Rarity.Epic)
            {
                backGround.sprite = ItemManager.Instance.rarityBG[3].background;
                frame.sprite = ItemManager.Instance.rarityBG[3].frame;
            }
            else if (data.info.stat.rarity == ItemManager.Rarity.Mythical)
            {
                backGround.sprite = ItemManager.Instance.rarityBG[4].background;
                frame.sprite = ItemManager.Instance.rarityBG[4].frame;
            }
            else if (data.info.stat.rarity == ItemManager.Rarity.Legendary)
            {
                backGround.sprite = ItemManager.Instance.rarityBG[5].background;
                frame.sprite = ItemManager.Instance.rarityBG[5].frame;
            }
            else if (data.info.stat.rarity == ItemManager.Rarity.God)
            {
                backGround.sprite = ItemManager.Instance.rarityBG[6].background;
                frame.sprite = ItemManager.Instance.rarityBG[6].frame;
            }
        }
    }
}
