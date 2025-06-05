using UnityEngine;
using System.Collections;

public class Boss : Enemy
{
    [Header("Boss专属设置")]
    [SerializeField] private float attackInterval = 2f;    // 攻击间隔
    [SerializeField] private float rotationSpeed = 15f;  // 弹幕旋转速度
    [SerializeField] private int bulletsPerWave = 36;     // 每波子弹数量
    [SerializeField] private int wavesPerAttack = 3;      // 每次攻击波数
    [Header("Boss特殊属性")]
    public float phase2HPThreshold = 0.5f;    // 进入二阶段的血量阈值
    public float enrageInterval = 10f;        // 狂暴状态间隔
    public float bulletAcceleration = 1.2f;   // 子弹加速度
    private float currentAngle;                          // 当前弹幕起始角度
    private Coroutine attackCoroutine;                    // 攻击协程引用
    [Header("受击反馈")]
    [SerializeField] private Color hurtColor = Color.red;
    [SerializeField] private float hurtDuration = 0.1f;
    [SerializeField] private Material hurtMaterial; // 新增材质引用
    private Material originalMaterial; // 存储原始材质
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    protected override void Start()
    {
        base.Start();
        FindPlayer();
        currentAngle = 0f;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    private void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj) player = playerObj.transform;
    }

    protected override void UpdateAIState()
    {
        if (attackCoroutine == null && !isDead)
        {
            attackCoroutine = StartCoroutine(AttackPattern());
        }
    }

    // 核心攻击模式协程
    private IEnumerator AttackPattern()
    {
        while (!isDead)
        {
            // 第一阶段：基础环形弹幕
            yield return StartCoroutine(CircularBarrage());

            // 第二阶段：旋转交叉弹幕
            yield return StartCoroutine(RotatingCrossBarrage());

            yield return new WaitForSeconds(attackInterval);
        }
    }

    // 基础环形弹幕
    private IEnumerator CircularBarrage()
    {
        for (int i = 0; i < wavesPerAttack; i++)
        {
            FireCircle(currentAngle, bulletsPerWave);
            currentAngle += rotationSpeed;
            yield return new WaitForSeconds(0.3f);
        }
    }

    // 旋转交叉弹幕
    private IEnumerator RotatingCrossBarrage()
    {
        int specialBullets = 8;
        float specialRotation = 0f;

        for (int i = 0; i < 4; i++)
        {
            // 交叉弹幕
            FireCircle(specialRotation, specialBullets);
            FireCircle(specialRotation + 22.5f, specialBullets); // 偏移角度形成交叉

            // 特殊弹幕旋转
            specialRotation += 45f;
            yield return new WaitForSeconds(0.4f);
        }
    }

    // 通用圆形弹幕生成方法
    private void FireCircle(float startAngle, int bulletCount)
    {
        float angleStep = 360f / bulletCount;
        for (int i = 0; i < bulletCount; i++)
        {
            float angle = startAngle + i * angleStep;
            Vector2 dir = CalculateDirection(angle);
            ShootProjectile(dir).SetSpeed(GetBulletSpeed(angle)); 
        }
    }

    // 动态速度控制（创建速度变化规律）
    private float GetBulletSpeed(float angle)
    {
        // 每45度区域使用不同速度
        float speedVariation = data.moveSpeed * (Mathf.Sin(angle * Mathf.Deg2Rad * 2) * 0.3f + 0.7f);
        return Mathf.Clamp(speedVariation, 3f, 8f);
    }

    private Vector2 CalculateDirection(float angle)
    {
        return new Vector2(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            Mathf.Sin(angle * Mathf.Deg2Rad)
        ).normalized;
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
    // 绘制攻击范围示意（Scene视图可见）
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 5f);
    }
}