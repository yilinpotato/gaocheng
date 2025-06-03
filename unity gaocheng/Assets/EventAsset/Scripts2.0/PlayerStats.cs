using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    // 基础属性
    public float baseHealth = 100f;
    public float baseAttack = 10f;

    // 当前状态
    [HideInInspector] public float currentHealth;
    [HideInInspector] public float currentAttack;
    [HideInInspector] public bool hasRevive = false;

    void Start()
    {
        ResetStats();
    }

    // 重置为初始属性
    public void ResetStats()
    {
        currentHealth = baseHealth;
        currentAttack = baseAttack;
        hasRevive = false;
    }

    // 恢复到最大生命值
    public void RestoreHealth()
    {
        currentHealth = baseHealth;
        Debug.Log("Health restored!");
    }
}

