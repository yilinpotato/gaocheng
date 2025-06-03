using System.Collections;
using UnityEngine;

public class BossEnemy : Enemy
{
    private Animator animator;
    [Header("阶段设置")]
    [SerializeField] private float phaseTransitionThreshold = 0.5f;  // 50%血量转换阶段
    [SerializeField] private float phaseTransitionTime = 0.11f;

    [Header("通用移动设置")]
    [SerializeField] private float patrolMinX = -3f;
    [SerializeField] private float patrolMaxX = 3f;
    [SerializeField] private float patrolSpeed = 2f;
    private bool movingRight = true;
    [Header("第一阶段：柱状弹幕")]
    [SerializeField] private int columnProjectileCount = 5;   // 弹幕数量
    [SerializeField] private float columnSpacing = 0.5f;      // 子弹间距
    [SerializeField] private float columnAttackCooldown = 3f;
    private float columnAttackTimer = 0f;

    [Header("第二阶段：扇形弹幕")]
    [SerializeField] private int fanProjectileCount = 7;
    [SerializeField] private float fanSpreadAngle = 60f;
    private float phase2AttackTimer = 0f;
    private float phase2AttackCooldown = 2.5f; // 激光和扇形弹幕的总冷却时间
    private bool useLaserNext = true;
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private Transform laserSpawnPoint;
    private bool isPhase2 = false;
    private bool isTransitioning = false;
    [Header("受击反馈")]
    [SerializeField] private Color hurtColor = Color.red;
    [SerializeField] private float hurtDuration = 0.1f;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    protected override void Start()
    {
        base.Start();
        LoadFromData(data); 
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    protected override void UpdateAIState()
    {
        if (isTransitioning) return;

        if (!isPhase2 && currentHP <= maxHP * phaseTransitionThreshold)
        {
            StartCoroutine(TransitionToNextPhase());
            return;
        }

        HorizontalPatrol();

        if (isPhase2)
        {
            Phase2Behavior();
        }
        else
        {
            Phase1Behavior();
        }
        Debug.Log($"当前阶段: {(isPhase2 ? "Phase2" : "Phase1")} 血量: {currentHP}/{maxHP}");
    }


    private IEnumerator TransitionToNextPhase()
    {
        isTransitioning = true;
        animator.SetTrigger("DoTransforming");
        yield return new WaitForSeconds(phaseTransitionTime);
        isPhase2 = true;
        isTransitioning = false;
    }

    private void HorizontalPatrol()
    {
        Vector3 pos = transform.position;
        pos.x += (movingRight ? 1f : -1f) * patrolSpeed * Time.deltaTime;

        if (pos.x >= patrolMaxX)
        {
            pos.x = patrolMaxX;
            movingRight = false;
        }
        else if (pos.x <= patrolMinX)
        {
            pos.x = patrolMinX;
            movingRight = true;
        }

        transform.position = pos;
    }

    private void Phase1Behavior()
    {
        // 全屏柱状弹幕攻击
        columnAttackTimer += Time.deltaTime;
        if (columnAttackTimer >= columnAttackCooldown)
        {
            columnAttackTimer = 0f;
            FireColumn();
        }
    }

    private void Phase2Behavior()
    {
        phase2AttackTimer += Time.deltaTime;

        if (phase2AttackTimer >= phase2AttackCooldown)
        {
            phase2AttackTimer = 0f;

            if (useLaserNext)
            {
                FireLaser();
            }
            else
            {
                FireFan();
            }

            useLaserNext = !useLaserNext; // 每次切换攻击类型
        }
    }

    private void FireColumn()
    {
        // 计算起始位置
        Vector2 startPos = transform.position; // 与 Boss 同一水平面


        for (int i = 0; i < columnProjectileCount; i++)
        {
            Vector2 spawnPos = startPos + Vector2.right * (i * columnSpacing - columnSpacing * (columnProjectileCount - 1) / 2f);

            GameObject bulletObj = BulletManager.Instance.GetBullet(
                BulletType.Enemy,
                spawnPos,
                Quaternion.Euler(0, 0, 270f)
            );
            if (bulletObj.TryGetComponent<Projectile>(out var projectile))
            {
                projectile.Initialize(transform, data.attackDamage, Vector2.down);
            }
            else
            {
                Debug.LogError("子弹预制体上没有 Projectile 组件！");
            }
        }
    }

    private void FireFan()
    {
        if (animator != null)
            animator.SetTrigger("DoAttack1");

        float startAngle = 270f - fanSpreadAngle / 2f;
        float delta = fanSpreadAngle / (fanProjectileCount - 1);
        for (int i = 0; i < fanProjectileCount; i++)
        {
            float angle = startAngle + delta * i;
            Vector2 dir = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );
            ShootProjectile(dir);
        }
    }

    private void FireLaser()
    {
        if (animator != null)
            animator.SetTrigger("DoAttack2");

        if (laserPrefab != null)
        {
            Instantiate(laserPrefab, laserSpawnPoint.position, laserSpawnPoint.rotation);
        }
    }


    private void OnDrawGizmosSelected()//调试使用
    {
        // 绘制移动范围
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(
            new Vector3(patrolMinX, transform.position.y - 1f),
            new Vector3(patrolMaxX, transform.position.y - 1f)
        );

        // 绘制扇形弹幕生成区域（第一阶段）
        if (!isPhase2)
        {
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 2f, fanSpreadAngle);
        }

        // 绘制柱状弹幕生成区域（第二阶段）
        if (isPhase2)
        {
            Gizmos.DrawWireCube(
                transform.position + Vector3.up * 2f,
                new Vector3(columnSpacing * 4, 0.5f)
            );
        }
    }
    protected override void Die()
    {
        base.Die();
        if (animator != null)
        {
            animator.SetTrigger("Dead");
        }

        
        Destroy(gameObject, 3f); // 等待死亡动画播放完成
    }
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        StartCoroutine(HurtEffect());
    }
    IEnumerator HurtEffect()
    {
        spriteRenderer.color = hurtColor;
        yield return new WaitForSeconds(hurtDuration);
        spriteRenderer.color = originalColor;
    }
}

