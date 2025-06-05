using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
public class SaveManager : MonoBehaviour
{
    // ����ģʽ
    public static SaveManager Instance { get; private set; }

    // �浵Ŀ¼
    private string saveDirectory;

    // �浵�ļ���չ��
    private const string SAVE_EXTENSION = ".json";

    // ��ǰ�浵ID������
    private int currentSaveIdCounter = 0;

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

            // ��ʼ���浵ID������
            InitializeSaveIdCounter();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ��ʼ���浵ID���������ҵ����д浵�е����ID
    private void InitializeSaveIdCounter()
    {
        int maxId = 0;

        if (Directory.Exists(saveDirectory))
        {
            string[] saveFiles = Directory.GetFiles(saveDirectory, "*" + SAVE_EXTENSION);

            foreach (string file in saveFiles)
            {
                try
                {
                    string fileName = Path.GetFileNameWithoutExtension(file);
                    if (fileName.StartsWith("save_"))
                    {
                        string idStr = fileName.Substring(5); // ɾ�� "save_" ǰ׺
                        if (int.TryParse(idStr, out int id) && id > maxId)
                        {
                            maxId = id;
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"����浵�ļ���ʧ��: {file}, ����: {e.Message}");
                }
            }
        }

        currentSaveIdCounter = maxId + 1;
        Debug.Log($"�浵ID��������ʼ��Ϊ: {currentSaveIdCounter}");
    }

    // ������Ϸ����
    public int SaveGame(PlayerData playerData, bool createNewSave = false)
    {
        // ���Ҫ�����´浵����û��ָ���浵ID
        if (createNewSave || playerData.saveId == 0)
        {
            playerData.saveId = currentSaveIdCounter++;
        }

        // ������Ϸʱ��
        playerData.playTime += Time.time; // ���ۼӵ�ǰ�Ựʱ��

        string savePath = Path.Combine(saveDirectory, $"save_{playerData.saveId}{SAVE_EXTENSION}");

        try
        {
            // ������������л�ΪJSON
            string json = JsonUtility.ToJson(playerData, true);
            File.WriteAllText(savePath, json);

            Debug.Log($"��Ϸ�ѱ��浽: {savePath}");

            // ���������﷢���¼�֪ͨ��Ϸ״̬����
            // EventManager.TriggerEvent("OnGameSaved", playerData);

            return playerData.saveId;
        }
        catch (Exception e)
        {
            Debug.LogError($"������Ϸʧ��: {e.Message}");
            return -1;
        }
    }

    // ��������Ϸ�浵
    public int CreateNewSave(string playerName)
    {
        PlayerData newData = new PlayerData
        {
            playerName = playerName,
            saveId = currentSaveIdCounter,
            level = 1,
            health = 100f,
            playTime = 0f,
            money = 100,
            score = 0
        };

        return SaveGame(newData, true);
    }

    // ���ٱ��浱ǰ��Ϸ
    public int QuickSave(PlayerData currentData)
    {
        if (currentData == null)
        {
            Debug.LogError("�޷����ٱ��棺��ǰ��Ϸ����Ϊ��");
            return -1;
        }

        // ʹ�����д浵ID���б���
        return SaveGame(currentData, false);
    }

    // ��ȡ��ǰʱ��ĸ�ʽ���ַ�������������ʱ�浵��
    private string GetTimeStamp()
    {
        return DateTime.Now.ToString("yyyyMMdd_HHmmss");
    }

    // �����Զ��浵
    public int CreateAutoSave(PlayerData currentData)
    {
        if (currentData == null)
        {
            Debug.LogError("�޷������Զ��浵����ǰ��Ϸ����Ϊ��");
            return -1;
        }

        // ���Ƶ�ǰ���ݲ��޸�Ϊ�Զ��浵
        PlayerData autoSaveData = new PlayerData(
            currentSaveIdCounter,
            $"{currentData.playerName}_�Զ��浵",
            currentData.level,
            currentData.health,
            currentData.playTime,
            currentData.money,
            currentData.score
        );

        return SaveGame(autoSaveData, true);
    }

    // ������Ϸ״̬����ʱ�ļ�(���ڳ����л���)
    public void SaveGameState(PlayerData currentData)
    {
        if (currentData == null)
        {
            Debug.LogError("�޷�������Ϸ״̬����ǰ��Ϸ����Ϊ��");
            return;
        }

        string tempPath = Path.Combine(saveDirectory, "temp_state" + SAVE_EXTENSION);

        try
        {
            // ������������л�ΪJSON
            string json = JsonUtility.ToJson(currentData, true);
            File.WriteAllText(tempPath, json);

            Debug.Log("��Ϸ״̬����ʱ����");
        }
        catch (Exception e)
        {
            Debug.LogError($"������Ϸ״̬ʧ��: {e.Message}");
        }
    }

    // ������ʱ��Ϸ״̬
    public PlayerData LoadGameState()
    {
        string tempPath = Path.Combine(saveDirectory, "temp_state" + SAVE_EXTENSION);

        if (File.Exists(tempPath))
        {
            try
            {
                string json = File.ReadAllText(tempPath);
                return JsonUtility.FromJson<PlayerData>(json);
            }
            catch (Exception e)
            {
                Debug.LogError($"������Ϸ״̬ʧ��: {e.Message}");
            }
        }

        return null;
    }

    // �������д浵
    public bool OverwriteSave(int saveId, PlayerData newData)
    {
        if (saveId <= 0)
        {
            Debug.LogError("�޷����Ǵ浵����Ч�Ĵ浵ID");
            return false;
        }

        newData.saveId = saveId;
        return SaveGame(newData, false) > 0;
    }
}
