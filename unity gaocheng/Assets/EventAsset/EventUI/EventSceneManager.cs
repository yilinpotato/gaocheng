using UnityEngine;
public class EventSceneManager : MonoBehaviour
{
    public GameObject springUI;
    public GameObject growthUI;
    public GameObject rewardUI;

    void Start()
    {
        Debug.Log("�¼�������ʼ��");
        Debug.Log("��ǰ�¼�����: " + EventSceneData.currentEventType);
        ShowEventUI(EventSceneData.currentEventType);
    }

    void ShowEventUI(EventType type)
    {

    
            growthUI?.SetActive(true);
            GrowthEventManager growthManager = growthUI?.GetComponent<GrowthEventManager>();
            if (growthManager != null)
            {
                growthManager.ShowGrowthEvent();
                Debug.Log("��ʾ�ɳ��¼�UI");
            }
            else
            {
                Debug.LogError("�Ҳ��� GrowthEventManager ���!");
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