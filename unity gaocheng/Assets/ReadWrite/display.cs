using UnityEngine;
using UnityEngine.UI; // 添加UI命名空间

public class display : MonoBehaviour
{
    [Header("UI文本组件引用")]
    public Text totalTimeText;  // 拖拽TotalTime txt组件到这里
    public Text totalScoreText; // 拖拽TotalScore txt组件到这里

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 检查UI组件引用
        Debug.Log($"[UI检查] totalTimeText: {(totalTimeText != null ? "已设置" : "未设置")}");
        Debug.Log($"[UI检查] totalScoreText: {(totalScoreText != null ? "已设置" : "未设置")}");
        
        // 读取并显示玩家信息
        DisplayPlayerInfo();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DisplayPlayerInfo()
    {
        try
        {
            // 确保LoadManager实例存在
            if (LoadManager.Instance == null)
            {
                Debug.LogError("LoadManager实例不存在！请确保场景中有LoadManager组件。");
                return;
            }

            // 获取所有存档信息
            var saveInfoList = LoadManager.Instance.GetAllSaves();
            Debug.Log("开始获取存档信息...");
            
            if (saveInfoList.Count > 0)
            {
                Debug.Log("=== 所有存档信息 ===");
                foreach (var saveInfo in saveInfoList)
                {
                    Debug.Log($"存档ID: {saveInfo.saveId}");
                    Debug.Log($"玩家姓名: {saveInfo.playerName}");
                    Debug.Log($"等级: {saveInfo.level}");
                    Debug.Log($"游戏时间: {saveInfo.playTime:F2} 秒");
                    Debug.Log($"保存时间: {saveInfo.saveDateTime:yyyy-MM-dd HH:mm:ss}");
                    Debug.Log("-----------------");
                }

                // 加载最新存档的详细信息
                PlayerData latestPlayerData = LoadManager.Instance.LoadLatestSave();
                if (latestPlayerData != null)
                {
                    DisplayDetailedPlayerData(latestPlayerData);
                    // 使用PlayerData更新UI
                    UpdateUITextsFromPlayerData(latestPlayerData);
                }
                else
                {
                    // 如果无法加载详细数据，尝试使用存档信息
                    var firstSave = saveInfoList[0];
                    UpdateUIFromSaveInfo(firstSave);
                }
            }
            else
            {
                Debug.Log("没有找到任何存档文件");
                // 设置默认文本
                SetDefaultUI();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"读取玩家信息时发生错误: {e.Message}");
        }
    }

    // 从存档信息更新UI（避免使用dynamic）
    private void UpdateUIFromSaveInfo(object saveInfo)
    {
        try
        {
            // 使用反射获取playTime属性
            var playTimeProperty = saveInfo.GetType().GetProperty("playTime");
            if (playTimeProperty != null)
            {
                float playTime = (float)playTimeProperty.GetValue(saveInfo);
                Debug.Log($"[UI更新] 开始更新UI，playTime: {playTime}");
                
                // 更新TotalTime文本
                if (totalTimeText != null)
                {
                    totalTimeText.text = $"{playTime:F1}秒";
                    Debug.Log($"[UI更新] TotalTime更新为: {playTime:F1}秒");
                }
                else
                {
                    Debug.LogError("[UI更新] totalTimeText为null！");
                }
                
                // 更新TotalScore文本
                if (totalScoreText != null)
                {
                    float randomMultiplier = Random.Range(0.8f, 1.2f);
                    float score = playTime * randomMultiplier;
                    totalScoreText.text = $"{score:F0}";
                    Debug.Log($"[UI更新] TotalScore更新为: {score:F0}");
                }
                else
                {
                    Debug.LogError("[UI更新] totalScoreText为null！");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[UI更新] 更新UI时发生错误: {e.Message}");
        }
    }

    // 设置默认UI
    private void SetDefaultUI()
    {
        if (totalTimeText != null)
            totalTimeText.text = "0.0秒";
        
        if (totalScoreText != null)
            totalScoreText.text = "0";
    }

    // 更新UI文本（从PlayerData）
    private void UpdateUITextsFromPlayerData(PlayerData playerData)
    {
        if (playerData != null)
        {
            Debug.Log($"[UI更新] 使用PlayerData更新UI，playTime: {playerData.playTime}");
            
            // 更新TotalTime文本
            if (totalTimeText != null)
            {
                totalTimeText.text = $"{playerData.playTime:F1}秒";
                Debug.Log($"[UI更新] TotalTime更新为: {playerData.playTime:F1}秒");
            }
            else
            {
                Debug.LogError("[UI更新] totalTimeText为null！");
            }
            
            // 更新TotalScore文本（playTime * 0.8到1.2的随机数）
            if (totalScoreText != null)
            {
                float randomMultiplier = Random.Range(8f, 12f);
                float score = playerData.playTime * randomMultiplier;
                totalScoreText.text = $"{score:F0}";
                Debug.Log($"[UI更新] TotalScore更新为: {score:F0}");
            }
            else
            {
                Debug.LogError("[UI更新] totalScoreText为null！");
            }
        }
    }

    private void DisplayDetailedPlayerData(PlayerData playerData)
    {
        Debug.Log("=====================");
        Debug.Log($"存档ID: {playerData.saveId}");
        Debug.Log($"玩家姓名: {playerData.playerName}");
        Debug.Log($"等级: {playerData.level}");
        Debug.Log($"生命值: {playerData.health}");
        Debug.Log($"游戏时间: {playerData.playTime:F2} 秒 ({playerData.playTime / 3600:F2} 小时)");
        Debug.Log($"金钱: {playerData.money}");
        Debug.Log($"分数: {playerData.score}");
        Debug.Log("=====================");
    }
}
