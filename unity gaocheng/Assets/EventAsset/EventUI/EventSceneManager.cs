using UnityEngine;
public class EventSceneManager : MonoBehaviour
{
    public GameObject springUI;
    public GameObject growthUI;
    public GameObject rewardUI;

    public void InitializeEvent()
    {
        Debug.Log("事件场景初始化");
        Debug.Log("当前事件类型: " + EventSceneData.currentEventType);
        ShowEventUI(EventSceneData.currentEventType);
    }

    void ShowEventUI(EventType type)
    {
        // 隐藏所有UI
        springUI?.SetActive(false);
        growthUI?.SetActive(false);
        rewardUI?.SetActive(false);

        switch (type)
        {
            case EventType.Spring:
                springUI?.SetActive(true);
                springUI?.GetComponent<SpringEventUI>()?.Show();
                break;
            case EventType.Growth:
                growthUI?.SetActive(true);
                growthUI?.GetComponent<GrowthEventManager>()?.ShowGrowthEvent();
                break;
            case EventType.CombatReward:
                rewardUI?.SetActive(true);
                rewardUI?.GetComponent<BattleEventUI>()?.Show(EventSceneData.combatNode);
                break;
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