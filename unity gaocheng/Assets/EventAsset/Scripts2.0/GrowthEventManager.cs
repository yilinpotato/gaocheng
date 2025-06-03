using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GrowthEventManager : MonoBehaviour
{
    public PlayerStats playerStats;

    // 用于下一局生效的buff
    private float nextHealthMultiplier = 1f;
    private float nextAttackMultiplier = 1f;
    private bool nextHasRevive = false;

    public GrowthEventUIController uiController;

    void Start()
    {
        ResetBuffs();
    }

    public void ShowGrowthEvent()
    {
        uiController.ShowEventUI(OnOptionSelected);
    }

    // 回调，玩家选择了某个选项
    private void OnOptionSelected(int option)
    {
        ResetBuffs();

        switch (option)
        {
            case 0: // 宙斯之盾
                nextHealthMultiplier = 1.5f;
                break;
            case 1: // 拉玛西亚之锤
                nextAttackMultiplier = 3f;
                break;
            case 2: // 伊蒂哈德幻影
                nextHasRevive = true;
                break;
        }

        Debug.Log("选择了选项：" + option);

        // 关闭UI
        uiController.HideEventUI();
    }

    // 在下一局开始时调用，应用buff
    public void ApplyBuffsForNextRound()
    {
        playerStats.currentHealth = playerStats.baseHealth * nextHealthMultiplier;
        playerStats.currentAttack = playerStats.baseAttack * nextAttackMultiplier;
        playerStats.hasRevive = nextHasRevive;
    }

    // 每局结束后重置buff（除非下一局还没开始）
    public void ResetBuffs()
    {
        nextHealthMultiplier = 1f;
        nextAttackMultiplier = 1f;
        nextHasRevive = false;
    }
}
