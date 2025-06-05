using UnityEngine;

public class display : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
            Debug.Log("ttttttt ");
            if (saveInfoList.Count > 0)
            {
                Debug.Log("=== 所有存档信息 ===");
                foreach (var saveInfo in saveInfoList)
                {
                    Debug.Log($"存档ID: {saveInfo.saveId}");
                    Debug.Log($"玩家姓名: {saveInfo.playerName}");
                    Debug.Log($"等级: {saveInfo.level}");
                    Debug.Log($"游戏时间: {saveInfo.playTime:F2} 小时");
                    Debug.Log($"保存时间: {saveInfo.saveDateTime:yyyy-MM-dd HH:mm:ss}");
                    Debug.Log("-----------------");
                }

                // 加载并显示最新存档的详细信息
                Debug.Log("=== 最新存档详细信息 ===");
                PlayerData latestPlayerData = LoadManager.Instance.LoadLatestSave();
                
                if (latestPlayerData != null)
                {
                    DisplayDetailedPlayerData(latestPlayerData);
                }
                else
                {
                    Debug.LogWarning("无法加载最新存档的详细数据");
                }
            }
            else
            {
                Debug.Log("没有找到任何存档文件");
                
                // 如果没有存档，尝试检查当前加载的玩家数据
                PlayerData currentData = LoadManager.Instance.GetCurrentPlayerData();
                if (currentData != null)
                {
                    Debug.Log("=== 当前玩家数据 ===");
                    DisplayDetailedPlayerData(currentData);
                }
                else
                {
                    Debug.Log("当前也没有加载任何玩家数据");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"读取玩家信息时发生错误: {e.Message}");
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
