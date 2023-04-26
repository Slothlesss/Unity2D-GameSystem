using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftManager : MonoBehaviour
{
    Cards currentCard;
    public Image customCursor;

    public Slot[] craftSlots;
    public List<Cards> cardList;
    public string[] recipes;

    [System.Serializable]
    public struct recipeResult
    {
        public string name;
        public ItemInfo rarityEarned;//Question icon
        public ItemInfo[] type;
    }

    public recipeResult[] itemEarned;
    public InventoryItem resultSlot;
    public InventoryItem resultSlotReal;

    public GameObject CraftSystem;

    int recipeIndex;
    bool canCraft;
    private void Update()
    {
        if(!CraftSystem.activeInHierarchy)
        {
            RemoveAll();
            RemoveResult();
        }
        if(Input.GetMouseButtonUp(0))
        {
            if (currentCard != null)
            {
                customCursor.gameObject.SetActive(false);
                Slot nearestSlot = null;
                float minDistance = 200f; //Only nearby
                foreach (Slot slot in craftSlots)
                {
                    float dist = Vector2.Distance(Input.mousePosition, slot.transform.position);
                    if (dist < minDistance)
                    {
                        nearestSlot = slot;
                        if(slot.card != null)
                        {
                            GameManager.Instance.ReceiveItem(slot.card.card.itemName);
                        }
                        break;
                    }

                    Debug.Log(dist);
                }
                if (nearestSlot != null)
                {
                    nearestSlot.gameObject.SetActive(true);
                    nearestSlot.card = currentCard;
                    cardList[nearestSlot.index] = currentCard;
                }
                else
                {
                    GameManager.Instance.ReceiveItem(currentCard.card.itemName);
                }
                CheckForCreatedRecipes();
                currentCard = null;
            }
        }
    }

    void CheckForCreatedRecipes()
    {
        RemoveResult();
        string currentRecipeString = "";
        foreach(Cards card in cardList)
        {
            if(card != null)
            {
                currentRecipeString += card.card.itemName;
            }
            else
            {
                currentRecipeString += "null";
            }
        }

        for (int i = 0; i < recipes.Length; i++)
        {
            if (recipes[i] == currentRecipeString) //Check recipe
            {
                resultSlot.gameObject.SetActive(true);
                resultSlot.data.info = itemEarned[i].rarityEarned;
                recipeIndex = i;
                canCraft = true;
            }
        }
    }
    public void RemoveAll()
    {
        foreach(Slot slot in craftSlots)
        {
            OnClickSlot(slot);
        }
    }
    public void RemoveResult()
    {
        canCraft = false;
        resultSlot.gameObject.SetActive(false);
        resultSlot.data.info = null;
    }
    public void OnClickSlot(Slot slot)
    {
        if (slot.card != null)
        {
            canCraft = false;
            GameManager.Instance.ReceiveItem(slot.card.card.itemName);
            slot.card = null;
            cardList[slot.index] = null;
            slot.gameObject.SetActive(false);
            CheckForCreatedRecipes();

        }
    }
    public void Craft()
    {
        if(canCraft) //can craft ?
        {
            //GameManager.Instance.ReceiveItem(resultSlot.item.itemName);

            int rand = Random.Range(0, itemEarned[recipeIndex].type.Length);
            resultSlot.data.info = itemEarned[recipeIndex].type[rand];
            InventoryManager.Instance.AddItem(resultSlot.data);
            //resultSlot.item = null;
            //resultSlot.gameObject.SetActive(false);
            foreach (Slot slot in craftSlots) //After crafting
            {
                if (slot.card != null)
                {
                    slot.card = null;
                    cardList[slot.index] = null;
                    slot.gameObject.SetActive(false);
                }
            }
            canCraft = false;
        }
    }
    public void OnMouseDownItem(Cards card)
    {
        if(currentCard == null)
        {
            if (GameManager.Instance.NumberItem(card.card.itemName) > 0)
            {
                currentCard = card;
                customCursor.gameObject.SetActive(true);
                customCursor.sprite = currentCard.GetComponent<Image>().sprite;
                customCursor.color = currentCard.GetComponent<Image>().color;
                GameManager.Instance.DecreaseItem(currentCard.card.itemName);
            }
        }
    }    
}
