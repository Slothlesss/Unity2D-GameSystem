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
    public ItemInfo[] recipeResults;
    public InventoryItem resultSlot;

    private void Update()
    {
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
                    nearestSlot.GetComponent<Image>().sprite = currentCard.GetComponent<Image>().sprite;
                    nearestSlot.GetComponent<Image>().color = currentCard.GetComponent<Image>().color;
                    nearestSlot.card = currentCard;
                    cardList[nearestSlot.index] = currentCard;
                }
                else
                {
                    GameManager.Instance.ReceiveItem(currentCard.card.itemName);
                }
                currentCard = null;
                CheckForCreatedRecipes();
            }
        }
    }

    void CheckForCreatedRecipes()
    {
        resultSlot.gameObject.SetActive(false);
        resultSlot.item = null;
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
                //Chi can thay doi ItemInfo
                resultSlot.item = recipeResults[i];
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
    public void OnClickSlot(Slot slot)
    {
        if (slot.card != null)
        {

            GameManager.Instance.ReceiveItem(slot.card.card.itemName);
            slot.card = null;
            cardList[slot.index] = null;
            slot.gameObject.SetActive(false);
            CheckForCreatedRecipes();

        }
    }
    public void Craft()
    {
        if(resultSlot.item != null) //can craft ?
        {
            //InventoryManager.Instance.AddItem();
            GameManager.Instance.ReceiveItem(resultSlot.item.itemName);
            InventoryManager.Instance.AddItem(resultSlot.item);
            resultSlot.item = null;
            resultSlot.gameObject.SetActive(false);
            foreach (Slot slot in craftSlots) //After crafting
            {
                if (slot.card != null)
                {
                    slot.card = null;
                    cardList[slot.index] = null;
                    slot.gameObject.SetActive(false);
                }
            }
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
