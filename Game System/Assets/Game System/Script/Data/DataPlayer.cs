using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
[CreateAssetMenu(fileName = "new data", menuName = "DataPlayer")]
public class DataPlayer : ScriptableObject
{
    public int level;
    public int gold;
    public int diamond;
    public ItemInfo.ItemStat.Stat[] baseStats;
    public ItemInfo.ItemStat.Stat[] additionalStats;

    public void ResetAllData()
    {
        level = 1;
        gold = 1000000;
        diamond = 100000;
        for (int i = 0; i < additionalStats.Length; i++)
        {
            additionalStats[i].value = 0;
        }
        File.Delete(Application.persistentDataPath + "/playerData.json");
    }
    public static void SaveData(DataPlayer data)
    {
        PlayerDataSaveData saveData = new PlayerDataSaveData();
        saveData.level = data.level;
        saveData.gold = data.gold;
        saveData.diamond = data.diamond;
        saveData.baseStats = data.baseStats;
        saveData.additionalStats = data.additionalStats;

        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(Application.persistentDataPath + "/playerData.json", json);
    }

    public static DataPlayer LoadData()
    {
        string path = Application.persistentDataPath + "/playerData.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerDataSaveData saveData = JsonUtility.FromJson<PlayerDataSaveData>(json);

            DataPlayer data = ScriptableObject.CreateInstance<DataPlayer>();
            data.level = saveData.level;
            data.gold = saveData.gold;
            data.diamond = saveData.diamond;
            data.baseStats = saveData.baseStats;
            data.additionalStats = saveData.additionalStats;

            return data;
        }
        else
        {
            return null;
        }
    }
}
[System.Serializable]
public class PlayerDataSaveData
{
    public int level;
    public int gold;
    public int diamond;
    public ItemInfo.ItemStat.Stat[] baseStats;
    public ItemInfo.ItemStat.Stat[] additionalStats;
}


//This is used for make some buttons in the inspector

#if UNITY_EDITOR
[CustomEditor(typeof(DataPlayer))]
public class PlayerDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DataPlayer playerData = (DataPlayer)target;
        GUIStyle style = new GUIStyle(GUI.skin.button);
        style.fontSize = 14;



        if (GUILayout.Button("Set Base Stats To Zero", style, GUILayout.Height(30)))
        {
            SetAdditionalStatsToZero(playerData.baseStats);
        }

        if (GUILayout.Button("Set Additional Stats To Zero", style, GUILayout.Height(30)))
        {
            SetAdditionalStatsToZero(playerData.additionalStats);
        }
    }

    private void SetAdditionalStatsToZero(ItemInfo.ItemStat.Stat[] stats)
    {
        for (int i = 0; i < stats.Length; i++)
        {
            stats[i].value = 0;
        }
    }
}
#endif
