using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
public class LoadManager : MonoBehaviour
{
    // ����ģʽ
    public static LoadManager Instance { get; private set; }

    // �浵Ŀ¼
    private string saveDirectory;
    // �浵�ļ���չ��
    private const string SAVE_EXTENSION = ".json";

    // ��ǰ���ص��������
    private PlayerData currentPlayerData;

    // �浵��Ϣ�б����ڴ浵ѡ����棩
    private List<SaveInfo> saveInfoList = new List<SaveInfo>();

    // �浵��Ϣ��
    [Serializable]
    public class SaveInfo
    {
        public int saveId;
        public string playerName;
        public DateTime saveDateTime;
        public float playTime;
        public int level;

        public SaveInfo(PlayerData data)
        {
            this.saveId = data.saveId;
            this.playerName = data.playerName;
            this.saveDateTime = DateTime.Now;
            this.playTime = data.playTime;
            this.level = data.level;
        }
    }

    void Awake()
    {
        // ����ģʽʵ��
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // ��ʼ���浵Ŀ¼
            saveDirectory = Path.Combine(Application.persistentDataPath, "Saves");

            // ȷ���浵Ŀ¼����
            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }

            // �������д浵��Ϣ
            RefreshSaveList();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ˢ�´浵�б�
    public void RefreshSaveList()
    {
        saveInfoList.Clear();

        // ���浵Ŀ¼�µ������ļ�
        if (Directory.Exists(saveDirectory))
        {
            string[] saveFiles = Directory.GetFiles(saveDirectory, "*" + SAVE_EXTENSION);

            foreach (string file in saveFiles)
            {
                try
                {
                    // ��ȡ�ļ�����
                    string json = File.ReadAllText(file);
                    PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);

                    if (playerData != null)
                    {
                        SaveInfo info = new SaveInfo(playerData);
                        saveInfoList.Add(info);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"���ش浵��Ϣʧ��: {file}, ����: {e.Message}");
                }
            }

            // ���浵ID����
            saveInfoList = saveInfoList.OrderByDescending(x => x.saveDateTime).ToList();
        }
    }

    // ��ȡ���д浵��Ϣ
    public List<SaveInfo> GetAllSaves()
    {
        RefreshSaveList();
        return saveInfoList;
    }

    // ���ش浵
    public PlayerData LoadGame(int saveId)
    {
        string savePath = Path.Combine(saveDirectory, $"save_{saveId}{SAVE_EXTENSION}");

        if (File.Exists(savePath))
        {
            try
            {
                string json = File.ReadAllText(savePath);
                currentPlayerData = JsonUtility.FromJson<PlayerData>(json);

                Debug.Log($"�ɹ����ش浵 ID: {saveId}");

                // ���������﷢���¼�֪ͨ��Ϸ״̬����
                // EventManager.TriggerEvent("OnGameLoaded", currentPlayerData);

                return currentPlayerData;
            }
            catch (Exception e)
            {
                Debug.LogError($"���ش浵 {saveId} ʧ��: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning($"�浵������: {saveId}");
        }

        return null;
    }

    // ɾ���浵
    public bool DeleteSave(int saveId)
    {
        string savePath = Path.Combine(saveDirectory, $"save_{saveId}{SAVE_EXTENSION}");

        if (File.Exists(savePath))
        {
            try
            {
                File.Delete(savePath);
                RefreshSaveList();
                Debug.Log($"�ɹ�ɾ���浵 ID: {saveId}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"ɾ���浵 {saveId} ʧ��: {e.Message}");
            }
        }

        return false;
    }

    // ��ȡ��ǰ���ص��������
    public PlayerData GetCurrentPlayerData()
    {
        return currentPlayerData;
    }

    // ��������Ĵ浵
    public PlayerData LoadLatestSave()
    {
        RefreshSaveList();

        if (saveInfoList.Count > 0)
        {
            // ��ȡ���µĴ浵ID
            int latestSaveId = saveInfoList[0].saveId;
            return LoadGame(latestSaveId);
        }

        return null;
    }

    // �ж�ָ��ID�Ĵ浵�Ƿ����
    public bool DoesSaveExist(int saveId)
    {
        string savePath = Path.Combine(saveDirectory, $"save_{saveId}{SAVE_EXTENSION}");
        return File.Exists(savePath);
    }
}
