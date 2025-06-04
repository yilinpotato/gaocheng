using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        SceneManager.sceneLoaded += OnSceneLoaded;

        InitializePools();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mode == LoadSceneMode.Additive) return; // 忽略战斗场景加载

        // 当主场景加载时重置所有子弹
        ResetAllBullets();
    }

    void InitializePools()
    {
        foreach (var pool in bulletPools)
        {
            for (int i = 0; i < pool.poolSize; i++)
            {
                GameObject bullet = Instantiate(pool.prefab, transform);
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

        // 重置子弹状态
        BulletBase bulletBase = bullet.GetComponent<BulletBase>();
        if (bulletBase != null) bulletBase.ResetBullet();

        return bullet;
    }

    private void ExpandPool(BulletPool pool)
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject bullet = Instantiate(pool.prefab, transform);
            bullet.SetActive(false);
            pool.pool.Enqueue(bullet);
        }
    }

    public void ReturnToPool(GameObject bullet, BulletType type)
    {
        if (bullet == null) return;

        bullet.SetActive(false);
        bullet.transform.SetParent(transform);

        BulletPool targetPool = System.Array.Find(bulletPools, p => p.type == type);
        if (targetPool != null)
        {
            targetPool.pool.Enqueue(bullet);
        }
    }

    // 重置所有子弹
    public void ResetAllBullets()
    {
        // 停用所有活跃子弹并返回池中
        foreach (var pool in bulletPools)
        {
            foreach (var bullet in pool.pool)
            {
                if (bullet.activeSelf)
                {
                    bullet.SetActive(false);
                    ReturnToPool(bullet, pool.type);
                }
            }
        }
    }
}