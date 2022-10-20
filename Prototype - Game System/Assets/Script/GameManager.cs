using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public static GameManager Instance;
    public ItemCount[] itemCounts;
    private void Awake()
    {
        Instance = this;
    }

    public void ReceiveItem(string nameItem)
    {
        for(int i = 0; i < itemCounts.Length; i++)
        {
            if(itemCounts[i].name == nameItem)
            {
                itemCounts[i].Count++;
            }
        }
    }
    public int NumberItem(string nameItem)
    {
        for (int i = 0; i < itemCounts.Length; i++)
        {
            if (itemCounts[i].name == nameItem)
            {
                return itemCounts[i].Count;
            }
        }
        return 0;
    }
    public void DecreaseItem(string nameItem)
    {
        for (int i = 0; i < itemCounts.Length; i++)
        {
            if (itemCounts[i].name == nameItem)
            {
                itemCounts[i].Count--;
            }
        }
    }

}
