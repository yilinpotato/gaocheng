using UnityEngine;

public class SplittingProjectile : Projectile
{
    [Header("分裂设置")]
    public GameObject subBulletPrefab;
    public int splitCount = 7; // 改为4发
    public float splitAngleRange = 360f;
    public float subBulletSpeed = 3f;

    // 移除计时器逻辑，改为在生命周期结束时分裂
    public override void Initialize(Transform shooter, float dmg, Vector2 dir, float speedMultiplier = 1f)
    {
        base.Initialize(shooter, dmg, dir, speedMultiplier);
        CancelInvoke(nameof(DestroySelf)); // 取消自动销毁
        Invoke(nameof(Split), lifetime - 0.1f); // 在寿命结束前分裂
    }

    void Split()
    {
        float angleStep = splitAngleRange / splitCount;
        for (int i = 0; i < splitCount; i++)
        {
            float angle = i * angleStep;
            Vector2 dir = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );

            GameObject bullet = Instantiate(subBulletPrefab, transform.position, Quaternion.identity);
            Projectile sub = bullet.GetComponent<Projectile>();
            sub.Initialize(transform, damage * 0.5f, dir.normalized, subBulletSpeed);
        }
        Destroy(gameObject);
    }
}