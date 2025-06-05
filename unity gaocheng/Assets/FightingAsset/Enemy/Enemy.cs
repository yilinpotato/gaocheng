using UnityEngine;
using System.Collections;
public abstract class Enemy : Entity
{
    [Header("敌人基础属性")]
    [SerializeField] protected EnemyData data;
    [SerializeField] protected LayerMask playerLayer;
    protected Transform player;
    protected EnemyState currentState;
    protected BattleNode battleNode;

    protected Entity playerEntity;

    public virtual void SetBattleNode(BattleNode node)
    {
        battleNode = node;
    }

    protected override void Awake()
    {
        base.Awake();

        if (data != null)
        {
            maxHP = data.baseHP;
            moveSpeed = data.moveSpeed;
            attackPower = data.attackDamage;
            currentHP = maxHP;
        }
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerEntity = playerObject.GetComponent<Entity>();
            if (playerEntity == null)
            {
                Debug.LogWarning("找到玩家对象，但没有Entity组件");
            }
        }
        else
        {
            Debug.LogWarning("无法找到玩家对象");
        }
        // 为死亡事件添加处理方法
        OnDeath.AddListener(HandleEnemyDeath);
    }

    private void SavePlayerCurrentHP()
    {
        if (playerEntity != null)
        {
            float playerCurrentHP = playerEntity.CurrentHP;
            Debug.Log($"保存玩家当前血量: {playerCurrentHP}");

            try
            {
                // 尝试通过 LoadManager 和 SaveManager 直接保存，不依赖 GameController
                if (LoadManager.Instance != null && SaveManager.Instance != null)
                {
                    PlayerData currentPlayerData = LoadManager.Instance.GetCurrentPlayerData();
                    if (currentPlayerData != null)
                    {
                        currentPlayerData.health = playerCurrentHP;
                        SaveManager.Instance.QuickSave(currentPlayerData);
                        Debug.Log("玩家数据已保存，血量更新为: " + playerCurrentHP);
                    }
                }
                else
                {
                    Debug.LogWarning("LoadManager 或 SaveManager 实例不存在");
                }
            }
            catch (System.Exception e)
            {
                // 捕获任何可能的错误，确保不影响场景切换
                Debug.LogError($"保存玩家血量时出错: {e.Message}");
            }
        }
    }

    // 添加的死亡处理方法
    private void HandleEnemyDeath()
    {
        if (battleNode != null)
        {
            Debug.Log("敌人死亡，准备结束战斗...");

            // 直接使用 forTestButton 的方式返回地图
            StartCoroutine(ReturnToMapAfterDelay(2.0f));

            // 如果仍然需要通过 battleNode 结束战斗，可以保留下面的代码
            // StartCoroutine(EndBattleAfterDelay(2.0f));
        }
    }

    private IEnumerator ReturnToMapAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 跟进 forTestButton 的方法，直接加载地图场景
        string mapSceneName = "MapScene";
        Debug.Log("[调试] 开始执行返回地图流程...");

        try
        {
            // 直接尝试加载场景 - 不再使用 SceneUtility
            Debug.Log($"[调试] 直接尝试加载场景: {mapSceneName}");
            UnityEngine.SceneManagement.SceneManager.LoadScene(mapSceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[调试] 返回地图场景时出错: {e.Message}\n{e.StackTrace}");
        }
    }

    private IEnumerator EndBattleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (battleNode != null)
        {
            battleNode.EndBattle();
        }
    }

    protected override void Start()
    {
        base.Start();
        if (data != null) LoadFromData(data);
    }

    protected override void Update()
    {
        if (isDead) return;
        UpdateAIState();
    }

    // AI状态更新逻辑
    protected abstract void UpdateAIState();

    protected override void Die()
    {
        base.Die();
        SavePlayerCurrentHP();
        StopAllCoroutines();
    }

    // 发射弹幕通用方法
    protected Projectile ShootProjectile(Vector2 direction)
    {
        GameObject bulletObj = BulletManager.Instance.GetBullet(
          BulletType.Enemy,
          transform.position,
          Quaternion.identity
        );
        if (bulletObj.TryGetComponent<Projectile>(out var projectile))
        {
            projectile.Initialize(transform, data.attackDamage, direction);
        }
        return projectile;
    }
}
