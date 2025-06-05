using UnityEngine;
using System.IO;
public class GameManager2 : MonoBehaviour
{
    // 单例模式
    public static GameManager2 Instance { get; private set; }

    void Awake()
    {
        // 实现单例模式
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // 确保存档目录存在
            string saveDirectory = Path.Combine(Application.persistentDataPath, "Saves");
            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }

            // 确保 SaveManager 和 LoadManager 已实例化
            InitializeSaveLoadSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 初始化存档和读档系统
    private void InitializeSaveLoadSystem()
    {
        // 确保 SaveManager 存在
        if (SaveManager.Instance == null)
        {
            GameObject saveManagerObj = new GameObject("SaveManager");
            saveManagerObj.AddComponent<SaveManager>();
            // 保持在场景切换时不被销毁
            DontDestroyOnLoad(saveManagerObj);
        }

        // 确保 LoadManager 存在
        if (LoadManager.Instance == null)
        {
            GameObject loadManagerObj = new GameObject("LoadManager");
            loadManagerObj.AddComponent<LoadManager>();
            // 保持在场景切换时不被销毁
            DontDestroyOnLoad(loadManagerObj);
        }

        Debug.Log("存档系统初始化完成");
    }

    // 提供一个方法从其他脚本获取当前玩家数据
    public PlayerData GetCurrentPlayerData()
    {
        if (LoadManager.Instance != null)
        {
            return LoadManager.Instance.GetCurrentPlayerData();
        }
        return null;
    }

    // 提供一个快速保存方法
    public void QuickSaveGame(PlayerData data)
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.QuickSave(data);
        }
    }
}
