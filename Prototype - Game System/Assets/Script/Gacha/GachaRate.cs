using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GachaRate
{
    public string rarity;
    [Range(1, 100)]
    public int rate;
    public cardInfo[] reward;
}
