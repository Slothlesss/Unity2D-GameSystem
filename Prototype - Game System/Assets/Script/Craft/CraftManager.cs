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
    public Cards[] recipeResults;
    public Slot resultSlot;

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
        resultSlot.card = null;
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
            if (recipes[i] == currentRecipeString)
            {
                resultSlot.gameObject.SetActive(true);
                resultSlot.GetComponent<Image>().sprite = recipeResults[i].GetComponent<Image>().sprite;
                resultSlot.GetComponent<Image>().color = recipeResults[i].GetComponent<Image>().color;
                resultSlot.card = recipeResults[i];
            }
        }
    }
    public void OnClickSlot(Slot slot)
    {
        if(slot.card != null)
        {
            GameManager.Instance.ReceiveItem(slot.card.card.itemName);
            slot.card = null;
            cardList[slot.index] = null;
            slot.gameObject.SetActive(false);
            CheckForCreatedRecipes();
        }
    }
    public void OnMouseDownItem(Cards card)
    {
        if(currentCard == null)
        {
            currentCard = card;
            customCursor.gameObject.SetActive(true);
            customCursor.sprite = currentCard.GetComponent<Image>().sprite;
            customCursor.color = currentCard.GetComponent<Image>().color;
            GameManager.Instance.DecreaseItem(currentCard.card.itemName);
        }
    }    
}
