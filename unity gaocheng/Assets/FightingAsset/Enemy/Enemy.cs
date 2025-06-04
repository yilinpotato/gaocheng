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

        // 为死亡事件添加处理方法
        OnDeath.AddListener(HandleEnemyDeath);
    }

    // 添加的死亡处理方法
    private void HandleEnemyDeath()
    {
        if (battleNode != null)
        {
            Debug.Log("敌人死亡，准备结束战斗...");
            // 延迟一点时间再结束战斗，给死亡动画一些播放时间
            StartCoroutine(EndBattleAfterDelay(2.0f));
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
