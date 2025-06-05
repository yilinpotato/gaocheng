using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // 添加这行用于场景管理

public class MomHeart : Enemy
{
    private enum AttackPattern
    {
        Circle,
        Spiral,
        DualRingStar,  // 替换激光的新型弹幕攻击
    }

    [Header("持续时间设置")]
    [SerializeField] private float circleDuration = 5f;       // 环形弹幕持续时间
    [SerializeField] private int circleWaveCount = 3;        // 环形弹幕波次
    [SerializeField] private float spiralDuration = 8f;      // 螺旋弹幕持续时间
    [SerializeField] private float starBurstDuration = 4f;  // 星爆弹幕持续时间

    [Header("攻击顺序设置")]
    [SerializeField] private AttackPattern[] phase1Patterns = { AttackPattern.Circle, AttackPattern.Spiral };
    [SerializeField] private AttackPattern[] phase2Patterns = { AttackPattern.Spiral, AttackPattern.DualRingStar };

    [Header("阶段设置")]
    [SerializeField] private float phase2Threshold = 0.5f;
    [SerializeField] private bool isPhase2 = false;

    [Header("通用弹幕设置")]
    [SerializeField] private Transform[] muzzlePoints;  // 发射点数组
    [SerializeField] private float attackInterval = 2f; // 攻击模式间隔
    [SerializeField] private float bulletSpeed = 5f;    // 通用子弹速度

    [Header("环形弹幕")]
    [SerializeField] private int circleBulletCount = 12; // 每波子弹数量

    [Header("螺旋弹幕")]
    [SerializeField] private int spiralArmCount = 4;    // 螺旋臂数量
    [SerializeField] private float spiralRotateSpeed = 180f; // 螺旋旋转速度
    [SerializeField] private float spiralFireRate = 0.15f;  // 子弹发射间隔

    [Header("双环星爆弹幕")]
    [SerializeField] private int innerRingBullets = 12;    // 内环子弹数
    [SerializeField] private int outerRingBullets = 16;    // 外环子弹数
    [SerializeField] private float ringRotationSpeed = 90f; // 环旋转速度
    [SerializeField] private int starBursts = 4;           // 星爆次数
    [SerializeField] private float burstCooldown = 0.5f;    // 星爆间冷却
    [SerializeField] private float starBulletSpeed = 7f;    // 星爆子弹速度
    [SerializeField] private float ringDistance = 1.5f;    // 环距离Boss中心

    private int currentPatternIndex;     // 当前攻击模式索引
    private Coroutine currentAttack;     // 当前攻击协程

    [Header("受击反馈")]
    [SerializeField] private Color hurtColor = Color.red;
    [SerializeField] private float hurtDuration = 0.1f;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private float[] ringAngles = new float[2]; // 内外环当前角度

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        // 初始化双环角度
        ringAngles[0] = 0f;  // 内环
        ringAngles[1] = 45f; // 外环

        // 确保发射点数组有内容
        if (muzzlePoints == null || muzzlePoints.Length == 0)
        {
            muzzlePoints = new Transform[1];
            muzzlePoints[0] = transform;
            Debug.LogWarning("No muzzle points assigned, using Boss transform as default");
        }
    }

    protected override void UpdateAIState()
    {
        // 确保播放死亡动画时停止AI行为
        if (isDead) return;

        // 处理阶段转换
        if (!isPhase2 && currentHP <= maxHP * phase2Threshold)
        {
            isPhase2 = true;
            Debug.Log("进入第二阶段！");
        }

        // 确保只有一个攻击协程运行
        if (currentAttack == null)
        {
            AttackPattern[] patterns = GetCurrentPatterns();
            currentAttack = StartCoroutine(ExecuteAttack(patterns));
        }
    }

    IEnumerator ExecuteAttack(AttackPattern[] patterns)
    {
        while (!isDead) // 确保死亡后停止攻击
        {
            AttackPattern pattern = patterns[currentPatternIndex];
            Debug.Log($"发动攻击模式：{pattern}");

            // 执行当前攻击模式
            yield return StartCoroutine(GetAttackCoroutine(pattern));

            // 准备下一轮攻击
            currentPatternIndex = (currentPatternIndex + 1) % patterns.Length;

            // 休息后继续攻击
            yield return new WaitForSeconds(attackInterval);
        }
    }

    AttackPattern[] GetCurrentPatterns()
    {
        // 第二阶段采用增强攻击模式
        return isPhase2 ? phase2Patterns : phase1Patterns;
    }

    IEnumerator GetAttackCoroutine(AttackPattern pattern)
    {
        switch (pattern)
        {
            case AttackPattern.Circle:
                yield return StartCoroutine(CircleAttack());
                break;

            case AttackPattern.Spiral:
                yield return StartCoroutine(SpiralAttack());
                break;

            case AttackPattern.DualRingStar:
                yield return StartCoroutine(DualRingStarAttack());
                break;
        }
    }

    IEnumerator CircleAttack()
    {
        float waveInterval = circleDuration / circleWaveCount;

        for (int wave = 0; wave < circleWaveCount; wave++)
        {
            float angleStep = 360f / circleBulletCount;

            // 从每个发射点发射环形弹幕
            foreach (var muzzle in muzzlePoints)
            {
                Vector2 spawnPos = muzzle.position;

                for (int i = 0; i < circleBulletCount; i++)
                {
                    float angle = angleStep * i;
                    Vector2 dir = new Vector2(
                        Mathf.Cos(angle * Mathf.Deg2Rad),
                        Mathf.Sin(angle * Mathf.Deg2Rad)
                    );

                    ShootProjectileAtPosition(spawnPos, dir, bulletSpeed);
                }
            }
            yield return new WaitForSeconds(waveInterval);
        }
    }

    IEnumerator SpiralAttack()
    {
        float startTime = Time.time;
        float currentAngle = 0f;

        while (Time.time - startTime < spiralDuration && !isDead)
        {
            currentAngle += spiralRotateSpeed * Time.deltaTime;

            foreach (var muzzle in muzzlePoints)
            {
                Vector2 spawnPos = muzzle.position;

                for (int j = 0; j < spiralArmCount; j++)
                {
                    float armAngle = currentAngle + (j * 360f / spiralArmCount);
                    Vector2 dir = new Vector2(
                        Mathf.Cos(armAngle * Mathf.Deg2Rad),
                        Mathf.Sin(armAngle * Mathf.Deg2Rad)
                    );

                    ShootProjectileAtPosition(spawnPos, dir, bulletSpeed);
                }
            }

            yield return new WaitForSeconds(spiralFireRate);
        }
    }

    IEnumerator DualRingStarAttack()
    {
        float startTime = Time.time;

        while (Time.time - startTime < starBurstDuration && !isDead)
        {
            // 每n秒触发一次星爆
            for (int burst = 0; burst < starBursts; burst++)
            {
                // 发射双环
                FireRingProjectiles();

                // 星爆子弹从各个方向射出
                foreach (var muzzle in muzzlePoints)
                {
                    FireStarBurstFromPoint(muzzle.position);
                }

                // 更新双环角度
                ringAngles[0] += ringRotationSpeed * burstCooldown;
                ringAngles[1] -= ringRotationSpeed * burstCooldown;

                yield return new WaitForSeconds(burstCooldown);

                // 如果死亡则立即退出
                if (isDead) yield break;
            }
        }
    }

    // 发射双环弹幕
    void FireRingProjectiles()
    {
        // 内环子弹
        for (int i = 0; i < innerRingBullets; i++)
        {
            float angle = ringAngles[0] + i * (360f / innerRingBullets);
            Vector2 dir = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );

            Vector2 spawnPos = (Vector2)transform.position + dir * ringDistance;
            ShootProjectileAtPosition(spawnPos, dir, bulletSpeed);
        }

        // 外环子弹
        for (int i = 0; i < outerRingBullets; i++)
        {
            float angle = ringAngles[1] + i * (360f / outerRingBullets);
            Vector2 dir = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );

            Vector2 spawnPos = (Vector2)transform.position + dir * (ringDistance * 1.5f);
            ShootProjectileAtPosition(spawnPos, dir, bulletSpeed);
        }
    }

    // 星爆子弹（从某点向各个方向发射）
    void FireStarBurstFromPoint(Vector2 origin)
    {
        int starBulletCount = 8;  // 8方向
        for (int i = 0; i < starBulletCount; i++)
        {
            float angle = i * (360f / starBulletCount);
            Vector2 dir = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );

            ShootProjectileAtPosition(origin, dir, starBulletSpeed);
        }
    }

    // 增强的子弹发射方法（兼容基类并添加位置参数）
    private void ShootProjectileAtPosition(Vector2 spawnPos, Vector2 direction, float speed)
    {
        float bulletAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        GameObject bulletObj = BulletManager.Instance.GetBullet(
            BulletType.Enemy,
            spawnPos,
            Quaternion.Euler(0, 0, bulletAngle - 90f)
        );

        if (bulletObj != null && bulletObj.TryGetComponent<Projectile>(out var projectile))
        {
            projectile.Initialize(transform, data.attackDamage, direction * speed);
        }
        else
        {
            Debug.LogError("Failed to create projectile");
        }
    }

    protected override void Die()
    {
        // 先执行基础的死亡逻辑，但不调用Entity的Die()来避免直接场景切换
        isDead = true;
        StopAllCoroutines();

        // 触发死亡事件
        OnDeath?.Invoke();

        // 通知事件系统
        EventBus.Publish(new DeathEvent(this));

        // Boss特有逻辑：显示结算面板而不是直接返回地图
        ShowVictoryPanel();

        // 禁用游戏对象
        gameObject.SetActive(false);
    }

    private void ShowVictoryPanel()
    {
        // 直接加载"结束面板"场景
        try
        {
            Debug.Log("Boss defeated! Loading victory scene...");
            UnityEngine.SceneManagement.SceneManager.LoadScene("结算面板");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load victory scene '结算面板': {e.Message}");
            // 如果加载结束面板失败，回退到直接返回地图
            ReturnToMap();
        }
    }

    private void ReturnToMap()
    {
        forTestButton buttonController = FindObjectOfType<forTestButton>();
        if (buttonController != null)
        {
            buttonController.ReturnToMapScene();
        }
        else
        {
            Debug.LogWarning("ForTestButton not found, cannot return to map");
        }
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

    // 可视化的调试信息
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        // 绘制双环
        Gizmos.color = Color.cyan;
        DrawRingGizmo(transform.position, ringDistance, ringAngles[0]);
        DrawRingGizmo(transform.position, ringDistance * 1.5f, ringAngles[1]);

        // 绘制发射点
        if (muzzlePoints != null)
        {
            Gizmos.color = Color.yellow;
            foreach (var muzzle in muzzlePoints)
            {
                if (muzzle != null)
                {
                    Gizmos.DrawWireSphere(muzzle.position, 0.2f);
                }
            }
        }
    }

    private void DrawRingGizmo(Vector3 center, float radius, float angleOffset)
    {
        int segments = 36;
        float angleStep = 360f / segments;

        Vector3 prevPoint = center + Quaternion.Euler(0, 0, angleOffset) * Vector3.up * radius;
        for (int i = 1; i <= segments; i++)
        {
            float angle = angleOffset + i * angleStep;
            Vector3 point = center + Quaternion.Euler(0, 0, angle) * Vector3.up * radius;
            Gizmos.DrawLine(prevPoint, point);
            prevPoint = point;
        }
    }
}