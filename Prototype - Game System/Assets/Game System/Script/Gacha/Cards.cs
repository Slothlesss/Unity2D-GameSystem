using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Cards : MonoBehaviour
{
    public cardInfo card;
    [SerializeField] Image img;
    // Start is called before the first frame update
    void Start()
    {
        if(card != null)
        {
            img.sprite = card.image;
            img.color = card.color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Start();
    }
}
