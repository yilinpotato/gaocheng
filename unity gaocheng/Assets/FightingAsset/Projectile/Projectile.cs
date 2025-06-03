using UnityEngine;
using UnityEngine.Events;
using System.Collections;
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] protected float baseSpeed = 10f;
    [SerializeField] protected float lifetime = 3f;
    [SerializeField] protected LayerMask collisionMask;
    public UnityEvent<Vector2> OnHit = new UnityEvent<Vector2>();
    protected float damage;
    protected Transform owner;
    protected Vector2 direction;
    protected Rigidbody2D rb;
    // 受保护的生命周期控制方法
    protected float currentLifetime;
    protected Coroutine destroyCoroutine;

    public virtual void Initialize(Transform shooter, float dmg, Vector2 dir, float speedMultiplier = 1f)
    {
        owner = shooter;
        damage = dmg;
        direction = dir.normalized;
        rb = GetComponent<Rigidbody2D>();
        SetSpeed(baseSpeed * speedMultiplier);      
        destroyCoroutine = StartCoroutine(DestroyAfterLifetime());
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), owner.GetComponent<Collider2D>());
    }
    public void SetSpeed(float newSpeed)
    {
        if (rb != null)
        {
            // 保持当前方向，只修改速度大小
            rb.linearVelocity = direction * newSpeed;
        }
    }

    public void SetSpeedWithDirection(Vector2 newDirection, float newSpeed)
    {
        if (rb != null)
        {
            direction = newDirection.normalized;
            rb.linearVelocity = direction * newSpeed;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((collisionMask.value & (1 << other.gameObject.layer)) == 0) return;

        if (other.CompareTag("Player"))
        {
            Vector2 knockbackDirection = (other.transform.position - transform.position).normalized;

            if (other.TryGetComponent<Player>(out var player))
            {
                Debug.Log("对玩家造成击退");
                player.TakeDamageWithKnockback(damage, knockbackDirection);
            }

            OnHitEffect(other.ClosestPoint(transform.position)); // 播放命中特效
        }
        else if (other.TryGetComponent<Entity>(out var entity))
        {
            entity.TakeDamage(damage);
            OnHitEffect(other.ClosestPoint(transform.position));
        }

        Destroy(gameObject);
    }


    protected virtual void OnHitEffect(Vector2 hitPosition)
    {
        OnHit.Invoke(hitPosition);
    }
    public void ReturnToPool(GameObject bullet)
    {
        bullet.SetActive(false); 
    }
    protected virtual IEnumerator DestroyAfterLifetime()
    {
        yield return new WaitForSeconds(lifetime);
        DestroySelf();
    }
    protected virtual void DestroySelf()
    {
        Destroy(gameObject);
    }
}

