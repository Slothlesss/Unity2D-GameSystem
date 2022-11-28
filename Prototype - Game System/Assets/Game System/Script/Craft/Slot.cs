using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Slot : MonoBehaviour
{
    public Cards card;
    public int index;
    Image img;
    // Update is called once per frame
    private void Start()
    {
        img = GetComponent<Image>();
    }
    void Update()
    {
        if (card != null)
        {
            img.sprite = card.card.image;
            img.color = card.card.color;
        }
    }
}

