using UnityEngine;
using System.Collections;

public class EventSceneManager : MonoBehaviour
{
    public GameObject springUI;
    public GameObject growthUI;
    public GameObject rewardUI;

    void Start()
    {
        Debug.Log("=== EventSceneManager Start ===");
        
        // 首先处理重复的EventSystem
        FixDuplicateEventSystems();
        
        // 强制查找并激活Growth UI
        ForceActivateGrowthUI();
        
        // 强制设置为Growth事件类型
        EventSceneData.currentEventType = EventType.Growth;
        Debug.Log($"强制设定事件类型为: {EventSceneData.currentEventType}");
    }

    private void FixDuplicateEventSystems()
    {
        Debug.Log("=== 修复重复的EventSystem ===");
        
        // 查找所有EventSystem
        UnityEngine.EventSystems.EventSystem[] eventSystems = FindObjectsOfType<UnityEngine.EventSystems.EventSystem>();
        
        Debug.Log($"找到 {eventSystems.Length} 个EventSystem");
        
        if (eventSystems.Length > 1)
        {
            Debug.LogWarning("发现多个EventSystem，将删除多余的");
            
            // 保留第一个，删除其余的
            for (int i = 1; i < eventSystems.Length; i++)
            {
                Debug.Log($"删除多余的EventSystem: {eventSystems[i].gameObject.name}");
                DestroyImmediate(eventSystems[i].gameObject);
            }
        }
        
        // 确保剩余的EventSystem是激活的
        if (eventSystems.Length > 0 && eventSystems[0] != null)
        {
            eventSystems[0].gameObject.SetActive(true);
            Debug.Log($"确保EventSystem激活: {eventSystems[0].gameObject.name}");
        }
    }

    private void ForceActivateGrowthUI()
    {
        Debug.Log("=== 强制激活Growth UI ===");
        
        // 方式1：通过引用激活
        if (growthUI != null)
        {
            growthUI.SetActive(true);
            Debug.Log($"通过引用激活 growthUI: {growthUI.name}");
        }
        
        // 方式2：按名称查找并激活
        GameObject[] candidates = {
            GameObject.Find("GrowthEventCanvas"),
            GameObject.Find("GrowthEventController"),
            GameObject.Find("MainPanel")
        };
        
        foreach (GameObject candidate in candidates)
        {
            if (candidate != null)
            {
                candidate.SetActive(true);
                Debug.Log($"按名称激活: {candidate.name}");
                
                // 如果是Canvas，确保子对象也激活
                if (candidate.name.Contains("Canvas"))
                {
                    for (int i = 0; i < candidate.transform.childCount; i++)
                    {
                        candidate.transform.GetChild(i).gameObject.SetActive(true);
                    }
                }
            }
        }
        
        // 方式3：强制激活所有Growth相关对象
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("Growth") || obj.name.Contains("Event"))
            {
                if (!obj.activeInHierarchy)
                {
                    obj.SetActive(true);
                    Debug.Log($"强制激活: {obj.name}");
                }
            }
        }
        
        // 方式4：直接通过组件查找
        GrowthEventManager manager = FindObjectOfType<GrowthEventManager>();
        if (manager != null)
        {
            manager.gameObject.SetActive(true);
            Debug.Log($"通过GrowthEventManager激活: {manager.gameObject.name}");
        }
        
        GrowthEventUIController controller = FindObjectOfType<GrowthEventUIController>();
        if (controller != null)
        {
            controller.gameObject.SetActive(true);
            Debug.Log($"通过GrowthEventUIController激活: {controller.gameObject.name}");
        }
    }

    // 添加缺失的EndEvent方法
    public void EndEvent()
    {
        Debug.Log("=== EventSceneManager.EndEvent 被调用 ===");
        
        try
        {
            // 卸载当前EventScene，返回MapScene
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("EventScene");
            Debug.Log("EventScene 已卸载，返回MapScene");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"卸载EventScene失败: {e.Message}");
        }
    }

    // 显示事件UI的通用方法
    public void ShowEventUI(EventType type)
    {
        Debug.Log($"=== ShowEventUI 被调用，事件类型: {type} ===");
        
        // 隐藏所有UI
        if (springUI != null) springUI.SetActive(false);
        if (growthUI != null) growthUI.SetActive(false);
        if (rewardUI != null) rewardUI.SetActive(false);
        
        // 根据事件类型显示对应UI
        switch (type)
        {
            case EventType.Growth:
                ShowGrowthUI();
                break;
            case EventType.Spring:
                ShowSpringUI();
                break;
            case EventType.CombatReward:
                ShowRewardUI();
                break;
            default:
                Debug.LogWarning($"未知事件类型: {type}，默认显示Growth");
                ShowGrowthUI();
                break;
        }
    }

    private void ShowGrowthUI()
    {
        Debug.Log("=== 显示Growth事件UI ===");
        if (growthUI != null)
        {
            growthUI.SetActive(true);
            
            GrowthEventManager manager = growthUI.GetComponent<GrowthEventManager>();
            if (manager != null)
            {
                manager.ShowGrowthEvent();
            }
        }
        else
        {
            Debug.LogError("growthUI 为空！");
        }
    }

    private void ShowSpringUI()
    {
        Debug.Log("=== 显示Spring事件UI ===");
        if (springUI != null)
        {
            springUI.SetActive(true);
        }
        else
        {
            Debug.LogError("springUI 为空！");
        }
    }

    private void ShowRewardUI()
    {
        Debug.Log("=== 显示Reward事件UI ===");
        if (rewardUI != null)
        {
            rewardUI.SetActive(true);
        }
        else
        {
            Debug.LogError("rewardUI 为空！");
        }
    }

    // 统一的场景切换方法
    public void LoadScene(string sceneName)
    {
        Debug.Log($"=== 加载场景: {sceneName} ===");
        
        try
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"加载场景失败: {e.Message}");
        }
    }

    // 返回地图的便捷方法
    public void ReturnToMap()
    {
        Debug.Log("=== 返回地图 ===");
        LoadScene("MapScene");
    }
}