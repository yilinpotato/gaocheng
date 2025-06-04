using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// ������Ϸʵ��Ļ���
/// �������������������Ժ�ͨ����Ϊ
/// </summary>
public abstract class Entity : MonoBehaviour
{
    //===================== �������� =====================
    [SerializeField]
    protected float maxHP = 100f;  // �������ֵ

    [SerializeField]
    protected float moveSpeed = 5f; // �ƶ��ٶ�

    [SerializeField]
    protected float attackPower = 10f; // ������

    protected float currentHP;      // ��ǰ����ֵ
    protected bool isDead;         // ����״̬���

    private bool hasRevive;
    //===================== �¼����� =====================
    public UnityEvent<float> OnDamageTaken;     // �����¼��������˺�ֵ��
    public UnityEvent OnDeath;                 // �����¼�

    //===================== ���Է����� =====================
    public float CurrentHP => currentHP;
    public float MaxHP => maxHP;
    public bool IsDead => isDead;
    public float MoveSpeed => moveSpeed;
    public float AttackPower => attackPower;

    public void SetMaxHP(float value)
    {
        maxHP = value;
        currentHP = Mathf.Min(currentHP, maxHP); // ���⵱ǰѪ������
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

    //===================== �������ڷ��� =====================
    protected virtual void Start()
    {
        Awake();
    }

    /// <summary>
    /// ��ʼ��ʵ�����״̬
    /// </summary>
    protected virtual void Awake()
    {
        currentHP = maxHP;
        isDead = false;
    }
    protected virtual void Update()
    {
       
    }
    //===================== ���ķ��� =====================
    /// <summary>
    /// ����ʵ���ܵ��˺�
    /// </summary>
    /// <param name="damage">�ܵ����˺�ֵ</param>
    public virtual void TakeDamage(float damage)
    {
        if (isDead) return;

        // ����ʵ���˺�
        float finalDamage = Mathf.Max(damage, 0);
        currentHP = Mathf.Clamp(currentHP - finalDamage, 0, maxHP);

        // ���������¼�
        OnDamageTaken?.Invoke(finalDamage);

        // ����UI��ͨ���¼����ߣ�
        EventBus.Publish(new DamageEvent(this, finalDamage));

        // �������
        if (currentHP <= Mathf.Epsilon)
        {
            Die();
        }
    }

    /// <summary>
    /// ����ʵ��
    /// </summary>
    /// <param name="healAmount">������</param>
    public virtual void Heal(float healAmount)
    {
        currentHP = Mathf.Clamp(currentHP + healAmount, 0, maxHP);
    }

    /// <summary>
    /// ���������ɱ�������д��
    /// </summary>
    protected virtual void Die()
    {
        isDead = true;

        // ���������¼�
        OnDeath?.Invoke();

        // ֪ͨ�¼�ϵͳ
        EventBus.Publish(new DeathEvent(this));

        // Ĭ����Ϊ��������Ϸ����
        gameObject.SetActive(false);
    }

    //===================== �ƶ���� =====================
    /// <summary>
    /// �����ƶ�����
    /// </summary>
    /// <param name="direction">��׼���ƶ�����</param>
    public virtual void Move(Vector2 direction)
    {
        if (isDead) return;

        Vector2 movement = direction * moveSpeed * Time.deltaTime;
        transform.Translate(movement);
    }
}

//===================== �����¼��� =====================
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

