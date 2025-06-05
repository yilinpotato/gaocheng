using UnityEngine;
using System.Collections;
public class BarrageWeaver : Enemy
{
    [Header("������Ļ����")]
    [SerializeField] private int spiralCount = 5;        // ��Ļ����
    [SerializeField] private float spiralSpeed = 180f;  // ��ת�ٶȣ���/�룩
    [SerializeField] private float spiralRadius = 1f;   // ��ʼ�뾶
    [SerializeField] private float spiralInterval = 0.2f; // ������

    [Header("�ܻ�����")]
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
            Debug.LogError("������ĻԤ����δ�ҵ���");
            return;
        }

        float currentAngle = Time.time * (-spiralSpeed); // �����ǹؼ�
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