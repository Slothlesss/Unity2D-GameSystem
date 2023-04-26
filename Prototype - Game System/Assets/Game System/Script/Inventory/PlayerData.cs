using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
[CreateAssetMenu(fileName = "new data", menuName = "PlayerStat Data")]
public class PlayerData : ScriptableObject
{
    public int level;
    public int gold;
    public int diamond;
    public ItemInfo.ItemStat.Stat[] baseStats;
    public ItemInfo.ItemStat.Stat[] additionalStats;

    public static void SaveData(PlayerData data)
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

    public static PlayerData LoadData()
    {
        string path = Application.persistentDataPath + "/playerData.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerDataSaveData saveData = JsonUtility.FromJson<PlayerDataSaveData>(json);

            PlayerData data = ScriptableObject.CreateInstance<PlayerData>();
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




#if UNITY_EDITOR
[CustomEditor(typeof(PlayerData))]
public class PlayerDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        PlayerData playerData = (PlayerData)target;

        GUIStyle style = new GUIStyle(GUI.skin.button);
        style.fontSize = 14;

        if (GUILayout.Button("Clear all baseStats value", style, GUILayout.Height(30)))
        {
            ClearAdditionalStats(playerData.baseStats);
        }

        if (GUILayout.Button("Clear all additionalStats value", style, GUILayout.Height(30)))
        {
            ClearAdditionalStats(playerData.additionalStats);
        }


    }

    private void ClearAdditionalStats(ItemInfo.ItemStat.Stat[] stats)
    {
        for (int i = 0; i < stats.Length; i++)
        {
            stats[i].value = 0;
        }
    }
}
#endif
