using UnityEngine;
[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("基础属性")]
    public float baseHP = 100f;
    public float moveSpeed = 2f;
    public float attackDamage = 10f;

    [Header("AI行为参数")]
    public float patrolSpeed = 1f;     // 巡逻速度或角速度
    public float attackInterval = 1f;  // 攻击间隔
}
