using UnityEngine;
using System.Collections;

public class MomHeart : Enemy
{

    private enum AttackPattern
    {
        Circle,
        Spiral,
        Laser,
    }
    [Header("持续时间设置")]
    [SerializeField] private float circleDuration = 5f;       // 环形弹幕持续时间
    [SerializeField] private int circleWaveCount = 3;        // 环形弹幕波次
    [SerializeField] private float spiralDuration = 8f;      // 螺旋弹幕持续时间
    [SerializeField] private float laserDuration = 3f;       // 激光持续时间
    [Header("攻击顺序设置")]
    [SerializeField] private AttackPattern[] phase1Patterns = { AttackPattern.Circle, AttackPattern.Spiral };
    [SerializeField] private AttackPattern[] phase2Patterns = { AttackPattern.Circle, AttackPattern.Spiral, AttackPattern.Laser };
    [Header("阶段设置")]
    [SerializeField] private float phase2Threshold = 0.5f;

    [Header("通用弹幕设置")]
    [SerializeField] private Transform[] muzzlePoints;
    [SerializeField] private float attackInterval = 2f;

    [Header("环形弹幕")]
    [SerializeField] private int circleBulletCount = 12;
    [SerializeField] private float circleSpeed = 5f;

    [Header("螺旋弹幕")]
    [SerializeField] private int spiralArmCount = 4;
    [SerializeField] private float spiralRotateSpeed = 180f;
    private int currentPatternIndex;
    private Coroutine currentAttack;
    [Header("交叉激光")]
    [SerializeField] private GameObject laserPrefab;

    [Header("受击反馈")]
    [SerializeField] private Color hurtColor = Color.red;
    [SerializeField] private float hurtDuration = 0.1f;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isPhase2;

    protected override void Start()
    {
        base.Start();
        LoadFromData(data);
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    protected override void UpdateAIState()
    {
        HandlePhaseTransition();

        if (currentAttack == null)
        {
            AttackPattern[] patterns = GetCurrentPatterns();
            currentAttack = StartCoroutine(ExecuteAttack(patterns));
        }        
    }

    void HandlePhaseTransition()
    {
        float hpPercent = currentHP / maxHP;
        isPhase2 = hpPercent <= phase2Threshold;
    }

    IEnumerator ExecuteAttack(AttackPattern[] patterns)
    {
        while (true)
        {
            AttackPattern pattern = patterns[currentPatternIndex];
            yield return StartCoroutine(GetAttackCoroutine(pattern));

            currentPatternIndex = (currentPatternIndex + 1) % patterns.Length;
            yield return new WaitForSeconds(attackInterval);
        }
    }

    AttackPattern[] GetCurrentPatterns()
    {
        if (isPhase2) return phase2Patterns;
        return phase1Patterns;
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

            case AttackPattern.Laser:
                yield return StartCoroutine(CrossLaserAttack());
                break;

        }
    }

    IEnumerator CircleAttack()
    {
        float waveInterval = circleDuration / circleWaveCount;

        for (int wave = 0; wave < circleWaveCount; wave++)
        {
            float angleStep = 360f / circleBulletCount;
            for (int i = 0; i < circleBulletCount; i++)
            {
                Vector2 dir = Quaternion.Euler(0, 0, angleStep * i) * Vector2.up;
                Projectile p = ShootProjectile(dir);
                p.GetComponent<Rigidbody2D>().linearVelocity = dir * circleSpeed;
            }
            yield return new WaitForSeconds(waveInterval);
        }
    }

    IEnumerator SpiralAttack()
    {
        float startTime = Time.time;
        float currentAngle = 0f;

        while (Time.time - startTime < spiralDuration)
        {
            currentAngle += spiralRotateSpeed * Time.deltaTime;
            for (int j = 0; j < spiralArmCount; j++)
            {
                float armAngle = currentAngle + (j * 360f / spiralArmCount);
                Vector2 dir = Quaternion.Euler(0, 0, armAngle) * Vector2.up;
                ShootProjectile(dir);
            }
            yield return new WaitForSeconds(0.15f); // 更密集的发射间隔
        }
    }

    IEnumerator CrossLaserAttack()
    {
        GameObject laserH = Instantiate(laserPrefab, transform.position, Quaternion.identity);
        GameObject laserV = Instantiate(laserPrefab, transform.position, Quaternion.Euler(0, 0, 90));

        float timer = 0f;
        while (timer < laserDuration)
        {
            timer += Time.deltaTime;
            // 激光持续期间可添加闪烁效果
            laserH.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.red, Color.white, Mathf.PingPong(timer * 5, 1));
            laserV.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.red, Color.white, Mathf.PingPong(timer * 5, 1));
            yield return null;
        }

        Destroy(laserH);
        Destroy(laserV);
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
