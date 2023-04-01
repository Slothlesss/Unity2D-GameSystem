using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItem : ItemBase
{
    private int level;
    public TextMeshProUGUI levelText;
    public int Level
    {
        get { return level; }
        set
        {
            this.level = value;
            levelText.text = "+" + value.ToString();
            Stats = Mathf.CeilToInt(Mathf.Sqrt(SetPriceByRarity() * item.itemLevel * (Level + 1)));
            NextStats = Mathf.CeilToInt(Mathf.Sqrt(SetPriceByRarity() * item.itemLevel * (Level + 2)));
        }
    }
    int stats;
    public int Stats
    {
        get
        {
            return stats;

        }
        set
        {
            this.stats = value;
        }
    }

    public int NextStats;

    public override void Start()
    {
        base.Start();
        Stats = Mathf.CeilToInt(Mathf.Sqrt(SetPriceByRarity() * item.itemLevel * (Level + 1)));
        NextStats = Mathf.CeilToInt(Mathf.Sqrt(SetPriceByRarity() * item.itemLevel * (Level + 2)));
        if (item.countable)
        {
            InventorySlot slot = GetComponentInParent<InventorySlot>();
            if (slot)
            {
                slot.CountText(true);
            }

            levelText.gameObject.SetActive(false);
        }

    }



}
