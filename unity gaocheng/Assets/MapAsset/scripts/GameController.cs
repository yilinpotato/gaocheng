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
    }

    // Update is called once per frame  
    void Update()
    {

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
        currentPlayerData.playTime += Time.time;

        // ���������Ӹ�������������ݵĴ���
        // ����Ѫ������Ǯ���÷ֵ�

        Debug.Log($"�Ѹ������ {currentPlayerData.playerName} ��������׼������");
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
}
