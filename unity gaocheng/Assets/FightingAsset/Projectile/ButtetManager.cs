using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    [System.Serializable]
    public class BulletPool
    {
        public BulletType type;
        public GameObject prefab;
        public int poolSize = 20;
        [HideInInspector] public Queue<GameObject> pool = new Queue<GameObject>();
    }

    [SerializeField] private BulletPool[] bulletPools;

    private static BulletManager _instance;
    public static BulletManager Instance => _instance;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        InitializePools();
    }

    void InitializePools()
    {
        foreach (var pool in bulletPools)
        {
            for (int i = 0; i < pool.poolSize; i++)
            {
                GameObject bullet = Instantiate(pool.prefab);
                bullet.SetActive(false);
                pool.pool.Enqueue(bullet);
            }
        }
    }

    public GameObject GetBullet(BulletType type, Vector2 position, Quaternion rotation)
    {
        BulletPool targetPool = System.Array.Find(bulletPools, p => p.type == type);
        if (targetPool == null) return null;

        if (targetPool.pool.Count == 0)
        {
            ExpandPool(targetPool);
        }

        GameObject bullet = targetPool.pool.Dequeue();
        bullet.transform.position = position;
        bullet.transform.rotation = rotation;
        bullet.SetActive(true);
        return bullet;
    }

    private void ExpandPool(BulletPool pool)
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject bullet = Instantiate(pool.prefab);
            bullet.SetActive(false);
            pool.pool.Enqueue(bullet);
        }
    }

    public void ReturnToPool(GameObject bullet, BulletType type)
    {
        bullet.SetActive(false);
        BulletPool targetPool = System.Array.Find(bulletPools, p => p.type == type);
        targetPool?.pool.Enqueue(bullet);
    }
}