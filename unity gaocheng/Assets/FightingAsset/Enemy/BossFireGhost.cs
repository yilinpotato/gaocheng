using UnityEngine;
using System.Collections;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class CircleSniper : Enemy
{
    [Header("扇形攻击设置")]
    [SerializeField] private int fanBulletCount = 7;
    [SerializeField] private float fanSpreadAngle = 90f;
    [SerializeField] private float fanAttackInterval = 3f;

    [Header("圆周攻击设置")]
    [SerializeField] private int circleBulletCount = 12;
    [SerializeField] private float rotateSpeed = 90f;
    [SerializeField] private float expandSpeed = 1f;
    [SerializeField] private float circleAttackInterval = 3f;
    private float attackTimer;
    private bool isFanNext = true; // 先扇形，再圆周，交替
    [Header("受击反馈")]
    [SerializeField] private Color hurtColor = Color.red;
    [SerializeField] private float hurtDuration = 0.1f;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    protected override void Start()
    {
        base.Start();
        LoadFromData(data); 
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

    }

    protected override void UpdateAIState()
    {
        if (isDead) return;

        attackTimer += Time.deltaTime;

        // 每隔 3 秒交替攻击
        float currentInterval = isFanNext ? fanAttackInterval : circleAttackInterval;
        if (attackTimer >= currentInterval)
        {
            attackTimer = 0f;

            if (isFanNext)
            {
                FireFanSpread();
            }
            else
            {
                StartCoroutine(FireRotatingCircle());
            }

            isFanNext = !isFanNext; // 交替攻击
        }
    }

    void FireFanSpread()
    {
        float startAngle = -fanSpreadAngle / 2;
        float angleStep = fanSpreadAngle / (fanBulletCount - 1);

        for (int i = 0; i < fanBulletCount; i++)
        {
            float angle = startAngle + angleStep * i;
            Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.down;
            ShootProjectile(dir);
        }
    }

    IEnumerator FireRotatingCircle()
    {
        CircularMotion[] bullets = new CircularMotion[circleBulletCount];
        Vector2 center = transform.position; // 记录当前中心点

        float angleStep = 360f / circleBulletCount;

        for (int i = 0; i < circleBulletCount; i++)
        {
            // 计算初始角度（基于玩家位置）
            Vector2 toPlayer = (Player.Instance.transform.position - transform.position).normalized;
            float baseAngle = Vector2.SignedAngle(Vector2.up, toPlayer);
            float startAngle = baseAngle + angleStep * i;

            // 发射子弹（方向随机，因为马上会覆盖运动）
            Projectile p = ShootProjectile(Random.insideUnitCircle.normalized);

            // 添加旋转组件并初始化
            bullets[i] = p.gameObject.AddComponent<CircularMotion>();
            bullets[i].Initialize(center, startAngle);
            bullets[i].rotateSpeed = this.rotateSpeed;
            bullets[i].expandSpeed = this.expandSpeed;

            // 移除自动销毁
            Destroy(p.GetComponent<AutoDestroy>());
        }

        // 控制持续时间
        yield return new WaitForSeconds(5f);

        // 销毁所有子弹
        foreach (var bullet in bullets)
        {
            if (bullet != null) Destroy(bullet.gameObject);
        }
    }

    // 受击反馈方法
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

// 圆周子弹行为组件
public class CircularMotion : MonoBehaviour
{
    // 旋转中心点（设置为敌人的位置）
    private Vector2 centerPoint;
    private float currentAngle;
    private float currentRadius;

    [Header("运动参数")]
    public float rotateSpeed = 90f;
    public float expandSpeed = 1f;

    // 初始化方法（由敌人调用）
    public void Initialize(Vector2 center, float startAngle)
    {
        centerPoint = center;
        currentRadius = 0f;
        currentAngle = startAngle;

        // 禁用原有线性运动
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
    }

    void Update()
    {
        // 更新运动参数
        currentRadius += expandSpeed * Time.deltaTime;
        currentAngle += rotateSpeed * Time.deltaTime;

        // 计算新位置
        Vector2 newPos = new Vector2(
            Mathf.Cos(currentAngle * Mathf.Deg2Rad),
            Mathf.Sin(currentAngle * Mathf.Deg2Rad)
        ) * currentRadius;

        // 应用位置（相对中心点）
        transform.position = centerPoint + newPos;
    }
}


