using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
public class SaveManager : MonoBehaviour
{
    // ??????
    public static SaveManager Instance { get; private set; }

    // ?��??
    private string saveDirectory;

    // ?��????????
    private const string SAVE_EXTENSION = ".json";

    // ????��ID??????
    private int currentSaveIdCounter = 0;

    void Awake()
    {
        // ?????????
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // ??????��??
            saveDirectory = Path.Combine(Application.persistentDataPath, "Saves");

            // ????��??????
            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }

            // ??????��ID??????
            InitializeSaveIdCounter();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ??????��ID??????????????�՛�?��????ID
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
                        string idStr = fileName.Substring(5); // ??? "save_" ??
                        if (int.TryParse(idStr, out int id) && id > maxId)
                        {
                            maxId = id;
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"?????��????????: {file}, ????: {e.Message}");
                }
            }
        }

        currentSaveIdCounter = maxId + 1;
        Debug.Log($"?��ID????????????: {currentSaveIdCounter}");
    }

    // ???????????
    public int SaveGame(PlayerData playerData, bool createNewSave = false)
    {
        // ??????????��???????????��ID
        if (createNewSave || playerData.saveId == 0)
        {
            playerData.saveId = currentSaveIdCounter++;
        }

        // ??????????
        playerData.playTime += Time.time; // ????????????

        string savePath = Path.Combine(saveDirectory, $"save_{playerData.saveId}{SAVE_EXTENSION}");

        try
        {
            // ????????????��??JSON
            string json = JsonUtility.ToJson(playerData, true);
            File.WriteAllText(savePath, json);

            Debug.Log($"???????��: {savePath}");

            // ??????????????????????????
            // EventManager.TriggerEvent("OnGameSaved", playerData);

            return playerData.saveId;
        }
        catch (Exception e)
        {
            Debug.LogError($"??????????: {e.Message}");
            return -1;
        }
    }

    // ??????????��
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

    // ??????��????
    public int QuickSave(PlayerData currentData)
    {
        if (currentData == null)
        {
            Debug.LogError("?????????��?????????????");
            return -1;
        }

        // ??????�՛�ID???��???
        return SaveGame(currentData, false);
    }

    // ???????????????????????????????��??
    private string GetTimeStamp()
    {
        return DateTime.Now.ToString("yyyyMMdd_HHmmss");
    }

    // ????????��
    public int CreateAutoSave(PlayerData currentData)
    {
        if (currentData == null)
        {
            Debug.LogError("???????????��???????????????");
            return -1;
        }

        // ???????????????????��
        PlayerData autoSaveData = new PlayerData(
            currentSaveIdCounter,
            $"{currentData.playerName}_????��",
            currentData.level,
            currentData.health,
            currentData.playTime,
            currentData.money,
            currentData.score
        );

        return SaveGame(autoSaveData, true);
    }

    // ?????????????????(????????��???)
    public void SaveGameState(PlayerData currentData)
    {
        if (currentData == null)
        {
            Debug.LogError("???????????????????????????");
            return;
        }

        string tempPath = Path.Combine(saveDirectory, "temp_state" + SAVE_EXTENSION);

        try
        {
            // ????????????��??JSON
            string json = JsonUtility.ToJson(currentData, true);
            File.WriteAllText(tempPath, json);

            Debug.Log("??????????????");
        }
        catch (Exception e)
        {
            Debug.LogError($"????????????: {e.Message}");
        }
    }

    // ????????????
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
                Debug.LogError($"????????????: {e.Message}");
            }
        }

        return null;
    }

    // ???????�՛�
    public bool OverwriteSave(int saveId, PlayerData newData)
    {
        if (saveId <= 0)
        {
            Debug.LogError("???????��????��??��ID");
            return false;
        }

        newData.saveId = saveId;
        return SaveGame(newData, false) > 0;
    }
}
