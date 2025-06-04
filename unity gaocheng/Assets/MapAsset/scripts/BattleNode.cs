using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class BattleNode : Node
{
    [Header("战斗基础配置")]
    public bool isBattleActive = false;
    public Enemy enemy;

    protected abstract string BattleSceneName { get; }
    private GameObject playerInstance;

    public virtual void StartBattle()
    {
        isBattleActive = true;

        // 保存玩家引用
        playerInstance = FindObjectOfType<Player>()?.gameObject;

        // 禁用玩家（而非销毁），保持其状态
        if (playerInstance != null)
        {
            playerInstance.SetActive(false);
        }

        SceneManager.LoadScene(BattleSceneName, LoadSceneMode.Additive);
        SceneManager.sceneLoaded += OnBattleSceneLoaded;
        SceneHider.SetSceneActive("MapScene", false);
        Debug.Log($"{BattleSceneName}战斗开始！");
        Invoke("EndBattle", 10f); // 测试用，10秒后结束战斗
    }

    private void OnBattleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == BattleSceneName)
        {
            SceneManager.sceneLoaded -= OnBattleSceneLoaded;
            SceneManager.SetActiveScene(scene);

            // 重定位现有玩家（如果有）
            BattleSceneController controller = FindObjectOfType<BattleSceneController>();
            Player player = FindObjectOfType<Player>();

            if (player != null && controller != null)
            {
                player.transform.position = controller.playerSpawn.position;
            }
        }
    }

    public virtual void EndBattle()
    {
        isBattleActive = false;
        if (BulletManager.Instance != null)
        {
            BulletManager.Instance.ResetAllBullets();
        }
        // 恢复玩家（如果之前禁用了）
        if (playerInstance != null)
        {
            playerInstance.SetActive(true);
            playerInstance = null;
        }

        // 卸载战斗场景
        SceneManager.UnloadSceneAsync(BattleSceneName);

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
        Debug.Log($"{BattleSceneName}战斗结束");

        // 战斗胜利后的逻辑
        BattleCompleted();
    }

    protected virtual void BattleCompleted()
    {
        Debug.Log($"战斗胜利！");
    }
}