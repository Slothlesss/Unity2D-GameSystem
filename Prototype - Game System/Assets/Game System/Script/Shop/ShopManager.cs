using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ShopManager : MonoBehaviour
{
    public enum Currency { Gold, Diamond }
    public PlayerStat playerStat;

    [SerializeField] TextMeshProUGUI diamondText;
    [SerializeField] TextMeshProUGUI goldText;

    public int Diamond
    {
        get
        {
            return playerStat.playerData.diamond;
        }
        set
        {
            playerStat.playerData.diamond = value;
            diamondText.text = value.ToString();
        }
    }
    public int Gold
    {
        get
        {
            return playerStat.playerData.gold;
        }
        set
        {
            playerStat.playerData.gold = value;
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
    [System.Serializable]
    public struct shopContainer
    {
        public string name;
        public dailyShopItemType[] kindOfItem;
    }
    public shopContainer[] shopContainers;


    public List<int> times;





    public void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        Diamond = playerStat.playerData.diamond;
        Gold = playerStat.playerData.gold;
        for (int i = 0; i < shopMenu.Length; i++)
        {
            DisplayMenu(i);
            if (i < 4)
            {
                Refresh(i);
            }
            else
            {
                RefreshCurrency();
            }
        }
        DisplayMenu(0);
    }

    public void BuyItem(ShopItem item)
    {
        if (item.Attempt < item.maxAttempt)
        {
            if (item.data.info.stat.type != ItemManager.ItemType.Currency) //If item is not Gold or Diamond
            {
                if (item.typeCurrency == Currency.Gold && Gold >= item.price)
                {
                    Gold -= item.price;
                    item.Attempt++;
                    InventoryManager.Instance.AddItem(item.data);
                }
                else if (item.typeCurrency == Currency.Diamond && Diamond >= item.price)
                {
                    Diamond -= item.price;
                    item.Attempt++;
                    InventoryManager.Instance.AddItem(item.data);
                }
            }
            else //If item you buy is Gold or Diamond
            {
                if (item.typeCurrency == Currency.Diamond && Diamond >= item.price)
                {
                    Diamond -= item.price;
                    item.Attempt++;
                    Gold += item.price * 10;
                }
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

    public void Refresh(int idx)
    {
        for (int i = 0; i < shopSlots.Length; i++)
        {
            int randKind = Random.Range(0, shopContainers[idx].kindOfItem.Length);
            int ranItem = Random.Range(0, shopContainers[idx].kindOfItem[randKind].itemInfos.Length);
            shopSlots[i].data.info = shopContainers[idx].kindOfItem[randKind].itemInfos[ranItem];
            int ranCurrency = Random.Range(0, 2);
            shopSlots[i].typeCurrency = ranCurrency == 0 ? Currency.Gold : Currency.Diamond;
            shopSlots[i].Attempt = 0;
            shopSlots[i].EnableBuyButton();
        }
    }
    public void RefreshCurrency()
    {
        for(int i = 0; i <shopSlots.Length; i++)
        {
            shopSlots[i].Attempt = 0;
            shopSlots[i].EnableBuyButton();
        }
    }


}
