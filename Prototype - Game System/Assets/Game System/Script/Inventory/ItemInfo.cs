using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CreateAssetMenu(fileName = "new Item", menuName = "Equipment")]


public class ItemInfo : ScriptableObject
{
    public Sprite image;

    public Color color;
    public bool countable;
    public string itemName;
    public int itemLevel;
    public ItemManager.ItemType typeOfItem;
    public ItemManager.Rarity rarity;

    [CustomEditor(typeof(ItemInfo))]
    public class ItemInfoEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // Reference the variables in the script
            ItemInfo script = (ItemInfo)target;

            /*if (script.a)
            {
                // Ensure the label and the value are on the same line
                EditorGUILayout.BeginHorizontal();

                // A label that says "b" (change b to B if you want it uppercase like default) and restrict its length.
                // You can change 50 to any other value
                EditorGUILayout.LabelField("b", GUILayout.MaxWidth(50));

                // Show and save the value of b
                script.b = EditorGUILayout.IntField(script.b);
                // If you would like to restrict the length of the int field, replace the above line with this one:
                // script.b = EditorGUILayout.IntField(script.b, GUILayout.MaxWidth(50)); // (or any other value other than 50)

                EditorGUILayout.EndHorizontal();
            }*/
        }


    }
}
