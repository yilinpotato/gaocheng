using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // 当前玩家数据
    private PlayerData currentPlayerData;

    public string LastMapSceneName { get; set; }

    public MapManager mapManager;

    public static GameController Instance { get; private set; }

    // 随机种子
    public int RandomSeed { get; private set; }

    // Start is called before the first frame update  
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // 生成随机种子
        RandomSeed = GenerateRandomSeed();
        Debug.Log("随机种子: " + RandomSeed);

        // 只有在地图未生成的情况下才生成地图
        if (!MapManager.isMapGenerated)
        {
            mapManager.GenerateMap();
        }
        else
        {
            Debug.Log("地图已存在，不再重新生成");
        }
    }

    // Update is called once per frame  
    void Update()
    {

    }

    public void StartNewGame(string playerName)
    {
        // 创建新存档
        int saveId = SaveManager.Instance.CreateNewSave(playerName);

        // 加载新创建的存档
        if (saveId > 0)
        {
            currentPlayerData = LoadManager.Instance.LoadGame(saveId);
            // 初始化游戏状态...
        }
    }

    // 保存当前游戏
    public void SaveCurrentGame()
    {
        if (currentPlayerData != null)
        {
            // 更新玩家数据（如果需要）
            UpdatePlayerDataBeforeSave();

            // 保存游戏
            SaveManager.Instance.QuickSave(currentPlayerData);
        }
    }
    // 生成随机种子的方法
    private int GenerateRandomSeed()
    {
        return System.DateTime.Now.Millisecond; // 使用当前时间生成种子
    }

    // 加载游戏
    public void LoadGame(int saveId)
    {
        currentPlayerData = LoadManager.Instance.LoadGame(saveId);
        if (currentPlayerData != null)
        {
            // 应用加载的数据到游戏中
            ApplyLoadedData();
        }
    }
    // 在保存前更新玩家数据
    public void UpdatePlayerDataBeforeSave()
    {
        if (currentPlayerData == null)
        {
            Debug.LogWarning("无法更新玩家数据：当前玩家数据为空");
            return;
        }

        // 更新游戏时间
        currentPlayerData.playTime += Time.time;

        // 这里可以添加更新玩家其他数据的代码
        // 例如血量、金钱、得分等

        Debug.Log($"已更新玩家 {currentPlayerData.playerName} 的数据以准备保存");
    }

    // 应用加载的数据
    public void ApplyLoadedData()
    {
        if (currentPlayerData == null)
        {
            Debug.LogWarning("无法应用玩家数据：当前玩家数据为空");
            return;
        }

        // 这里可以添加将加载的数据应用到游戏对象的代码
        // 例如设置玩家血量、金钱、得分等

        Debug.Log($"已应用玩家 {currentPlayerData.playerName} 的存档数据");
    }
}
