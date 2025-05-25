using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonDataManager : MonoBehaviour
{
    private string fileName = "playerdata.json";
    private string FilePath => Path.Combine(Application.persistentDataPath, fileName);

    public PlayerData currentData;

    void Start()
    {
        // 模拟一组数据（游戏实际运行时替换）
        currentData = new PlayerData(
            score: 1500,
            exploredNodes: 12,
            currentHealth: 87.5f,
            sessionSeed: Random.Range(1000, 9999),
            obtainedItems: new List<string> { "Sword", "Health Potion", "Map Fragment" }
        );

        SaveData(currentData); // 保存数据
        PlayerData loaded = LoadData(); // 加载数据

        Debug.Log($"读取成功：Score={loaded.score}, Health={loaded.currentHealth}, Items={string.Join(", ", loaded.obtainedItems)}");
    }

    public void SaveData(PlayerData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(FilePath, json);
        Debug.Log("数据已保存到: " + FilePath);
    }

    public PlayerData LoadData()
    {
        if (File.Exists(FilePath))
        {
            string json = File.ReadAllText(FilePath);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
            return data;
        }
        else
        {
            Debug.LogWarning("没有找到数据文件，返回默认数据");
            return new PlayerData(0, 0, 100f, 0, new List<string>());
        }
    }
}
