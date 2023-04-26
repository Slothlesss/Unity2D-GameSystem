using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [System.Serializable]
    
    public struct rarityBackGround
    {
        public string name;
        public Sprite background;
        public Sprite frame;

    }

    public enum ItemType 
    { 
        Currency,
        Weapon, 
        Armor, 
        Shoes, 
        Helmet, 
        Necklace, 
        Ring, 
        Scroll, 
        Potion, 
        Book 
    }
    public enum Rarity 
    { 
        Common, 
        Uncommon,
        Rare,
        Epic,
        Mythical,
        Legendary,
        God 
    }
    public enum StatType
    {
        Attack,
        AttackRange,
        AttackSpeed,
        PhysicalDefense,
        MagicalDefense,
        Health,
    }

    public rarityBackGround[] rarityBG;

    public static ItemManager Instance;
    private void Awake()
    {
        Instance = this;
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
