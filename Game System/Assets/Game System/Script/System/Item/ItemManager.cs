using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ItemManager : Singleton<ItemManager>
{
    [System.Serializable]
    public struct rarityBackGround
    {
        public Rarity rarity;
        public Sprite background;
    }

    public rarityBackGround[] backgrounds;
    [SerializeField]
    public Dictionary<Rarity, Sprite> rarityBackgroundDict;

    private void Awake()
    {
        InitializeDictionary();
    }

    public void InitializeDictionary()
    {
        rarityBackgroundDict = new Dictionary<Rarity, Sprite>();
        foreach (rarityBackGround item in backgrounds)
        {
            rarityBackgroundDict.Add(item.rarity, item.background);
        }
    }

    public Rarity GetRandomShopDailyRarity()
    {
        int rand = Random.Range(0, 101);
        if (rand <= 5) return Rarity.Epic;
        else if (rand <= 15) return Rarity.Rare;
        else if (rand <= 45) return Rarity.Uncommon;
        else return Rarity.Common;
    }
}