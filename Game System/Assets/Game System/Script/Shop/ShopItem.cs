using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItem : ItemBase
{
    [SerializeField] Image currencyImage;
    public int price;
    public Currency typeCurrency;
    public TextMeshProUGUI priceText;

    public int maxAttempt = 1;
    private int attempt;
    public int Attempt
    {
        get
        {
            return attempt;
        }
        set
        {
            this.attempt = value;
            attemptText.text = "Attempt: " + attempt.ToString() + "/" + maxAttempt.ToString();
        }
    }
    [SerializeField] TextMeshProUGUI attemptText;

    [SerializeField] Image buttonImage;
    public override void Start()
    {
        base.Start();
        if (typeCurrency == Currency.Gold)
        {
            currencyImage.sprite = ShopManager.Instance.currencySprites[0];
            price = base.data.info.baseStat.requiredLevel * 10 * base.SetPriceByRarity();
            priceText.text = price.ToString();
        }
        else
        {
            currencyImage.sprite = ShopManager.Instance.currencySprites[1];
            price = base.data.info.baseStat.requiredLevel * base.SetPriceByRarity();
            priceText.text = price.ToString();
        }
    }
    

    public void EnableBuyButton()
    {
        buttonImage.color = Color.yellow;
    }

    public void DisableBuyButton()
    {
        buttonImage.color = Color.grey;
    }


}
