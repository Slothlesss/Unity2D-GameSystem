using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "new Item", menuName = "Equipment")]

public class ItemInfo : ScriptableObject
{
    public Sprite image;

    public Color color;
    public bool countable;
    public string itemName;
    public int itemLevel;
    public ItemManager.ItemType typeOfItem;
    public ItemManager.Rarity rarity;

}
