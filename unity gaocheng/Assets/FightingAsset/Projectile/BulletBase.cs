using UnityEngine;

public class BulletBase : MonoBehaviour
{
    public BulletType type;
    private Vector2 direction;
    private float speed;
    private float lifetime;

    // 用于重置的状态
    private Vector2 originalDirection;
    private float originalSpeed;
    private float originalLifetime;
    private Vector3 originalScale;
    private Color originalColor;

    void Start()
    {
        // 保存初始状态
        originalDirection = direction;
        originalSpeed = speed;
        originalLifetime = lifetime;
        originalScale = transform.localScale;

        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer != null) originalColor = renderer.color;
    }

    // 外部调用设置子弹参数
    public void Initialize(Vector2 dir, float spd, float life)
    {
        direction = dir;
        speed = spd;
        lifetime = life;
    }

    // 重置子弹到初始状态
    public void ResetBullet()
    {
        direction = originalDirection;
        speed = originalSpeed;
        lifetime = originalLifetime;
        transform.localScale = originalScale;

        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer != null) renderer.color = originalColor;

        // 重置定时器
        CancelInvoke(nameof(Deactivate));
    }

    void Update()
    {
        transform.position += (Vector3)direction * speed * Time.deltaTime;
        lifetime -= Time.deltaTime;

        if (lifetime <= 0)
        {
            Deactivate();
        }
    }

    private void Deactivate()
    {
        BulletManager.Instance.ReturnToPool(gameObject, type);
    }
}