using UnityEngine;
public enum AttackMode
{
    Normal,
    CircularSpread
}

public class PlayerShooting : MonoBehaviour
{
    [Header("射击设置")]
    [SerializeField] private Transform firePoint;       // 射击点
    [SerializeField] private BulletType currentBulletType = BulletType.Standard;  // 当前子弹类型
    private PlayerStats stats;          // 引用PlayerStats

    void Start()
    {
        // 获取PlayerStats组件
        stats = GetComponent<PlayerStats>();
    }


    public float GetAttackCooldown()
    {
        return 1f / stats.AttackSpeed;  // 根据AttackSpeed计算冷却时间
    }
    public void Shoot()
    {
        // 生成子弹
        GameObject bullet = BulletManager.Instance.GetBullet(currentBulletType, firePoint.position, firePoint.rotation);

        // 设置子弹属性
        if (bullet.TryGetComponent<Projectile>(out var projectile))
        {
            // 添加射击偏差
            Vector2 deviation = Random.insideUnitCircle * stats.ShotSpread;

            // 初始化子弹
            projectile.Initialize(
                transform,             // 发射者
                stats.AttackPower,         // 伤害
                (firePoint.up + (Vector3)deviation).normalized,  // 射击方向
                stats.ShotSpeed       // 射击速度
            );
        }
    }
}
