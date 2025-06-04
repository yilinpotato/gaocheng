using UnityEngine;
using System.Collections.Generic;

public class EnemyPoolManager : MonoBehaviour
{
    public static EnemyPoolManager Instance;

    [Header("敌人预制体池")]
    public GameObject[] enemyPrefabs; // 所有可能的敌人预制体

    [Header("池设置")]
    [SerializeField] private int initialPoolSize = 5;

    private Dictionary<GameObject, Queue<GameObject>> poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();

    void Awake()
    {
        Instance = this;
        InitializePool();
    }

    // 初始化对象池
    void InitializePool()
    {
        foreach (var prefab in enemyPrefabs)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < initialPoolSize; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(prefab, objectPool);
        }
    }

    // 从池中获取敌人
    public GameObject GetEnemyFromPool(Vector3 position, Quaternion rotation)
    {
        // 随机选择预制体类型
        int randomIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject prefab = enemyPrefabs[randomIndex];

        if (poolDictionary[prefab].Count == 0)
        {
            // 如果池空则新建
            GameObject newObj = Instantiate(prefab);
            newObj.SetActive(false);
            poolDictionary[prefab].Enqueue(newObj);
        }

        GameObject enemy = poolDictionary[prefab].Dequeue();
        enemy.transform.position = position;
        enemy.transform.rotation = rotation;
        enemy.SetActive(true);

        return enemy;
    }

    // 回收敌人到池
    public void ReturnToPool(GameObject enemy)
    {
        enemy.SetActive(false);
        foreach (var pair in poolDictionary)
        {
            if (pair.Key.name == enemy.name.Replace("(Clone)", ""))
            {
                pair.Value.Enqueue(enemy);
                return;
            }
        }
    }
}