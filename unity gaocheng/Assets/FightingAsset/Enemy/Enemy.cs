using UnityEngine;
using System.Collections;
public abstract class Enemy : Entity
{
    [Header("���˻�������")]
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

        // Ϊ�����¼���Ӵ�����
        OnDeath.AddListener(HandleEnemyDeath);
    }

    // ��ӵ�����������
    private void HandleEnemyDeath()
    {
        if (battleNode != null)
        {
            Debug.Log("����������׼������ս��...");
            // �ӳ�һ��ʱ���ٽ���ս��������������һЩ����ʱ��
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

    // AI״̬�����߼�
    protected abstract void UpdateAIState();

    protected override void Die()
    {
        base.Die();
        StopAllCoroutines();
    }

    // ���䵯Ļͨ�÷���
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
