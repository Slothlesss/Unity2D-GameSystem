using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GachaManager : MonoBehaviour
{
    [SerializeField] GachaRate[] gacha;
    [SerializeField] Transform parent;
    [SerializeField] List<TextMeshProUGUI> UIRate;
    [SerializeField] GameObject characterCardGO;
    Cards card;

    GameObject[] characterCards = new GameObject[10];
    private void Awake()
    {
        for(int i = 0; i < gacha.Length; i++ )
        {
            UIRate[i].text = gacha[i].rate.ToString() + "%";
        }
    }
    public void GachaOneTime()
    {
        for (int i = 1; i < characterCards.Length; i++)
        {
            if (characterCards[i] != null)
                characterCards[i].SetActive(false);
        }
        if (characterCards[0] == null)
        {
            characterCards[0] = Instantiate(characterCardGO, parent.position, Quaternion.identity);
            characterCards[0].transform.SetParent(parent);
        }

        card = characterCards[0].GetComponent<Cards>();
        int rnd = UnityEngine.Random.Range(1, 101);
        int totalRate = 0;
        for (int i = 0; i < gacha.Length; i++)
        {
            totalRate += gacha[i].rate;
            if (rnd <= totalRate)
            {
                card.card = Reward(gacha[i].rarity);
                return;
            }
            
        }
    }

    public void GachaTenTime()
    {
        for (int i = 0; i < 10; i++)
        {
            if (characterCards[i] != null)
                characterCards[i].SetActive(true);

            if (characterCards[i] == null)
            {
                characterCards[i] = Instantiate(characterCardGO, parent.position, Quaternion.identity);
                characterCards[i].transform.SetParent(parent);
            }


            card = characterCards[i].GetComponent<Cards>();

            int rnd = UnityEngine.Random.Range(1, 101);
            Debug.Log(rnd);
            int totalRate = 0;
            for (int j = 0; j < gacha.Length; j++)
            {
                totalRate += gacha[j].rate;
                if (rnd <= totalRate)
                {
                    card.card = Reward(gacha[j].rarity);
                    break;
                }
            }
        }
    }

    IEnumerator GachaSpawning()
    {
        yield return null;
    }

    cardInfo Reward(string rarity)
    {
        GachaRate gr = Array.Find(gacha, rt => rt.rarity == rarity);
        cardInfo[] reward = gr.reward;
        int rnd = UnityEngine.Random.Range(0, reward.Length);
        GameManager.Instance.ReceiveItem(reward[rnd].itemName);
        return reward[rnd];
    }
}
