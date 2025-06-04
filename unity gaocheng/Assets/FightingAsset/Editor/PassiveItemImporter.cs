using UnityEngine;
using UnityEditor;
using System.IO;

public class PassiveItemImporter : MonoBehaviour
{
    [MenuItem("Tools/导入被动道具")]
    public static void ImportPassiveItemsFromJSON()
    {
        string path = "Assets/Resources/PassiveItems.json";
        if (!File.Exists(path))
        {
            Debug.LogError("找不到 PassiveItems.json");
            return;
        }

        string json = File.ReadAllText(path);
        PassiveItemEntry[] entries = JsonHelper.FromJson<PassiveItemEntry>(json);

        foreach (var entry in entries)
        {
            PassiveItem item = ScriptableObject.CreateInstance<PassiveItem>();
            item.itemName = entry.itemName;
            item.description = entry.description;
            item.type = ItemType.Passive;
            item.healthBoost = entry.healthBoost;
            item.speedBoost = entry.speedBoost;
            item.damageBoost = entry.damageBoost;

            string assetPath = $"Assets/Data/Items/{entry.itemName}.asset";
            AssetDatabase.CreateAsset(item, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("成功导入被动道具！");
    }
}

