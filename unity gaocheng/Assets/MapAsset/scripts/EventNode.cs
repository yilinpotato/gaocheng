using UnityEngine;
using UnityEngine.SceneManagement;

public class EventNode : Node
{
    [HideInInspector] public EventType eventType;

    protected virtual string EventSceneName => "EventScene";



    public void RandomizeEvent()
    {
        // 使用Growth替换Shop
        EventType[] availableEvents = { EventType.Spring, EventType.Growth, EventType.CombatReward };
        eventType = availableEvents[Random.Range(0, availableEvents.Length)];
        Debug.Log($"事件类型被设定为: {eventType}");
    }

    public void EnterEventScene()
    {

        SceneManager.LoadScene(EventSceneName, LoadSceneMode.Additive);
        SceneManager.sceneLoaded += OnEventSceneLoaded;
        SceneHider.SetSceneActive("MapScene", false);
        Debug.Log($"进入事件场景: {EventSceneName}");
    }

    private void OnEventSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == EventSceneName)
        {
            SceneManager.sceneLoaded -= OnEventSceneLoaded;
            SceneManager.SetActiveScene(scene);

            // 初始化事件场景
            EventSceneManager eventManager = FindObjectOfType<EventSceneManager>();
            if (eventManager != null)
            {
                eventManager.InitializeEvent();
            }
        }
    }

    public void EndEvent()
    {

        // 卸载事件场景
        SceneManager.UnloadSceneAsync(EventSceneName);

        // 确保地图场景被重新激活
        Scene mapScene = SceneManager.GetSceneByName("MapScene");
        if (mapScene.isLoaded)
        {
            SceneManager.SetActiveScene(mapScene);
        }
        else
        {
            Debug.LogWarning("地图场景未加载");
        }

        SceneHider.SetSceneActive("MapScene", true);
        Debug.Log($"事件场景结束");
    }
}