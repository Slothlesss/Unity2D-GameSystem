using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ShopManager : MonoBehaviour
{
    public enum Currency { Gold, Diamond }
    private int diamond;
    private int gold;

    [SerializeField] TextMeshProUGUI diamondText;
    [SerializeField] TextMeshProUGUI goldText;

    public int Diamond
    {
        get
        {
            return diamond;
        }
        set
        {
            this.diamond = value;
            diamondText.text = value.ToString();
        }
    }
    public int Gold
    {
        get
        {
            return gold;
        }
        set
        {
            this.gold = value;
            goldText.text = value.ToString();
        }
    }
    public Sprite[] currencySprites;
    public static ShopManager Instance;
    public GameObject[] shopMenu;

    public ShopItem[] shopSlots;
    [System.Serializable]
    public struct dailyShopItemType //Weapon, Scroll, ...
    {
        public string name;
        public ItemInfo[] itemInfos; //Kinds of Weapon
    }

    public dailyShopItemType[] shopDailyItems;

    public void Awake()
    {
        Instance = this;
        Diamond = 500;
        Gold = 10000;
    }

    public void Start()
    {
        DisplayMenu(0);
        Refresh();
    }

    public void BuyItem(ShopItem item)
    {
        if (item.Attempt < item.maxAttempt)
        {
            if (item.typeCurrency == Currency.Gold && Gold >= item.price)
            {
                Gold -= item.price;
                item.Attempt++;
                InventoryManager.Instance.AddItem(item.item);
            }
            else if (item.typeCurrency == Currency.Diamond && Diamond >= item.price)
            {
                Diamond -= item.price;
                item.Attempt++;
                InventoryManager.Instance.AddItem(item.item);
            }
        }
        if(item.Attempt >= item.maxAttempt)
        {
            item.DisableBuyButton();
        }
    }

    public void DisplayMenu(int idx)
    {
        foreach(GameObject shop in shopMenu)
        {
            shop.SetActive(false);
        }
        shopMenu[idx].SetActive(true);

        shopSlots = shopMenu[idx].GetComponentsInChildren<ShopItem>();
        
    }

    public void Refresh()
    {
        for (int i = 0; i < shopSlots.Length; i++)
        {
            int randType = Random.Range(0, shopDailyItems.Length);
            int ranLevel = Random.Range(0, shopDailyItems[randType].itemInfos.Length);
            shopSlots[i].item = shopDailyItems[randType].itemInfos[ranLevel];
            int ranCurrency = Random.Range(0, 2);
            shopSlots[i].typeCurrency = ranCurrency == 0 ? Currency.Gold : Currency.Diamond;
            shopSlots[i].Attempt = 0;
            shopSlots[i].EnableBuyButton();

        }
    }


}
