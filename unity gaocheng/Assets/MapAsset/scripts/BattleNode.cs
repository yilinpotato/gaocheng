using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class BattleNode : Node
{
    [Header("战斗节点设置")]
    public bool isBattleActive = false;
    public Enemy enemy;

    protected abstract string BattleSceneName { get; }
    private GameObject playerInstance;
    private bool battleCompleted = false; // 添加标记，避免重复结束战斗

    public virtual void StartBattle()
    {
        isBattleActive = true;
        battleCompleted = false; // 重置战斗完成标记

        // 保存主角引用
        playerInstance = FindObjectOfType<Player>()?.gameObject;

        // 主角暂时隐藏，保留其状态
        if (playerInstance != null)
        {
            playerInstance.SetActive(false);
        }

        // 加载战斗场景
        SceneManager.LoadScene(BattleSceneName, LoadSceneMode.Additive);
        SceneManager.sceneLoaded += OnBattleSceneLoaded;
        SceneHider.SetSceneActive("MapScene", false);
        Debug.Log($"{BattleSceneName}战斗开始！");

        // 移除自动结束战斗的调用
        // Invoke("EndBattle", 10f); 
    }

    private void OnBattleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == BattleSceneName)
        {
            SceneManager.sceneLoaded -= OnBattleSceneLoaded;
            SceneManager.SetActiveScene(scene);

            // 重定位玩家（如需）
            BattleSceneController controller = FindObjectOfType<BattleSceneController>();
            Enemy[] enemies = FindObjectsOfType<Enemy>();

            // 将敌人与战斗节点关联
            foreach (Enemy enemy in enemies)
            {
                enemy.SetBattleNode(this);
            }

            // 如果需要，可以对Player进行处理
            Player player = FindObjectOfType<Player>();
            if (player != null && controller != null)
            {
                player.transform.position = controller.playerSpawn.position;
            }
        }
    }

    public virtual void EndBattle()
    {
        // 防止重复调用
        if (battleCompleted) return;
        battleCompleted = true;

        isBattleActive = false;

        // 清理子弹等战斗资源
        if (BulletManager.Instance != null)
        {
            BulletManager.Instance.ResetAllBullets();
        }

        // 标记为从战斗返回
        MapManager.isReturningFromBattle = true;

        // 恢复地图上的玩家
        if (playerInstance != null)
        {
            playerInstance.SetActive(true);
            playerInstance = null;
        }

        // 卸载战斗场景
        SceneManager.UnloadSceneAsync(BattleSceneName);

        // 确保地图场景成为活跃场景
        Scene mapScene = SceneManager.GetSceneByName("MapScene");
        if (mapScene.isLoaded)
        {
            SceneManager.SetActiveScene(mapScene);
        }
        else
        {
            Debug.LogWarning("地图场景未加载");
        }

        // 恢复地图场景显示
        SceneHider.SetSceneActive("MapScene", true);
        Debug.Log($"{BattleSceneName}战斗结束！");

        // 战斗胜利后续逻辑
        BattleCompleted();
    }

    protected virtual void BattleCompleted()
    {
        Debug.Log("战斗胜利！");

        // 更新节点状态为已访问
        IsVisited = true;

        // 在这里可以添加奖励逻辑
    }
}
