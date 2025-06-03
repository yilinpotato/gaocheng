using UnityEngine;
using System.Collections;
public abstract class Enemy : Entity
{
    [Header("敌人基础设置")]
    [SerializeField] protected EnemyData data;          // ScriptableObject配置
    [SerializeField] protected LayerMask playerLayer;   // 玩家层级
    [SerializeField] protected Projectile projectilePrefab; // 子弹预制体
    protected Transform player;                         // 玩家引用
    protected EnemyState currentState;                  // 当前AI状态
    protected override void Awake()
    {
        // 先让父类把 currentHP = maxHP
        base.Awake();

        // 再把 Data 里的值写进父类字段
        if (data != null)
        {
            maxHP = data.baseHP;
            moveSpeed = data.moveSpeed;
            attackPower = data.attackDamage;
        }
        // 最后把 currentHP 同步到新的 maxHP
        currentHP = maxHP;

    }
    protected override void Start()
    {
        base.Start();
        LoadFromData(data);
    }

    protected override void Update()
    {
        if (isDead) return;
        UpdateAIState();
    }

    // AI状态机核心逻辑
    protected abstract void UpdateAIState();

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
