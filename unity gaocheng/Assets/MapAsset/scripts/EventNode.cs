using UnityEngine;
using UnityEngine.SceneManagement;

public class EventNode : Node
{
    [HideInInspector] public EventType eventType;

    protected virtual string EventSceneName => "EventScene";

    public void RandomizeEvent()
    {
        // 暂时固定为Growth事件
        eventType = EventType.Growth;
        
        // 设置全局事件数据
        EventSceneData.currentEventType = eventType;
        
        Debug.Log($"事件类型被强制设定为: {eventType}");
        Debug.Log($"EventSceneData.currentEventType 设置为: {EventSceneData.currentEventType}");
    }

    public void StartEvent()
    {
        Debug.Log("=== StartEvent 开始 ===");
        
        // 强制设置为Growth事件
        RandomizeEvent();
        
        // 显示事件面板
        ShowEventPanel();
    }

    private void ShowEventPanel()
    {
        try
        {
            Debug.Log($"ShowEventPanel - 即将加载场景，事件类型: {EventSceneData.currentEventType}");
            
            // 重要：改为Additive模式，不替换当前场景
            SceneManager.LoadScene("EventScene", LoadSceneMode.Additive);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load event scene: {e.Message}");
            ReturnToMap();
        }
    }

    public void EndEvent()
    {
        Debug.Log($"事件场景结束");
        
        // 卸载事件场景，而不是加载新场景
        try
        {
            SceneManager.UnloadSceneAsync("EventScene");
            Debug.Log("EventScene 已卸载");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"卸载EventScene失败: {e.Message}");
        }
    }

    private void ReturnToMap()
    {
        forTestButton buttonController = FindObjectOfType<forTestButton>();
        if (buttonController != null)
        {
            buttonController.ReturnToMapScene();
        }
        else
        {
            Debug.LogWarning("ForTestButton not found, cannot return to map");
        }
    }
}