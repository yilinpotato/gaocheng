using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
public class LoadManager : MonoBehaviour
{
    // 单例模式
    public static LoadManager Instance { get; private set; }

    // 存档目录
    private string saveDirectory;
    // 存档文件扩展名
    private const string SAVE_EXTENSION = ".json";

    // 当前加载的玩家数据
    private PlayerData currentPlayerData;

    // 存档信息列表（用于存档选择界面）
    private List<SaveInfo> saveInfoList = new List<SaveInfo>();

    // 存档信息类
    [Serializable]
    public class SaveInfo
    {
        public int saveId;
        public string playerName;
        public DateTime saveDateTime;
        public float playTime;
        public int level;

        public SaveInfo(PlayerData data)
        {
            this.saveId = data.saveId;
            this.playerName = data.playerName;
            this.saveDateTime = DateTime.Now;
            this.playTime = data.playTime;
            this.level = data.level;
        }
    }

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

            // 加载所有存档信息
            RefreshSaveList();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 刷新存档列表
    public void RefreshSaveList()
    {
        saveInfoList.Clear();

        // 检查存档目录下的所有文件
        if (Directory.Exists(saveDirectory))
        {
            string[] saveFiles = Directory.GetFiles(saveDirectory, "*" + SAVE_EXTENSION);

            foreach (string file in saveFiles)
            {
                try
                {
                    // 读取文件内容
                    string json = File.ReadAllText(file);
                    PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);

                    if (playerData != null)
                    {
                        SaveInfo info = new SaveInfo(playerData);
                        saveInfoList.Add(info);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"加载存档信息失败: {file}, 错误: {e.Message}");
                }
            }

            // 按存档ID排序
            saveInfoList = saveInfoList.OrderByDescending(x => x.saveDateTime).ToList();
        }
    }

    // 获取所有存档信息
    public List<SaveInfo> GetAllSaves()
    {
        RefreshSaveList();
        return saveInfoList;
    }

    // 加载存档
    public PlayerData LoadGame(int saveId)
    {
        string savePath = Path.Combine(saveDirectory, $"save_{saveId}{SAVE_EXTENSION}");

        if (File.Exists(savePath))
        {
            try
            {
                string json = File.ReadAllText(savePath);
                currentPlayerData = JsonUtility.FromJson<PlayerData>(json);

                Debug.Log($"成功加载存档 ID: {saveId}");

                // 可以在这里发送事件通知游戏状态更新
                // EventManager.TriggerEvent("OnGameLoaded", currentPlayerData);

                return currentPlayerData;
            }
            catch (Exception e)
            {
                Debug.LogError($"加载存档 {saveId} 失败: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning($"存档不存在: {saveId}");
        }

        return null;
    }

    // 删除存档
    public bool DeleteSave(int saveId)
    {
        string savePath = Path.Combine(saveDirectory, $"save_{saveId}{SAVE_EXTENSION}");

        if (File.Exists(savePath))
        {
            try
            {
                File.Delete(savePath);
                RefreshSaveList();
                Debug.Log($"成功删除存档 ID: {saveId}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"删除存档 {saveId} 失败: {e.Message}");
            }
        }

        return false;
    }

    // 获取当前加载的玩家数据
    public PlayerData GetCurrentPlayerData()
    {
        return currentPlayerData;
    }

    // 加载最近的存档
    public PlayerData LoadLatestSave()
    {
        RefreshSaveList();

        if (saveInfoList.Count > 0)
        {
            // 获取最新的存档ID
            int latestSaveId = saveInfoList[0].saveId;
            return LoadGame(latestSaveId);
        }

        return null;
    }

    // 判断指定ID的存档是否存在
    public bool DoesSaveExist(int saveId)
    {
        string savePath = Path.Combine(saveDirectory, $"save_{saveId}{SAVE_EXTENSION}");
        return File.Exists(savePath);
    }
}
