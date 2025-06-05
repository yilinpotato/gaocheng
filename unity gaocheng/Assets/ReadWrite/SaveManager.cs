using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
public class SaveManager : MonoBehaviour
{
    // 单例模式
    public static SaveManager Instance { get; private set; }

    // 存档目录
    private string saveDirectory;

    // 存档文件扩展名
    private const string SAVE_EXTENSION = ".json";

    // 当前存档ID计数器
    private int currentSaveIdCounter = 0;

    void Awake()
    {
        // 单例模式实现
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // 初始化存档目录
            saveDirectory = Path.Combine(Application.persistentDataPath, "Saves");

            // 确保存档目录存在
            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }

            // 初始化存档ID计数器
            InitializeSaveIdCounter();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 初始化存档ID计数器，找到已有存档中的最大ID
    private void InitializeSaveIdCounter()
    {
        int maxId = 0;

        if (Directory.Exists(saveDirectory))
        {
            string[] saveFiles = Directory.GetFiles(saveDirectory, "*" + SAVE_EXTENSION);

            foreach (string file in saveFiles)
            {
                try
                {
                    string fileName = Path.GetFileNameWithoutExtension(file);
                    if (fileName.StartsWith("save_"))
                    {
                        string idStr = fileName.Substring(5); // 删除 "save_" 前缀
                        if (int.TryParse(idStr, out int id) && id > maxId)
                        {
                            maxId = id;
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"处理存档文件名失败: {file}, 错误: {e.Message}");
                }
            }
        }

        currentSaveIdCounter = maxId + 1;
        Debug.Log($"存档ID计数器初始化为: {currentSaveIdCounter}");
    }

    // 保存游戏数据
    public int SaveGame(PlayerData playerData, bool createNewSave = false)
    {
        // 如果要创建新存档或者没有指定存档ID
        if (createNewSave || playerData.saveId == 0)
        {
            playerData.saveId = currentSaveIdCounter++;
        }

        // 更新游戏时间
        playerData.playTime += Time.time; // 简单累加当前会话时间

        string savePath = Path.Combine(saveDirectory, $"save_{playerData.saveId}{SAVE_EXTENSION}");

        try
        {
            // 将玩家数据序列化为JSON
            string json = JsonUtility.ToJson(playerData, true);
            File.WriteAllText(savePath, json);

            Debug.Log($"游戏已保存到: {savePath}");

            // 可以在这里发送事件通知游戏状态更新
            // EventManager.TriggerEvent("OnGameSaved", playerData);

            return playerData.saveId;
        }
        catch (Exception e)
        {
            Debug.LogError($"保存游戏失败: {e.Message}");
            return -1;
        }
    }

    // 创建新游戏存档
    public int CreateNewSave(string playerName)
    {
        PlayerData newData = new PlayerData
        {
            playerName = playerName,
            saveId = currentSaveIdCounter,
            level = 1,
            health = 100f,
            playTime = 0f,
            money = 100,
            score = 0
        };

        return SaveGame(newData, true);
    }

    // 快速保存当前游戏
    public int QuickSave(PlayerData currentData)
    {
        if (currentData == null)
        {
            Debug.LogError("无法快速保存：当前游戏数据为空");
            return -1;
        }

        // 使用已有存档ID进行保存
        return SaveGame(currentData, false);
    }

    // 获取当前时间的格式化字符串，可用于临时存档名
    private string GetTimeStamp()
    {
        return DateTime.Now.ToString("yyyyMMdd_HHmmss");
    }

    // 创建自动存档
    public int CreateAutoSave(PlayerData currentData)
    {
        if (currentData == null)
        {
            Debug.LogError("无法创建自动存档：当前游戏数据为空");
            return -1;
        }

        // 复制当前数据并修改为自动存档
        PlayerData autoSaveData = new PlayerData(
            currentSaveIdCounter,
            $"{currentData.playerName}_自动存档",
            currentData.level,
            currentData.health,
            currentData.playTime,
            currentData.money,
            currentData.score
        );

        return SaveGame(autoSaveData, true);
    }

    // 保存游戏状态到临时文件(用于场景切换等)
    public void SaveGameState(PlayerData currentData)
    {
        if (currentData == null)
        {
            Debug.LogError("无法保存游戏状态：当前游戏数据为空");
            return;
        }

        string tempPath = Path.Combine(saveDirectory, "temp_state" + SAVE_EXTENSION);

        try
        {
            // 将玩家数据序列化为JSON
            string json = JsonUtility.ToJson(currentData, true);
            File.WriteAllText(tempPath, json);

            Debug.Log("游戏状态已临时保存");
        }
        catch (Exception e)
        {
            Debug.LogError($"保存游戏状态失败: {e.Message}");
        }
    }

    // 加载临时游戏状态
    public PlayerData LoadGameState()
    {
        string tempPath = Path.Combine(saveDirectory, "temp_state" + SAVE_EXTENSION);

        if (File.Exists(tempPath))
        {
            try
            {
                string json = File.ReadAllText(tempPath);
                return JsonUtility.FromJson<PlayerData>(json);
            }
            catch (Exception e)
            {
                Debug.LogError($"加载游戏状态失败: {e.Message}");
            }
        }

        return null;
    }

    // 覆盖已有存档
    public bool OverwriteSave(int saveId, PlayerData newData)
    {
        if (saveId <= 0)
        {
            Debug.LogError("无法覆盖存档：无效的存档ID");
            return false;
        }

        newData.saveId = saveId;
        return SaveGame(newData, false) > 0;
    }
}
