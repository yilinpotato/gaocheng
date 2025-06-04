using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// 所有游戏实体的基类
/// 包含生命管理、基础属性和通用行为
/// </summary>
public abstract class Entity : MonoBehaviour
{
    //===================== 基础属性 =====================
    [SerializeField]
    protected float maxHP = 100f;  // 最大生命值

    [SerializeField]
    protected float moveSpeed = 5f; // 移动速度

    [SerializeField]
    protected float attackPower = 10f; // 攻击力

    protected float currentHP;      // 当前生命值
    protected bool isDead;         // 死亡状态标记

    private bool hasRevive;
    //===================== 事件定义 =====================
    public UnityEvent<float> OnDamageTaken;     // 受伤事件（传递伤害值）
    public UnityEvent OnDeath;                 // 死亡事件

    //===================== 属性访问器 =====================
    public float CurrentHP => currentHP;
    public float MaxHP => maxHP;
    public bool IsDead => isDead;
    public float MoveSpeed => moveSpeed;
    public float AttackPower => attackPower;

    public void SetMaxHP(float value)
    {
        maxHP = value;
        currentHP = Mathf.Min(currentHP, maxHP); // 避免当前血量超出
    }

    public void SetCurrentHP(float value)
    {
        currentHP = Mathf.Clamp(value, 0, maxHP);
    }

    public void SetAttackPower(float value)
    {
        attackPower = value;
    }

    public void SetMoveSpeed(float value)
    {
        moveSpeed = value;
    }
    public void SetRevive(bool value)
    {
        hasRevive = value;
    }

    public virtual void LoadFromData(EnemyData data)
    {
        SetMaxHP(data.baseHP);
        SetMoveSpeed(data.moveSpeed);
        SetAttackPower(data.attackDamage);
    }

    //===================== 生命周期方法 =====================
    protected virtual void Start()
    {
        Awake();
    }

    /// <summary>
    /// 初始化实体基础状态
    /// </summary>
    protected virtual void Awake()
    {
        currentHP = maxHP;
        isDead = false;
    }
    protected virtual void Update()
    {
       
    }
    //===================== 核心方法 =====================
    /// <summary>
    /// 处理实体受到伤害
    /// </summary>
    /// <param name="damage">受到的伤害值</param>
    public virtual void TakeDamage(float damage)
    {
        if (isDead) return;

        // 计算实际伤害
        float finalDamage = Mathf.Max(damage, 0);
        currentHP = Mathf.Clamp(currentHP - finalDamage, 0, maxHP);

        // 触发受伤事件
        OnDamageTaken?.Invoke(finalDamage);

        // 更新UI（通过事件总线）
        EventBus.Publish(new DamageEvent(this, finalDamage));

        // 检查死亡
        if (currentHP <= Mathf.Epsilon)
        {
            Die();
        }
    }

    /// <summary>
    /// 治疗实体
    /// </summary>
    /// <param name="healAmount">治疗量</param>
    public virtual void Heal(float healAmount)
    {
        currentHP = Mathf.Clamp(currentHP + healAmount, 0, maxHP);
    }

    /// <summary>
    /// 死亡处理（可被子类重写）
    /// </summary>
    protected virtual void Die()
    {
        isDead = true;

        // 触发死亡事件
        OnDeath?.Invoke();

        // 通知事件系统
        EventBus.Publish(new DeathEvent(this));

        // 默认行为：禁用游戏对象
        gameObject.SetActive(false);
    }

    //===================== 移动相关 =====================
    /// <summary>
    /// 基础移动方法
    /// </summary>
    /// <param name="direction">标准化移动方向</param>
    public virtual void Move(Vector2 direction)
    {
        if (isDead) return;

        Vector2 movement = direction * moveSpeed * Time.deltaTime;
        transform.Translate(movement);
    }
}

//===================== 配套事件类 =====================
public class DamageEvent
{
    public Entity Target;
    public float Damage;

    public DamageEvent(Entity target, float damage)
    {
        Target = target;
        Damage = damage;
    }
}

public class DeathEvent
{
    public Entity Deceased;

    public DeathEvent(Entity deceased)
    {
        Deceased = deceased;
    }
}

