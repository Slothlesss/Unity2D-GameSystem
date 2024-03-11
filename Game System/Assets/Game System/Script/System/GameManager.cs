using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

/* This class is used for controlling all systems' views
 */
public class GameManager : Singleton<GameManager>
{
    public GameObject systemPool;
    UISystemView[] systemViews;
    private void Awake()
    {
        systemViews = systemPool.GetComponentsInChildren<UISystemView>();
        DisplaySystem(0);
    }
    public void DisplaySystem(int idx)
    {
        for (int i = 0; i < systemViews.Length; i++)
        {
            systemViews[i].gameObject.SetActive(false);
        }
        systemViews[idx].gameObject.SetActive(true);
    }
}
