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

    // 回调：玩家选择选项
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
        uiController.HideEventUI();
    }

    // 应用下一局的buff
    public void ApplyBuffsForNextRound()
    {
        // 设置最大生命值并更新当前生命值
        playerStats.MaxHP *= nextHealthMultiplier;
        playerStats.CurrentHP = playerStats.MaxHP; // 重置为满血

        // 设置攻击力
        playerStats.AttackPower *= nextAttackMultiplier;

    }

    // 重置buff
    public void ResetBuffs()
    {
        nextHealthMultiplier = 1f;
        nextAttackMultiplier = 1f;
        nextHasRevive = false;
    }
}