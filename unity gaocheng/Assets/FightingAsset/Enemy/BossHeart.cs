using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // ����������ڳ�������

public class MomHeart : Enemy
{
    private enum AttackPattern
    {
        Circle,
        Spiral,
        DualRingStar,  // �滻��������͵�Ļ����
    }

    [Header("����ʱ������")]
    [SerializeField] private float circleDuration = 5f;       // ���ε�Ļ����ʱ��
    [SerializeField] private int circleWaveCount = 3;        // ���ε�Ļ����
    [SerializeField] private float spiralDuration = 8f;      // ������Ļ����ʱ��
    [SerializeField] private float starBurstDuration = 4f;  // �Ǳ���Ļ����ʱ��

    [Header("����˳������")]
    [SerializeField] private AttackPattern[] phase1Patterns = { AttackPattern.Circle, AttackPattern.Spiral };
    [SerializeField] private AttackPattern[] phase2Patterns = { AttackPattern.Spiral, AttackPattern.DualRingStar };

    [Header("�׶�����")]
    [SerializeField] private float phase2Threshold = 0.5f;
    [SerializeField] private bool isPhase2 = false;

    [Header("ͨ�õ�Ļ����")]
    [SerializeField] private Transform[] muzzlePoints;  // ���������
    [SerializeField] private float attackInterval = 2f; // ����ģʽ���
    [SerializeField] private float bulletSpeed = 5f;    // ͨ���ӵ��ٶ�

    [Header("���ε�Ļ")]
    [SerializeField] private int circleBulletCount = 12; // ÿ���ӵ�����

    [Header("������Ļ")]
    [SerializeField] private int spiralArmCount = 4;    // ����������
    [SerializeField] private float spiralRotateSpeed = 180f; // ������ת�ٶ�
    [SerializeField] private float spiralFireRate = 0.15f;  // �ӵ�������

    [Header("˫���Ǳ���Ļ")]
    [SerializeField] private int innerRingBullets = 12;    // �ڻ��ӵ���
    [SerializeField] private int outerRingBullets = 16;    // �⻷�ӵ���
    [SerializeField] private float ringRotationSpeed = 90f; // ����ת�ٶ�
    [SerializeField] private int starBursts = 4;           // �Ǳ�����
    [SerializeField] private float burstCooldown = 0.5f;    // �Ǳ�����ȴ
    [SerializeField] private float starBulletSpeed = 7f;    // �Ǳ��ӵ��ٶ�
    [SerializeField] private float ringDistance = 1.5f;    // ������Boss����

    private int currentPatternIndex;     // ��ǰ����ģʽ����
    private Coroutine currentAttack;     // ��ǰ����Э��

    [Header("�ܻ�����")]
    [SerializeField] private Color hurtColor = Color.red;
    [SerializeField] private float hurtDuration = 0.1f;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private float[] ringAngles = new float[2]; // ���⻷��ǰ�Ƕ�

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        // ��ʼ��˫���Ƕ�
        ringAngles[0] = 0f;  // �ڻ�
        ringAngles[1] = 45f; // �⻷

        // ȷ�����������������
        if (muzzlePoints == null || muzzlePoints.Length == 0)
        {
            muzzlePoints = new Transform[1];
            muzzlePoints[0] = transform;
            Debug.LogWarning("No muzzle points assigned, using Boss transform as default");
        }
    }

    protected override void UpdateAIState()
    {
        // ȷ��������������ʱֹͣAI��Ϊ
        if (isDead) return;

        // ����׶�ת��
        if (!isPhase2 && currentHP <= maxHP * phase2Threshold)
        {
            isPhase2 = true;
            Debug.Log("����ڶ��׶Σ�");
        }

        // ȷ��ֻ��һ������Э������
        if (currentAttack == null)
        {
            AttackPattern[] patterns = GetCurrentPatterns();
            currentAttack = StartCoroutine(ExecuteAttack(patterns));
        }
    }

    IEnumerator ExecuteAttack(AttackPattern[] patterns)
    {
        while (!isDead) // ȷ��������ֹͣ����
        {
            AttackPattern pattern = patterns[currentPatternIndex];
            Debug.Log($"��������ģʽ��{pattern}");

            // ִ�е�ǰ����ģʽ
            yield return StartCoroutine(GetAttackCoroutine(pattern));

            // ׼����һ�ֹ���
            currentPatternIndex = (currentPatternIndex + 1) % patterns.Length;

            // ��Ϣ���������
            yield return new WaitForSeconds(attackInterval);
        }
    }

    AttackPattern[] GetCurrentPatterns()
    {
        // �ڶ��׶β�����ǿ����ģʽ
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

            // ��ÿ������㷢�价�ε�Ļ
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
            // ÿn�봥��һ���Ǳ�
            for (int burst = 0; burst < starBursts; burst++)
            {
                // ����˫��
                FireRingProjectiles();

                // �Ǳ��ӵ��Ӹ����������
                foreach (var muzzle in muzzlePoints)
                {
                    FireStarBurstFromPoint(muzzle.position);
                }

                // ����˫���Ƕ�
                ringAngles[0] += ringRotationSpeed * burstCooldown;
                ringAngles[1] -= ringRotationSpeed * burstCooldown;

                yield return new WaitForSeconds(burstCooldown);

                // ��������������˳�
                if (isDead) yield break;
            }
        }
    }

    // ����˫����Ļ
    void FireRingProjectiles()
    {
        // �ڻ��ӵ�
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

        // �⻷�ӵ�
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

    // �Ǳ��ӵ�����ĳ������������䣩
    void FireStarBurstFromPoint(Vector2 origin)
    {
        int starBulletCount = 8;  // 8����
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

    // ��ǿ���ӵ����䷽�������ݻ��ಢ���λ�ò�����
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
        // ��ִ�л����������߼�����������Entity��Die()������ֱ�ӳ����л�
        isDead = true;
        StopAllCoroutines();

        // ���������¼�
        OnDeath?.Invoke();

        // ֪ͨ�¼�ϵͳ
        EventBus.Publish(new DeathEvent(this));

        // Boss�����߼�����ʾ������������ֱ�ӷ��ص�ͼ
        ShowVictoryPanel();

        // ������Ϸ����
        gameObject.SetActive(false);
    }

    private void ShowVictoryPanel()
    {
        // ֱ�Ӽ���"�������"����
        try
        {
            Debug.Log("Boss defeated! Loading victory scene...");
            UnityEngine.SceneManagement.SceneManager.LoadScene("�������");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load victory scene '�������': {e.Message}");
            // ������ؽ������ʧ�ܣ����˵�ֱ�ӷ��ص�ͼ
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

    // ���ӻ��ĵ�����Ϣ
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        // ����˫��
        Gizmos.color = Color.cyan;
        DrawRingGizmo(transform.position, ringDistance, ringAngles[0]);
        DrawRingGizmo(transform.position, ringDistance * 1.5f, ringAngles[1]);

        // ���Ʒ����
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