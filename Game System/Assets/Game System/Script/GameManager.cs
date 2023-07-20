using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class GameManager : Singleton<GameManager>
{
    public GameObject systemPool;
    public GameObject optionButtonPool;
    Dictionary<string, UISystemView> viewDic = new Dictionary<string, UISystemView>();
    Toggle ActiveToggle;
    private void Awake()
    {

        UISystemView[] systemViews = systemPool.GetComponentsInChildren<UISystemView>();
        foreach (UISystemView view in systemViews)
        {
            viewDic.Add(view.name, view);
            view.gameObject.SetActive(false);
        }
        ActiveToggle = optionButtonPool.GetComponentsInChildren<Toggle>().Single(i => i.isOn);
        viewDic[ActiveToggle.name].gameObject.SetActive(true);
       
    }
    private void Update()
    {
        ActiveToggle = optionButtonPool.GetComponentsInChildren<Toggle>().Single(i => i.isOn);
        viewDic[ActiveToggle.name].gameObject.SetActive(true);
    }


    public void DisplaySystem()
    {
        foreach (KeyValuePair<string, UISystemView> view in viewDic)
        {
            view.Value.gameObject.SetActive(false);
        }

    }


}
