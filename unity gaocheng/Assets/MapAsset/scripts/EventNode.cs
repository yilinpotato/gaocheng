using UnityEngine;
using UnityEngine.SceneManagement;

public class EventNode : Node
{
    [HideInInspector] public EventType eventType;

    protected virtual string EventSceneName => "EventScene";

    public void RandomizeEvent()
    {
        // ��ʱ�̶�ΪGrowth�¼�
        eventType = EventType.Growth;
        
        // ����ȫ���¼�����
        EventSceneData.currentEventType = eventType;
        
        Debug.Log($"�¼����ͱ�ǿ���趨Ϊ: {eventType}");
        Debug.Log($"EventSceneData.currentEventType ����Ϊ: {EventSceneData.currentEventType}");
    }

    public void StartEvent()
    {
        Debug.Log("=== StartEvent ��ʼ ===");
        
        // ǿ������ΪGrowth�¼�
        RandomizeEvent();
        
        // ��ʾ�¼����
        ShowEventPanel();
    }

    private void ShowEventPanel()
    {
        try
        {
            Debug.Log($"ShowEventPanel - �������س������¼�����: {EventSceneData.currentEventType}");
            
            // ��Ҫ����ΪAdditiveģʽ�����滻��ǰ����
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
        Debug.Log($"�¼���������");
        
        // ж���¼������������Ǽ����³���
        try
        {
            SceneManager.UnloadSceneAsync("EventScene");
            Debug.Log("EventScene ��ж��");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"ж��EventSceneʧ��: {e.Message}");
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