using UnityEngine;
using System;
[Serializable]
public class PlayerData
{
    // ԭ������
    public string playerName;      // �������
    public int level;              // ��ҵȼ�
    public float health;           // ���Ѫ��

    // ��������
    public int saveId;             // �浵���
    public float playTime;         // ����ʱ�䣨����Ϊ��λ��
    public int money;              // ��ҽ�Ǯ
    public int score;              // ��ҷ���

    // ���캯��
    public PlayerData()
    {
        // Ĭ��ֵ��ʼ��
        playerName = "���";
        level = 1;
        health = 100f;
        saveId = 0;
        playTime = 0f;
        money = 0;
        score = 0;
    }

    // ��ѡ�������������Ĺ��캯��
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
