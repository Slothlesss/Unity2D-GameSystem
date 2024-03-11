using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This class provides functions for calculating combat power.
 */
public class CPCalculator: MonoBehaviour
{
    public const long CP_ATK = 20;
    public const long CP_ATK_SPD = 0;
    public const long CP_ATK_RANGE = 0;
    public const long CP_HP = 4;
    public const long CP_MAG_DEF = 10;
    public const long CP_PHY_DEF = 10;

    public long CalculateStatCP(StatType statType, float itemStat)
    {
        long multiplier = 1;
        switch (statType)
        {
            case StatType.Attack:
                multiplier = CP_ATK;
                break;
            case StatType.AttackSpeed:
                multiplier = CP_ATK_SPD;
                break;
            case StatType.AttackRange:
                multiplier = CP_ATK_RANGE;
                break;
            case StatType.Health:
                multiplier = CP_HP;
                break;
            case StatType.MagicalDefense:
                multiplier = CP_MAG_DEF;
                break;
            case StatType.PhysicalDefense:
                multiplier = CP_PHY_DEF;
                break;
        }
        return (long)(itemStat * multiplier);
    }
    public long GetItemCP(InventoryItem item)
    {
        int statLen = item.data.currentStat.Length;
        long totalCP = 0;
        for (int i = 0; i < statLen; i++)
        {
            StatType itemType = item.data.currentStat[i].type;
            float itemStat = item.data.currentStat[i].value;
            totalCP += CalculateStatCP(itemType, itemStat);
        }
        return totalCP;
    }
}
