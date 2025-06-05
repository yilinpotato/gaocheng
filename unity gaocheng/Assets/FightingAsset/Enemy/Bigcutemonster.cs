using UnityEngine;
using System.Collections;

public class Boss : Enemy
{
    [Header("Bossר������")]
    [SerializeField] private float attackInterval = 2f;    // �������
    [SerializeField] private float rotationSpeed = 15f;  // ��Ļ��ת�ٶ�
    [SerializeField] private int bulletsPerWave = 36;     // ÿ���ӵ�����
    [SerializeField] private int wavesPerAttack = 3;      // ÿ�ι�������
    [Header("Boss��������")]
    public float phase2HPThreshold = 0.5f;    // ������׶ε�Ѫ����ֵ
    public float enrageInterval = 10f;        // ��״̬���
    public float bulletAcceleration = 1.2f;   // �ӵ����ٶ�
    private float currentAngle;                          // ��ǰ��Ļ��ʼ�Ƕ�
    private Coroutine attackCoroutine;                    // ����Э������
    [Header("�ܻ�����")]
    [SerializeField] private Color hurtColor = Color.red;
    [SerializeField] private float hurtDuration = 0.1f;
    [SerializeField] private Material hurtMaterial; // ������������
    private Material originalMaterial; // �洢ԭʼ����
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

    // ���Ĺ���ģʽЭ��
    private IEnumerator AttackPattern()
    {
        while (!isDead)
        {
            // ��һ�׶Σ��������ε�Ļ
            yield return StartCoroutine(CircularBarrage());

            // �ڶ��׶Σ���ת���浯Ļ
            yield return StartCoroutine(RotatingCrossBarrage());

            yield return new WaitForSeconds(attackInterval);
        }
    }

    // �������ε�Ļ
    private IEnumerator CircularBarrage()
    {
        for (int i = 0; i < wavesPerAttack; i++)
        {
            FireCircle(currentAngle, bulletsPerWave);
            currentAngle += rotationSpeed;
            yield return new WaitForSeconds(0.3f);
        }
    }

    // ��ת���浯Ļ
    private IEnumerator RotatingCrossBarrage()
    {
        int specialBullets = 8;
        float specialRotation = 0f;

        for (int i = 0; i < 4; i++)
        {
            // ���浯Ļ
            FireCircle(specialRotation, specialBullets);
            FireCircle(specialRotation + 22.5f, specialBullets); // ƫ�ƽǶ��γɽ���

            // ���ⵯĻ��ת
            specialRotation += 45f;
            yield return new WaitForSeconds(0.4f);
        }
    }

    // ͨ��Բ�ε�Ļ���ɷ���
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

    // ��̬�ٶȿ��ƣ������ٶȱ仯���ɣ�
    private float GetBulletSpeed(float angle)
    {
        // ÿ45������ʹ�ò�ͬ�ٶ�
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
    // ���ƹ�����Χʾ�⣨Scene��ͼ�ɼ���
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 5f);
    }
}