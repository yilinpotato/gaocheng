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
        
        // ���ȴ����ظ���EventSystem
        FixDuplicateEventSystems();
        
        // ǿ�Ʋ��Ҳ�����Growth UI
        ForceActivateGrowthUI();
        
        // ǿ������ΪGrowth�¼�����
        EventSceneData.currentEventType = EventType.Growth;
        Debug.Log($"ǿ���趨�¼�����Ϊ: {EventSceneData.currentEventType}");
    }

    private void FixDuplicateEventSystems()
    {
        Debug.Log("=== �޸��ظ���EventSystem ===");
        
        // ��������EventSystem
        UnityEngine.EventSystems.EventSystem[] eventSystems = FindObjectsOfType<UnityEngine.EventSystems.EventSystem>();
        
        Debug.Log($"�ҵ� {eventSystems.Length} ��EventSystem");
        
        if (eventSystems.Length > 1)
        {
            Debug.LogWarning("���ֶ��EventSystem����ɾ�������");
            
            // ������һ����ɾ�������
            for (int i = 1; i < eventSystems.Length; i++)
            {
                Debug.Log($"ɾ�������EventSystem: {eventSystems[i].gameObject.name}");
                DestroyImmediate(eventSystems[i].gameObject);
            }
        }
        
        // ȷ��ʣ���EventSystem�Ǽ����
        if (eventSystems.Length > 0 && eventSystems[0] != null)
        {
            eventSystems[0].gameObject.SetActive(true);
            Debug.Log($"ȷ��EventSystem����: {eventSystems[0].gameObject.name}");
        }
    }

    private void ForceActivateGrowthUI()
    {
        Debug.Log("=== ǿ�Ƽ���Growth UI ===");
        
        // ��ʽ1��ͨ�����ü���
        if (growthUI != null)
        {
            growthUI.SetActive(true);
            Debug.Log($"ͨ�����ü��� growthUI: {growthUI.name}");
        }
        
        // ��ʽ2�������Ʋ��Ҳ�����
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
                Debug.Log($"�����Ƽ���: {candidate.name}");
                
                // �����Canvas��ȷ���Ӷ���Ҳ����
                if (candidate.name.Contains("Canvas"))
                {
                    for (int i = 0; i < candidate.transform.childCount; i++)
                    {
                        candidate.transform.GetChild(i).gameObject.SetActive(true);
                    }
                }
            }
        }
        
        // ��ʽ3��ǿ�Ƽ�������Growth��ض���
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("Growth") || obj.name.Contains("Event"))
            {
                if (!obj.activeInHierarchy)
                {
                    obj.SetActive(true);
                    Debug.Log($"ǿ�Ƽ���: {obj.name}");
                }
            }
        }
        
        // ��ʽ4��ֱ��ͨ���������
        GrowthEventManager manager = FindObjectOfType<GrowthEventManager>();
        if (manager != null)
        {
            manager.gameObject.SetActive(true);
            Debug.Log($"ͨ��GrowthEventManager����: {manager.gameObject.name}");
        }
        
        GrowthEventUIController controller = FindObjectOfType<GrowthEventUIController>();
        if (controller != null)
        {
            controller.gameObject.SetActive(true);
            Debug.Log($"ͨ��GrowthEventUIController����: {controller.gameObject.name}");
        }
    }

    // ���ȱʧ��EndEvent����
    public void EndEvent()
    {
        Debug.Log("=== EventSceneManager.EndEvent ������ ===");
        
        try
        {
            // ж�ص�ǰEventScene������MapScene
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("EventScene");
            Debug.Log("EventScene ��ж�أ�����MapScene");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"ж��EventSceneʧ��: {e.Message}");
        }
    }

    // ��ʾ�¼�UI��ͨ�÷���
    public void ShowEventUI(EventType type)
    {
        Debug.Log($"=== ShowEventUI �����ã��¼�����: {type} ===");
        
        // ��������UI
        if (springUI != null) springUI.SetActive(false);
        if (growthUI != null) growthUI.SetActive(false);
        if (rewardUI != null) rewardUI.SetActive(false);
        
        // �����¼�������ʾ��ӦUI
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
                Debug.LogWarning($"δ֪�¼�����: {type}��Ĭ����ʾGrowth");
                ShowGrowthUI();
                break;
        }
    }

    private void ShowGrowthUI()
    {
        Debug.Log("=== ��ʾGrowth�¼�UI ===");
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
            Debug.LogError("growthUI Ϊ�գ�");
        }
    }

    private void ShowSpringUI()
    {
        Debug.Log("=== ��ʾSpring�¼�UI ===");
        if (springUI != null)
        {
            springUI.SetActive(true);
        }
        else
        {
            Debug.LogError("springUI Ϊ�գ�");
        }
    }

    private void ShowRewardUI()
    {
        Debug.Log("=== ��ʾReward�¼�UI ===");
        if (rewardUI != null)
        {
            rewardUI.SetActive(true);
        }
        else
        {
            Debug.LogError("rewardUI Ϊ�գ�");
        }
    }

    // ͳһ�ĳ����л�����
    public void LoadScene(string sceneName)
    {
        Debug.Log($"=== ���س���: {sceneName} ===");
        
        try
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"���س���ʧ��: {e.Message}");
        }
    }

    // ���ص�ͼ�ı�ݷ���
    public void ReturnToMap()
    {
        Debug.Log("=== ���ص�ͼ ===");
        LoadScene("MapScene");
    }
}