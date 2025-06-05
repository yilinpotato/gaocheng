using UnityEngine;
public class EventSceneManager : MonoBehaviour
{
    public GameObject springUI;
    public GameObject growthUI;
    public GameObject rewardUI;

    void Start()
    {
        Debug.Log("事件场景初始化");
        Debug.Log("当前事件类型: " + EventSceneData.currentEventType);
        ShowEventUI(EventSceneData.currentEventType);
    }

    void ShowEventUI(EventType type)
    {

    
            growthUI?.SetActive(true);
            GrowthEventManager growthManager = growthUI?.GetComponent<GrowthEventManager>();
            if (growthManager != null)
            {
                growthManager.ShowGrowthEvent();
                Debug.Log("显示成长事件UI");
            }
            else
            {
                Debug.LogError("找不到 GrowthEventManager 组件!");
            }
       
    }

    // 统一的事件结束方法
    public void EndEvent()
    {
        EventNode eventNode = FindObjectOfType<EventNode>();
        if (eventNode != null)
        {
            eventNode.EndEvent();
        }
    }
}