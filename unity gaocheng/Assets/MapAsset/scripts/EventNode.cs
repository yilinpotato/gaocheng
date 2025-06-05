using UnityEngine;
using UnityEngine.SceneManagement;

public class EventNode : Node
{
    [HideInInspector] public EventType eventType;

    protected virtual string EventSceneName => "EventScene";



    public void RandomizeEvent()
    {
        // ʹ��Growth�滻Shop
        EventType[] availableEvents = { EventType.Spring, EventType.Growth, EventType.CombatReward };
        eventType = availableEvents[Random.Range(0, availableEvents.Length)];
        Debug.Log($"�¼����ͱ��趨Ϊ: {eventType}");
    }

    public void EnterEventScene()
    {

        SceneManager.LoadScene(EventSceneName, LoadSceneMode.Additive);
        SceneManager.sceneLoaded += OnEventSceneLoaded;
        SceneHider.SetSceneActive("MapScene", false);
        Debug.Log($"�����¼�����: {EventSceneName}");
    }

    private void OnEventSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == EventSceneName)
        {
            SceneManager.sceneLoaded -= OnEventSceneLoaded;
            SceneManager.SetActiveScene(scene);

            // ��ʼ���¼�����
            EventSceneManager eventManager = FindObjectOfType<EventSceneManager>();
            if (eventManager != null)
            {
                eventManager.InitializeEvent();
            }
        }
    }

    public void EndEvent()
    {

        // ж���¼�����
        SceneManager.UnloadSceneAsync(EventSceneName);

        // ȷ����ͼ���������¼���
        Scene mapScene = SceneManager.GetSceneByName("MapScene");
        if (mapScene.isLoaded)
        {
            SceneManager.SetActiveScene(mapScene);
        }
        else
        {
            Debug.LogWarning("��ͼ����δ����");
        }

        SceneHider.SetSceneActive("MapScene", true);
        Debug.Log($"�¼���������");
    }
}