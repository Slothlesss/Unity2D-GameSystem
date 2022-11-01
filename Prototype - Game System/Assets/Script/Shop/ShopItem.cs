using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ShopItem : ItemBase
{
    [SerializeField] Image currencyImage;
    public int price;
    public ShopManager.Currency typeCurrency;
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
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        if (typeCurrency == ShopManager.Currency.Gold)
        {
            currencyImage.sprite = ShopManager.Instance.currencySprites[0];
            price = base.item.itemLevel * 10 * SetPriceByRarity();
            priceText.text = price.ToString();
        }
        else
        {
            currencyImage.sprite = ShopManager.Instance.currencySprites[1];
            price = base.item.itemLevel * SetPriceByRarity();
            priceText.text = price.ToString();
        }
    }
    public int SetPriceByRarity()
    {
        if (item.rarity == ItemManager.Rarity.Common)
        {
            return 1;
        }
        else if (item.rarity == ItemManager.Rarity.Uncommon)
        {
            return 5;
        }
        else if (item.rarity == ItemManager.Rarity.Rare)
        {
            return 20;
        }
        else if (item.rarity == ItemManager.Rarity.Epic)
        {
            return 100;
        }
        else if (item.rarity == ItemManager.Rarity.Mythical)
        {
            return 500;
        }
        else if (item.rarity == ItemManager.Rarity.Legendary)
        {
            return 2000;
        }
        else if (item.rarity == ItemManager.Rarity.God)
        {
            return 10000;
        }
        return 0;
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
