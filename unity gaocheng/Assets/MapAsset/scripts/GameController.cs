using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // ��ǰ�������
    private PlayerData currentPlayerData;

    public string LastMapSceneName { get; set; }

    public MapManager mapManager;

    public static GameController Instance { get; private set; }

    // �������
    public int RandomSeed { get; private set; }

    [Header("�Զ���������")]
    [SerializeField] private float autoSaveInterval = 10f; // �Զ����������룩
    [SerializeField] private bool enableAutoSave = true;   // �Ƿ������Զ�����
    
    private float autoSaveTimer = 0f; // �Զ������ʱ��

    // Start is called before the first frame update  
    void Start()
    {
      
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // �����������
        RandomSeed = GenerateRandomSeed();
        Debug.Log("�������: " + RandomSeed);

        // ֻ���ڵ�ͼδ���ɵ�����²����ɵ�ͼ
        if (!MapManager.isMapGenerated)
        {
            mapManager.GenerateMap();
        }
        else
        {
            Debug.Log("��ͼ�Ѵ��ڣ�������������");
        }

        // ��ʼ���Զ������ʱ��
        autoSaveTimer = autoSaveInterval;
        
        // ��ʱ���ԣ�����һ�������������
        if (currentPlayerData == null)
        {
            Debug.Log("[����] ��������������������Զ��������");
            
            try 
            {
                // ʹ���޲ι��캯������
                currentPlayerData = new PlayerData();
                currentPlayerData.playerName = "�������";
                currentPlayerData.level = 1;
                currentPlayerData.saveId = 999; // �����õ�ID
                
                Debug.Log("[����] ����������ݴ����ɹ� - �������" + currentPlayerData.playerName);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[����] ���������������ʧ��: {e.Message}");
            }
        }

        // ��ʾ�浵�ļ�����·��
        Debug.Log("�浵�ļ�����·��: " + Application.persistentDataPath);
    }

    // Update is called once per frame  
    void Update()
    {
        // ��ӵ�����Ϣ
        if (Time.frameCount % 600 == 0) // ÿ10��������ʾһ��״̬
        {
            Debug.Log($"[�Զ�����״̬] enableAutoSave: {enableAutoSave}, currentPlayerData: {(currentPlayerData != null ? "����" : "null")}, ��ʱ��: {autoSaveTimer:F1}��");
        }
        
        // �Զ������߼�
        if (enableAutoSave && currentPlayerData != null)
        {
            autoSaveTimer -= Time.deltaTime;
            
            if (autoSaveTimer <= 0f)
            {
                Debug.Log("[�Զ�����] ��ʱ�������ʼִ���Զ�����...");
                
                // ִ���Զ�����
                AutoSave();
                
                // ���ü�ʱ��
                autoSaveTimer = autoSaveInterval;
            }
        }
        else if (enableAutoSave && currentPlayerData == null)
        {
            // ֻ�ڵ�һ����ʾ�������
            if (Time.frameCount % 300 == 0)
            {
                Debug.LogWarning("[�Զ�����] �������Ϊ�գ��޷�ִ���Զ�����");
            }
        }
    }

    // �Զ����淽��
    private void AutoSave()
    {
        
        try
        {
            Debug.Log("[�Զ�����] ��ʼִ���Զ�����...");
            
            // ȷ��SaveManagerʵ������
            if (SaveManager.Instance == null)
            {
                Debug.LogWarning("[�Զ�����] SaveManagerʵ�������ڣ������Զ�����");
                return;
            }

            // ���浱ǰ��Ϸ
            SaveCurrentGame();
            
            Debug.Log($"[�Զ�����] ���Զ�������� {currentPlayerData.playerName} ����Ϸ����");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[�Զ�����] �Զ�����ʱ��������: {e.Message}");
        }
    }

    public void StartNewGame(string playerName)
    {
        // �����´浵
        int saveId = SaveManager.Instance.CreateNewSave(playerName);

        // �����´����Ĵ浵
        if (saveId > 0)
        {
            currentPlayerData = LoadManager.Instance.LoadGame(saveId);
            // ��ʼ����Ϸ״̬...
            
            // �����Զ������ʱ��
            autoSaveTimer = autoSaveInterval;
        }
    }

    // ���浱ǰ��Ϸ
    public void SaveCurrentGame()
    {
        if (currentPlayerData != null)
        {
            // ����������ݣ������Ҫ��
            UpdatePlayerDataBeforeSave();

            // ������Ϸ
            SaveManager.Instance.QuickSave(currentPlayerData);
        }
    }

    // �ֶ����棨���Ա���ť������UI���ã�
    public void ManualSave()
    {
        if (currentPlayerData != null)
        {
            SaveCurrentGame();
            Debug.Log("[�ֶ�����] ��Ϸ���ֶ�����");
        }
        else
        {
            Debug.LogWarning("[�ֶ�����] �޷��ֶ����棺��ǰ�������Ϊ��");
        }
    }

    // �����Զ�������
    public void SetAutoSaveInterval(float interval)
    {
        autoSaveInterval = Mathf.Max(1f, interval); // ��С���1��
        Debug.Log($"�Զ�������������Ϊ {autoSaveInterval} ��");
    }

    // ����/�����Զ�����
    public void SetAutoSaveEnabled(bool enabled)
    {
        enableAutoSave = enabled;
        if (enabled)
        {
            autoSaveTimer = autoSaveInterval; // ���ü�ʱ��
            Debug.Log("�Զ�����������");
        }
        else
        {
            Debug.Log("�Զ������ѽ���");
        }
    }

    // ����������ӵķ���
    private int GenerateRandomSeed()
    {
        return System.DateTime.Now.Millisecond; // ʹ�õ�ǰʱ����������
    }

    // ������Ϸ
    public void LoadGame(int saveId)
    {
        currentPlayerData = LoadManager.Instance.LoadGame(saveId);
        if (currentPlayerData != null)
        {
            // Ӧ�ü��ص����ݵ���Ϸ��
            ApplyLoadedData();
            
            // �����Զ������ʱ��
            autoSaveTimer = autoSaveInterval;
        }
    }

    // �ڱ���ǰ�����������
    public void UpdatePlayerDataBeforeSave()
    {
        if (currentPlayerData == null)
        {
            Debug.LogWarning("�޷�����������ݣ���ǰ�������Ϊ��");
            return;
        }

        // ������Ϸʱ��
        currentPlayerData.playTime += Time.deltaTime;

        // ���������Ӹ�������������ݵĴ���
        // ����Ѫ������Ǯ���÷ֵ�

        // Debug.Log($"�Ѹ������ {currentPlayerData.playerName} ��������׼������");
    }

    // Ӧ�ü��ص�����
    public void ApplyLoadedData()
    {
        if (currentPlayerData == null)
        {
            Debug.LogWarning("�޷�Ӧ��������ݣ���ǰ�������Ϊ��");
            return;
        }

        // ���������ӽ����ص�����Ӧ�õ���Ϸ����Ĵ���
        // �����������Ѫ������Ǯ���÷ֵ�

        Debug.Log($"��Ӧ����� {currentPlayerData.playerName} �Ĵ浵����");
    }

    // ����Ϸ��ͣʱֹͣ�Զ�����
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            // ��Ϸ��ͣʱִ��һ�α���
            if (currentPlayerData != null)
            {
                SaveCurrentGame();
                Debug.Log("[��ͣ����] ��Ϸ��ͣʱ�ѱ�������");
            }
        }
    }

    // ��Ӧ���˳�ʱ����
    void OnApplicationQuit()
    {
        if (currentPlayerData != null)
        {
            SaveCurrentGame();
            Debug.Log("[�˳�����] Ӧ���˳�ʱ�ѱ�������");
        }
    }
}
