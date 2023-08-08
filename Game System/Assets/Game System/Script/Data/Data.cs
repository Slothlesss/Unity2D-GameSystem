using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[CreateAssetMenu(fileName = "new data", menuName = "Data")]
public class Data : ScriptableObject
{
    public DataPlayer dataPlayer;
    public DataInventory dataInventory;
}

#if UNITY_EDITOR
[CustomEditor(typeof(Data))]
public class DataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Data data = (Data)target;
        GUIStyle style = new GUIStyle(GUI.skin.button);
        style.fontSize = 14;


        if (GUILayout.Button("Reset All Data", style, GUILayout.Height(30)))
        {
            data.dataInventory.ResetAllData();
            data.dataPlayer.ResetAllData();
        }
    }
}
#endif