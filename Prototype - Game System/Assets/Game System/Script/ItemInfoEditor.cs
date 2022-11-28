using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemInfo))]
public class ItemInfoEditor : Editor
{
    ItemInfo itemInfo;

    private void OnEnable()
    {
        itemInfo = target as ItemInfo;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (itemInfo.image == null)
            return;
        Texture2D texture = AssetPreview.GetAssetPreview(itemInfo.image);
        GUILayout.Label("", GUILayout.Height(80), GUILayout.Width(80));
        GUI.DrawTexture(GUILayoutUtility.GetLastRect(), texture);
    }
}


