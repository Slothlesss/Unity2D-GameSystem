using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftRequirement : MonoBehaviour
{
    [System.Serializable]
    public struct RequireMaterial
    {
        public ItemInfo info;
        public int amount;
    }
    public GameObject craftSlotPool;
    public CraftSlot[] craftSlots;
    public GameObject resultSlot;
    public RequireMaterial[] requireMaterial;
    public ItemInfo resultItem;
    public int numMeetRequirement;


    private InventoryItem item;
    private void OnEnable()
    {
        craftSlots = craftSlotPool.GetComponentsInChildren<CraftSlot>();
        item = resultSlot.GetComponent<InventoryItem>();
        item.data.info = resultItem; //Update the image of result
        UpdateCraftSlot();
    }

    public void UpdateCraftSlot()
    {
        numMeetRequirement = 0;
        for (int i = 0; i < requireMaterial.Length; i++)
        {
            int requireAmount = requireMaterial[i].amount;
            int currentAmount = InventoryManager.Instance.GetAmountOfItem(requireMaterial[i].info);
            craftSlots[i].ShowRequirement(currentAmount, requireAmount, requireMaterial[i].info);
            if (currentAmount >= requireAmount)
            {
                numMeetRequirement++;
            }
        }
        for (int i = requireMaterial.Length; i < 4; i++)
        {
            craftSlots[i].DisplayLock();
        }
    }

    public void Craft()
    {
        for(int i = 0; i < requireMaterial.Length; i++)
        {
            InventoryManager.Instance.RemoveAmountOfItem(requireMaterial[i].info, requireMaterial[i].amount);
        }
        InventoryManager.Instance.AddAmountOfItem(item.data, 1);
    }

    public bool CanCraft()
    {
        return numMeetRequirement == requireMaterial.Length;
    }


}
