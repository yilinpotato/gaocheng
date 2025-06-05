using UnityEngine;
using System;
[Serializable]
public class PlayerData
{
    // 原有属性
    public string playerName;      // 玩家姓名
    public int level;              // 玩家等级
    public float health;           // 玩家血量

    // 新增属性
    public int saveId;             // 存档编号
    public float playTime;         // 游玩时间（以秒为单位）
    public int money;              // 玩家金钱
    public int score;              // 玩家分数

    // 构造函数
    public PlayerData()
    {
        // 默认值初始化
        playerName = "玩家";
        level = 1;
        health = 100f;
        saveId = 0;
        playTime = 0f;
        money = 0;
        score = 0;
    }

    // 可选：创建带参数的构造函数
    public PlayerData(int saveId, string name, int level, float health, float playTime, int money, int score)
    {
        this.saveId = saveId;
        this.playerName = name;
        this.level = level;
        this.health = health;
        this.playTime = playTime;
        this.money = money;
        this.score = score;
    }
}
