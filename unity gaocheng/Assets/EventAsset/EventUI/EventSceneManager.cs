using UnityEngine;
public class EventSceneManager : MonoBehaviour
{
    public GameObject springUI;
    public GameObject growthUI;
    public GameObject rewardUI;

    public void InitializeEvent()
    {
        Debug.Log("�¼�������ʼ��");
        Debug.Log("��ǰ�¼�����: " + EventSceneData.currentEventType);
        ShowEventUI(EventSceneData.currentEventType);
    }

    void ShowEventUI(EventType type)
    {
        // ��������UI
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

    // ͳһ���¼���������
    public void EndEvent()
    {
        EventNode eventNode = FindObjectOfType<EventNode>();
        if (eventNode != null)
        {
            eventNode.EndEvent();
        }
    }
}