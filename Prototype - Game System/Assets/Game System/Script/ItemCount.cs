using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class ItemCount
{
    [HideInInspector] public string name;
    public TextMeshProUGUI itemTextBox;
    private int count;
    public int Count
    {
        get { return count; }
        set
        {
            count = value;
            itemTextBox.text = value.ToString();
        }
    }
}
