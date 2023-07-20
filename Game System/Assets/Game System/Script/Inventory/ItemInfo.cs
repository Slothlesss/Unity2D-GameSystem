using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CreateAssetMenu(fileName = "new Item", menuName = "Equipment")]
public class ItemInfo : ScriptableObject
{

    [System.Serializable]
    public class ItemProperties
    {
        public Sprite image;
        public Color color = Color.white;
        public bool countable;
        public string itemName;
        [TextArea(2, 2)]
        public string specialStat;
        [TextArea(3, 3)]
        public string itemDescription;
        
    }
    [System.Serializable]
    public class ItemStat
    {
        public ItemManager.ItemType type;
        public ItemManager.Rarity rarity;
        public int itemLevel = 1;
        [SerializeField]
        public Stat[] stats;

        [System.Serializable]
        public class Stat
        {
            public ItemManager.StatType type;
            public float value;

        }
    }
    public ItemProperties prop;
    public ItemStat stat;
  
}
