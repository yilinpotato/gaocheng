using UnityEngine;
using System.Collections;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class CircleSniper : Enemy
{
    [Header("���ι�������")]
    [SerializeField] private int fanBulletCount = 7;
    [SerializeField] private float fanSpreadAngle = 90f;
    [SerializeField] private float fanAttackInterval = 3f;

    [Header("Բ�ܹ�������")]
    [SerializeField] private int circleBulletCount = 12;
    [SerializeField] private float rotateSpeed = 90f;
    [SerializeField] private float expandSpeed = 1f;
    [SerializeField] private float circleAttackInterval = 3f;
    private float attackTimer;
    private bool isFanNext = true; // �����Σ���Բ�ܣ�����
    [Header("�ܻ�����")]
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

        // ÿ�� 3 �뽻�湥��
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

            isFanNext = !isFanNext; // ���湥��
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
        Vector2 center = transform.position; // ��¼��ǰ���ĵ�

        float angleStep = 360f / circleBulletCount;

        for (int i = 0; i < circleBulletCount; i++)
        {
            // �����ʼ�Ƕȣ��������λ�ã�
            Vector2 toPlayer = (Player.Instance.transform.position - transform.position).normalized;
            float baseAngle = Vector2.SignedAngle(Vector2.up, toPlayer);
            float startAngle = baseAngle + angleStep * i;

            // �����ӵ��������������Ϊ���ϻḲ���˶���
            Projectile p = ShootProjectile(Random.insideUnitCircle.normalized);

            // �����ת�������ʼ��
            bullets[i] = p.gameObject.AddComponent<CircularMotion>();
            bullets[i].Initialize(center, startAngle);
            bullets[i].rotateSpeed = this.rotateSpeed;
            bullets[i].expandSpeed = this.expandSpeed;

            // �Ƴ��Զ�����
            Destroy(p.GetComponent<AutoDestroy>());
        }

        // ���Ƴ���ʱ��
        yield return new WaitForSeconds(5f);

        // ���������ӵ�
        foreach (var bullet in bullets)
        {
            if (bullet != null) Destroy(bullet.gameObject);
        }
    }

    // �ܻ���������
    public override void TakeDamage(float damage)
    {
        // ����Ѿ�����������ִ�к����߼�
        if (isDead) return;

        // ԭ�е� TakeDamage �߼�
        base.TakeDamage(damage);

        // �����ʱ�Ѿ�������������� base.TakeDamage ɱ�������Ͳ�Ҫ������Э��
        if (isDead) return;

        // ����Э�̵Ĵ���...
        StartCoroutine(HurtEffect());
    }

    IEnumerator HurtEffect()
    {
        spriteRenderer.color = hurtColor;
        yield return new WaitForSeconds(hurtDuration);
        spriteRenderer.color = originalColor;
    }
}

// Բ���ӵ���Ϊ���
public class CircularMotion : MonoBehaviour
{
    // ��ת���ĵ㣨����Ϊ���˵�λ�ã�
    private Vector2 centerPoint;
    private float currentAngle;
    private float currentRadius;

    [Header("�˶�����")]
    public float rotateSpeed = 90f;
    public float expandSpeed = 1f;

    // ��ʼ���������ɵ��˵��ã�
    public void Initialize(Vector2 center, float startAngle)
    {
        centerPoint = center;
        currentRadius = 0f;
        currentAngle = startAngle;

        // ����ԭ�������˶�
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
    }

    void Update()
    {
        // �����˶�����
        currentRadius += expandSpeed * Time.deltaTime;
        currentAngle += rotateSpeed * Time.deltaTime;

        // ������λ��
        Vector2 newPos = new Vector2(
            Mathf.Cos(currentAngle * Mathf.Deg2Rad),
            Mathf.Sin(currentAngle * Mathf.Deg2Rad)
        ) * currentRadius;

        // Ӧ��λ�ã�������ĵ㣩
        transform.position = centerPoint + newPos;
    }
}


