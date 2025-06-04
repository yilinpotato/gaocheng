using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections; // �����������IEnumerator

[RequireComponent(typeof(RectTransform))]
public class forTestButton : MonoBehaviour
{
    [Header("��ͼ��������")]
    public string mapSceneName = "MapScene";

    [Header("��ť����")]
    public string buttonText = "���ص�ͼ";
    public Color buttonColor = Color.green;

    private Button button;
    private Text text;

    void Awake()
    {
        // ������ļ�� - ȷ��������UI������
        if (GetComponent<RectTransform>() == null)
        {
            Debug.LogError("forTestButton��������ھ���RectTransform��UI�����ϣ�");
            enabled = false;
            return;
        }
    }

    void Start()
    {
        // ��鲢��ȡButton���
        button = GetComponent<Button>();
        if (button == null)
        {
            Debug.LogWarning("δ�ҵ�Button������������...");
            button = gameObject.AddComponent<Button>();
        }

        // ���ð�ť�¼�
        button.onClick.RemoveAllListeners(); // ������м�����
        button.onClick.AddListener(ReturnToMapScene);

        // ���ð�ť��ɫ
        ColorBlock colors = button.colors;
        colors.normalColor = buttonColor;
        button.colors = colors;

        // ���һ򴴽��ı����
        text = GetComponentInChildren<Text>();
        if (text == null)
        {
            // ���û����Text���󣬴���һ��
            GameObject textObj = new GameObject("ButtonText");
            textObj.transform.SetParent(transform, false);
            text = textObj.AddComponent<Text>();

            // ȷ��Text������RectTransform
            RectTransform textRect = text.rectTransform;
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
        }

        // �����ı�����
        text.text = buttonText;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
    }

    // ���ص�ͼ�����ķ���
    public void ReturnToMapScene()
    {
        Debug.Log("[����] ��ʼִ�з��ص�ͼ����...");

        try
        {
            // ��ӡ��ǰ���ص����г���
            Debug.Log("[����] ��ǰ���صĳ���:");
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                Debug.Log($"  - ����: {scene.name}, ·��: {scene.path}, ����: {i}, �Ѽ���: {scene.isLoaded}");
            }

            // ��ӡ���������е����г���
            Debug.Log("[����] ���������еĳ���:");
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                Debug.Log($"  - ����: {i}, ·��: {scenePath}, ����: {sceneName}");

                // ����ҵ�����Ҫ�ĵ�ͼ��������¼����ȷ������������
                if (scenePath.Contains("MapScene") || sceneName.Contains("MapScene"))
                {
                    Debug.Log($"[����] �ҵ����ܵĵ�ͼ����: ����={i}, ����={sceneName}, ·��={scenePath}");

                    // ����ֱ��ʹ�ô���������
                    Debug.Log($"[����] ֱ�ӳ��Լ������� {i} �ĳ���");
                    SceneManager.LoadScene(i, LoadSceneMode.Single);
                    return;
                }
            }

            // ���û���ҵ�ƥ��ĵ�ͼ����������ʹ��Ĭ������
            Debug.Log($"[����] δ�ڹ����������ҵ���ͼ����������ֱ��ʹ������'{mapSceneName}'");
            SceneManager.LoadScene(mapSceneName, LoadSceneMode.Single);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[����] ���ص�ͼ����ʱ����: {e.Message}\n{e.StackTrace}");
        }
    }

}
