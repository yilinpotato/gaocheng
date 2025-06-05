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

    [Header("自动保存设置")]
    [SerializeField] private float autoSaveInterval = 10f; // 自动保存间隔（秒）
    [SerializeField] private bool enableAutoSave = true;   // 是否启用自动保存
    
    private float autoSaveTimer = 0f; // 自动保存计时器

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
            return;
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

        // 初始化自动保存计时器
        autoSaveTimer = autoSaveInterval;
        
        // 临时测试：创建一个测试玩家数据
        if (currentPlayerData == null)
        {
            Debug.Log("[测试] 创建测试玩家数据用于自动保存测试");
            
            try 
            {
                // 使用无参构造函数创建
                currentPlayerData = new PlayerData();
                currentPlayerData.playerName = "测试玩家";
                currentPlayerData.level = 1;
                currentPlayerData.saveId = 999; // 测试用的ID
                
                Debug.Log("[测试] 测试玩家数据创建成功 - 玩家名：" + currentPlayerData.playerName);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[测试] 创建测试玩家数据失败: {e.Message}");
            }
        }

        // 显示存档文件保存路径
        Debug.Log("存档文件保存路径: " + Application.persistentDataPath);
    }

    // Update is called once per frame  
    void Update()
    {
        // 添加调试信息
        if (Time.frameCount % 600 == 0) // 每10秒左右显示一次状态
        {
            Debug.Log($"[自动保存状态] enableAutoSave: {enableAutoSave}, currentPlayerData: {(currentPlayerData != null ? "存在" : "null")}, 计时器: {autoSaveTimer:F1}秒");
        }
        
        // 自动保存逻辑
        if (enableAutoSave && currentPlayerData != null)
        {
            autoSaveTimer -= Time.deltaTime;
            
            if (autoSaveTimer <= 0f)
            {
                Debug.Log("[自动保存] 计时器到达，开始执行自动保存...");
                
                // 执行自动保存
                AutoSave();
                
                // 重置计时器
                autoSaveTimer = autoSaveInterval;
            }
        }
        else if (enableAutoSave && currentPlayerData == null)
        {
            // 只在第一次显示这个警告
            if (Time.frameCount % 300 == 0)
            {
                Debug.LogWarning("[自动保存] 玩家数据为空，无法执行自动保存");
            }
        }
    }

    // 自动保存方法
    private void AutoSave()
    {
        
        try
        {
            Debug.Log("[自动保存] 开始执行自动保存...");
            
            // 确保SaveManager实例存在
            if (SaveManager.Instance == null)
            {
                Debug.LogWarning("[自动保存] SaveManager实例不存在，跳过自动保存");
                return;
            }

            // 保存当前游戏
            SaveCurrentGame();
            
            Debug.Log($"[自动保存] 已自动保存玩家 {currentPlayerData.playerName} 的游戏数据");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[自动保存] 自动保存时发生错误: {e.Message}");
        }
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
            
            // 重置自动保存计时器
            autoSaveTimer = autoSaveInterval;
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

    // 手动保存（可以被按钮或其他UI调用）
    public void ManualSave()
    {
        if (currentPlayerData != null)
        {
            SaveCurrentGame();
            Debug.Log("[手动保存] 游戏已手动保存");
        }
        else
        {
            Debug.LogWarning("[手动保存] 无法手动保存：当前玩家数据为空");
        }
    }

    // 设置自动保存间隔
    public void SetAutoSaveInterval(float interval)
    {
        autoSaveInterval = Mathf.Max(1f, interval); // 最小间隔1秒
        Debug.Log($"自动保存间隔已设置为 {autoSaveInterval} 秒");
    }

    // 启用/禁用自动保存
    public void SetAutoSaveEnabled(bool enabled)
    {
        enableAutoSave = enabled;
        if (enabled)
        {
            autoSaveTimer = autoSaveInterval; // 重置计时器
            Debug.Log("自动保存已启用");
        }
        else
        {
            Debug.Log("自动保存已禁用");
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
            
            // 重置自动保存计时器
            autoSaveTimer = autoSaveInterval;
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
        currentPlayerData.playTime += Time.deltaTime;

        // 这里可以添加更新玩家其他数据的代码
        // 例如血量、金钱、得分等

        // Debug.Log($"已更新玩家 {currentPlayerData.playerName} 的数据以准备保存");
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

    // 当游戏暂停时停止自动保存
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            // 游戏暂停时执行一次保存
            if (currentPlayerData != null)
            {
                SaveCurrentGame();
                Debug.Log("[暂停保存] 游戏暂停时已保存数据");
            }
        }
    }

    // 当应用退出时保存
    void OnApplicationQuit()
    {
        if (currentPlayerData != null)
        {
            SaveCurrentGame();
            Debug.Log("[退出保存] 应用退出时已保存数据");
        }
    }
}
