using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "new card", menuName = "Character")]
public class cardInfo : ScriptableObject
{
    public Sprite image;
    public Color color;
    public string itemName;
    //You can consider it as materials
}
