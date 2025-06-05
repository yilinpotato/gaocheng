using System.Collections;
using System.Collections.Generic;
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

    [Header("第二阶段：弹幕攻击")]
    [SerializeField] private int fanProjectileCount = 7;
    [SerializeField] private float fanSpreadAngle = 60f;
    [SerializeField] private float phase2AttackCooldown = 2.5f; // 弹幕冷却时间
    [SerializeField] private int ringProjectileCount = 12;      // 环形弹幕数量
    [SerializeField] private float ringSpeed = 4f;              // 环形弹幕速度
    [SerializeField] private int spiralProjectileCount = 24;   // 螺旋弹幕总数
    [SerializeField] private float spiralInterval = 0.1f;      // 螺旋弹幕间隔
    [SerializeField] private float spiralAngleStep = 15f;      // 螺旋角度步长

    [Header("受击反馈")]
    [SerializeField] private Color hurtColor = Color.red;
    [SerializeField] private float hurtDuration = 0.1f;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    // 攻击模式追踪
    private enum AttackPattern { Fan, Ring, Spiral }
    private AttackPattern nextAttackPattern = AttackPattern.Fan;
    private bool isPhase2 = false;
    private bool isTransitioning = false;
    private float phase2AttackTimer = 0f;

    // 弹幕发射点
    [SerializeField] private Transform projectileSpawnPoint;

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
    }

    private IEnumerator TransitionToNextPhase()
    {
        isTransitioning = true;
        animator.SetTrigger("DoTransforming");
        yield return new WaitForSeconds(phaseTransitionTime);
        isPhase2 = true;
        isTransitioning = false;

        // 初始化第二阶段攻击模式
        nextAttackPattern = AttackPattern.Fan;
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

            // 根据攻击模式触发不同弹幕
            switch (nextAttackPattern)
            {
                case AttackPattern.Fan:
                    animator.SetTrigger("DoAttack1");
                    FireFan();
                    nextAttackPattern = AttackPattern.Ring;
                    break;

                case AttackPattern.Ring:
                    animator.SetTrigger("DoAttack1");
                    FireRing();
                    nextAttackPattern = AttackPattern.Spiral;
                    break;

                case AttackPattern.Spiral:
                    animator.SetTrigger("DoAttack2");
                    StartCoroutine(FireSpiral());
                    nextAttackPattern = AttackPattern.Fan;
                    break;
            }
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
                Quaternion.Euler(0, 0, 270f) // 向下发射
            );

            if (bulletObj.TryGetComponent<Projectile>(out var projectile))
            {
                projectile.Initialize(transform, data.attackDamage, Vector2.down);
            }
        }
    }

    private void FireFan()
    {
        float startAngle = 270f - fanSpreadAngle / 2f;
        float delta = fanSpreadAngle / (fanProjectileCount - 1);

        for (int i = 0; i < fanProjectileCount; i++)
        {
            float angle = startAngle + delta * i;
            Vector2 dir = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );

            GameObject bulletObj = BulletManager.Instance.GetBullet(
                BulletType.Enemy,
                projectileSpawnPoint.position,
                Quaternion.Euler(0, 0, angle - 90f) // 旋转子弹方向
            );

            if (bulletObj.TryGetComponent<Projectile>(out var projectile))
            {
                projectile.Initialize(transform, data.attackDamage, dir);
            }
        }
    }

    private void FireRing()
    {
        float angleStep = 360f / ringProjectileCount;

        for (int i = 0; i < ringProjectileCount; i++)
        {
            float angle = i * angleStep;
            Vector2 dir = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );

            GameObject bulletObj = BulletManager.Instance.GetBullet(
                BulletType.Enemy,
                transform.position,
                Quaternion.Euler(0, 0, angle - 90f)
            );

            if (bulletObj.TryGetComponent<Projectile>(out var projectile))
            {
                projectile.Initialize(transform, data.attackDamage, dir * ringSpeed);
            }
        }
    }

    private IEnumerator FireSpiral()
    {
        float currentAngle = 0f;

        for (int i = 0; i < spiralProjectileCount; i++)
        {
            Vector2 dir = new Vector2(
                Mathf.Cos(currentAngle * Mathf.Deg2Rad),
                Mathf.Sin(currentAngle * Mathf.Deg2Rad)
            );

            GameObject bulletObj = BulletManager.Instance.GetBullet(
                BulletType.Enemy,
                transform.position,
                Quaternion.Euler(0, 0, currentAngle - 90f)
            );

            if (bulletObj.TryGetComponent<Projectile>(out var projectile))
            {
                projectile.Initialize(transform, data.attackDamage, dir);
            }

            currentAngle += spiralAngleStep;
            yield return new WaitForSeconds(spiralInterval);
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
        // 如果已经死亡，不再执行后续逻辑
        if (isDead) return;

        // 原有的 TakeDamage 逻辑
        base.TakeDamage(damage);

        // 如果此时已经死亡（被上面的 base.TakeDamage 杀死），就不要再启动协程
        if (isDead) return;

        // 启动协程的代码...
        StartCoroutine(HurtEffect());
    }
    IEnumerator HurtEffect()
    {
        spriteRenderer.color = hurtColor;
        yield return new WaitForSeconds(hurtDuration);
        spriteRenderer.color = originalColor;
    }
}

