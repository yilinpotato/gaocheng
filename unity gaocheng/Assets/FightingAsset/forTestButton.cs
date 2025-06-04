using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections; // 添加这行用于IEnumerator

[RequireComponent(typeof(RectTransform))]
public class forTestButton : MonoBehaviour
{
    [Header("地图场景名称")]
    public string mapSceneName = "MapScene";

    [Header("按钮配置")]
    public string buttonText = "返回地图";
    public Color buttonColor = Color.green;

    private Button button;
    private Text text;

    void Awake()
    {
        // 最基本的检查 - 确保我们在UI环境中
        if (GetComponent<RectTransform>() == null)
        {
            Debug.LogError("forTestButton必须挂载在具有RectTransform的UI对象上！");
            enabled = false;
            return;
        }
    }

    void Start()
    {
        // 检查并获取Button组件
        button = GetComponent<Button>();
        if (button == null)
        {
            Debug.LogWarning("未找到Button组件，正在添加...");
            button = gameObject.AddComponent<Button>();
        }

        // 设置按钮事件
        button.onClick.RemoveAllListeners(); // 清除所有监听器
        button.onClick.AddListener(ReturnToMapScene);

        // 设置按钮颜色
        ColorBlock colors = button.colors;
        colors.normalColor = buttonColor;
        button.colors = colors;

        // 查找或创建文本组件
        text = GetComponentInChildren<Text>();
        if (text == null)
        {
            // 如果没有子Text对象，创建一个
            GameObject textObj = new GameObject("ButtonText");
            textObj.transform.SetParent(transform, false);
            text = textObj.AddComponent<Text>();

            // 确保Text对象有RectTransform
            RectTransform textRect = text.rectTransform;
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
        }

        // 设置文本属性
        text.text = buttonText;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
    }

    // 返回地图场景的方法
    public void ReturnToMapScene()
    {
        Debug.Log("[调试] 开始执行返回地图流程...");

        try
        {
            // 打印当前加载的所有场景
            Debug.Log("[调试] 当前加载的场景:");
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                Debug.Log($"  - 场景: {scene.name}, 路径: {scene.path}, 索引: {i}, 已加载: {scene.isLoaded}");
            }

            // 打印构建设置中的所有场景
            Debug.Log("[调试] 构建设置中的场景:");
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                Debug.Log($"  - 索引: {i}, 路径: {scenePath}, 名称: {sceneName}");

                // 如果找到我们要的地图场景，记录其正确的索引和名称
                if (scenePath.Contains("MapScene") || sceneName.Contains("MapScene"))
                {
                    Debug.Log($"[调试] 找到可能的地图场景: 索引={i}, 名称={sceneName}, 路径={scenePath}");

                    // 尝试直接使用此索引加载
                    Debug.Log($"[调试] 直接尝试加载索引 {i} 的场景");
                    SceneManager.LoadScene(i, LoadSceneMode.Single);
                    return;
                }
            }

            // 如果没有找到匹配的地图场景，尝试使用默认名称
            Debug.Log($"[调试] 未在构建设置中找到地图场景，尝试直接使用名称'{mapSceneName}'");
            SceneManager.LoadScene(mapSceneName, LoadSceneMode.Single);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[调试] 返回地图场景时出错: {e.Message}\n{e.StackTrace}");
        }
    }

}
