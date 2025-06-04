using UnityEngine;
using System.Collections;
public abstract class Enemy : Entity
{
    [Header("敌人基础设置")]
    [SerializeField] protected EnemyData data;          // ScriptableObject配置
    [SerializeField] protected LayerMask playerLayer;   // 玩家层级
    protected Transform player;                         // 玩家引用
    protected EnemyState currentState;                  // 当前AI状态
    protected BattleNode battleNode;                   // 改为protected以允许子类访问

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

    // AI状态机核心逻辑
    protected abstract void UpdateAIState();

    protected override void Die()
    {
        base.Die();
        StopAllCoroutines();
    }
// 发射弹幕的通用方法
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