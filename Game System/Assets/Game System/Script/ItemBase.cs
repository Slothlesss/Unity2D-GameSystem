using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


[System.Serializable]
public class ItemBase : MonoBehaviour
{
    [SerializeField] public Image img;
    [SerializeField] public Image backGround;
    [SerializeField] public Image frame;

    //Question: Why we need to create a ItemData class? Why don't we put everything like ID, info, amount, currentStat outside?
    //Answer: Because when we want to use operation = (to copy these values), we just need to use data = data instead of id = id, info = info, ...

    [System.Serializable]
    public class ItemData 
    {
        public int ID;
        public ItemInfo info; //Item Info will have base stats of item => when create Item GameObject, the gameObject will have base baseStat and can fix it.
        public int amount = 0;
        public int currentLevel = 0;
        public ItemInfo.ItemStat.Stat[] currentStat;


    }
    public ItemData data;

    //Question: Why we need to use ItemInfo.ItemStat currentStat but not use info.stat ?
    //Answer: Because Scriptable Object is read-only, we can't add value to info.stat.
    //        Therefore, we copy all the stats from info to current stats.
    //        After that, if we want to upgrade, the value will be added to current stat, not to info.stat.

    
    
    public virtual void Start()
    {
        UpdateItemImage();
    }

    public virtual void Update()
    {
        Start();
    }

    public virtual void UpdateItemImage()
    {
        if (data.info == null) return;

        img.sprite = data.info.prop.image;
        img.color = Color.white;
        CheckRarity();

    }

    public int SetPriceByRarity()
    {
        if (data.info != null)
        {
            if (data.info.baseStat.rarity == ItemManager.Rarity.Common)
            {
                return 1;
            }
            else if (data.info.baseStat.rarity == ItemManager.Rarity.Uncommon)
            {
                return 5;
            }
            else if (data.info.baseStat.rarity == ItemManager.Rarity.Rare)
            {
                return 20;
            }
            else if (data.info.baseStat.rarity == ItemManager.Rarity.Epic)
            {
                return 100;
            }
            else if (data.info.baseStat.rarity == ItemManager.Rarity.Mythical)
            {
                return 500;
            }
            else if (data.info.baseStat.rarity == ItemManager.Rarity.Legendary)
            {
                return 2000;
            }
            else if (data.info.baseStat.rarity == ItemManager.Rarity.God)
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
            if (data.info.baseStat.rarity == ItemManager.Rarity.Common)
            {
                backGround.sprite = ItemManager.Instance.rarityBG[0].background;
                frame.sprite = ItemManager.Instance.rarityBG[0].frame;
            }
            else if (data.info.baseStat.rarity == ItemManager.Rarity.Uncommon)
            {
                backGround.sprite = ItemManager.Instance.rarityBG[1].background;
                frame.sprite = ItemManager.Instance.rarityBG[1].frame;
            }
            else if (data.info.baseStat.rarity == ItemManager.Rarity.Rare)
            {
                backGround.sprite = ItemManager.Instance.rarityBG[2].background;
                frame.sprite = ItemManager.Instance.rarityBG[2].frame;
            }
            else if (data.info.baseStat.rarity == ItemManager.Rarity.Epic)
            {
                backGround.sprite = ItemManager.Instance.rarityBG[3].background;
                frame.sprite = ItemManager.Instance.rarityBG[3].frame;
            }
            else if (data.info.baseStat.rarity == ItemManager.Rarity.Mythical)
            {
                backGround.sprite = ItemManager.Instance.rarityBG[4].background;
                frame.sprite = ItemManager.Instance.rarityBG[4].frame;
            }
            else if (data.info.baseStat.rarity == ItemManager.Rarity.Legendary)
            {
                backGround.sprite = ItemManager.Instance.rarityBG[5].background;
                frame.sprite = ItemManager.Instance.rarityBG[5].frame;
            }
            else if (data.info.baseStat.rarity == ItemManager.Rarity.God)
            {
                backGround.sprite = ItemManager.Instance.rarityBG[6].background;
                frame.sprite = ItemManager.Instance.rarityBG[6].frame;
            }
        }
    }
}
