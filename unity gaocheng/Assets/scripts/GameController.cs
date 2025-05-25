using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public MapManager mapManager;

    public static GameController Instance { get; private set; }

    // 随机种子
    public int RandomSeed { get; private set; }

    // Start is called before the first frame update  
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // 生成随机种子
        RandomSeed = GenerateRandomSeed();
        Debug.Log("随机种子: " + RandomSeed);

        // 生成地图
        mapManager.GenerateMap();


    }

    // Update is called once per frame  
    void Update()
    {

    }

    // 生成随机种子的方法
    private int GenerateRandomSeed()
    {
        return System.DateTime.Now.Millisecond; // 使用当前时间生成种子
    }
}
