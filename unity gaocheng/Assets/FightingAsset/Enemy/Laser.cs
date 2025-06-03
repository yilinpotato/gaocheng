using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class LaserBeam : MonoBehaviour
{
    [Header("激光参数")]
    public float damage = 20f;
    public float duration = 1.0f;
    public float length = 10f;
    public LayerMask hitLayer;

    private SpriteRenderer sprite;
    private BoxCollider2D box;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        box = GetComponent<BoxCollider2D>();
    }

    private void OnEnable()
    {
        Debug.Log("Laser 激活！");
        DrawLaser();
        Invoke(nameof(Disable), duration);
    }

    private void DrawLaser()
    {

        sprite.size = new Vector2(sprite.size.x, length);
        box.size = new Vector2(0.2f, length);
        box.offset = new Vector2(0f, length / 2f); // 居中对齐
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & hitLayer) != 0)
        {
            // 如果是玩家
            if (other.TryGetComponent<Player>(out var player))
            {
                // 击退方向：激光向上的朝向
                Vector2 knockbackDir = transform.up.normalized;

                player.TakeDamageWithKnockback(damage, knockbackDir);
                Debug.Log("Laser 对玩家造成击退！");
            }
            else
            {
                // 其他实体
                Entity target = other.GetComponent<Entity>();
                if (target != null)
                {
                    target.TakeDamage(damage);
                }
            }
        }
    }


    private void Disable()
    {
        gameObject.SetActive(false);
    }
}
