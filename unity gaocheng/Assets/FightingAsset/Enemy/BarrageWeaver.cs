using UnityEngine;
using System.Collections;
public class BarrageWeaver : Enemy
{
    [Header("螺旋弹幕设置")]
    [SerializeField] private int spiralCount = 5;        // 弹幕数量
    [SerializeField] private float spiralSpeed = 180f;  // 旋转速度（度/秒）
    [SerializeField] private float spiralRadius = 1f;   // 初始半径
    [SerializeField] private float spiralInterval = 0.2f; // 发射间隔

    [Header("受击反馈")]
    [SerializeField] private Color hurtColor = Color.red;
    [SerializeField] private float hurtDuration = 0.1f;

    private float spiralTimer;
    private float sweepTimer;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    protected override void UpdateAIState()
    {
        HandleSpiralAttack();
    }

    void HandleSpiralAttack()
    {
        spiralTimer += Time.deltaTime;
        if (spiralTimer >= spiralInterval)
        {
            spiralTimer = 0;
            FireSpiral();
        }
    }



    void FireSpiral()
    {
        GameObject spiralBullet = BulletManager.Instance.GetBullet(
            BulletType.Enemy,
            transform.position,
            Quaternion.identity
        );

        if (spiralBullet == null)
        {
            Debug.LogError("螺旋弹幕预制体未找到！");
            return;
        }

        float currentAngle = Time.time * (-spiralSpeed); // 负号是关键
        float angleStep = 360f / spiralCount;

        for (int i = 0; i < spiralCount; i++)
        {
            float angle = (currentAngle + angleStep * i) % 360;
            Vector2 offset = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            ) * spiralRadius;

            Projectile p = Instantiate(spiralBullet,
                (Vector2)transform.position + offset,
                Quaternion.identity).GetComponent<Projectile>();

            p.Initialize(transform, data.attackDamage, offset.normalized);
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
}