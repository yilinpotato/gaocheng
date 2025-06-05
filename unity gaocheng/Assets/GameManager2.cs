using UnityEngine;
using System.IO;
public class GameManager2 : MonoBehaviour
{
    // ����ģʽ
    public static GameManager2 Instance { get; private set; }

    void Awake()
    {
        // ʵ�ֵ���ģʽ
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // ȷ���浵Ŀ¼����
            string saveDirectory = Path.Combine(Application.persistentDataPath, "Saves");
            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }

            // ȷ�� SaveManager �� LoadManager ��ʵ����
            InitializeSaveLoadSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ��ʼ���浵�Ͷ���ϵͳ
    private void InitializeSaveLoadSystem()
    {
        // ȷ�� SaveManager ����
        if (SaveManager.Instance == null)
        {
            GameObject saveManagerObj = new GameObject("SaveManager");
            saveManagerObj.AddComponent<SaveManager>();
            // �����ڳ����л�ʱ��������
            DontDestroyOnLoad(saveManagerObj);
        }

        // ȷ�� LoadManager ����
        if (LoadManager.Instance == null)
        {
            GameObject loadManagerObj = new GameObject("LoadManager");
            loadManagerObj.AddComponent<LoadManager>();
            // �����ڳ����л�ʱ��������
            DontDestroyOnLoad(loadManagerObj);
        }

        Debug.Log("�浵ϵͳ��ʼ�����");
    }

    // �ṩһ�������������ű���ȡ��ǰ�������
    public PlayerData GetCurrentPlayerData()
    {
        if (LoadManager.Instance != null)
        {
            return LoadManager.Instance.GetCurrentPlayerData();
        }
        return null;
    }

    // �ṩһ�����ٱ��淽��
    public void QuickSaveGame(PlayerData data)
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.QuickSave(data);
        }
    }
}
