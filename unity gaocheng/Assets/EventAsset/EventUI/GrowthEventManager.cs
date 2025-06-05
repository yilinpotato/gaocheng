using UnityEngine;

public class GrowthEventManager : MonoBehaviour
{
    public PlayerStats playerStats;
    public GrowthEventUIController uiController;

    private float nextHealthMultiplier = 1f;
    private float nextAttackMultiplier = 1f;
    private float nextAttackSpeedMultiplier = 1f;

    void Start()
    {
        if (uiController != null)
        {
            uiController.HideEventUI();
        }
    }

    public void ShowGrowthEvent()
    {
        if (uiController != null)
        {
            uiController.ShowEventUI(OnOptionSelected);
        }
    }

    private void OnOptionSelected(int option)
    {
        ResetBuffs();

        switch (option)
        {
            case 0: // +50% Max HP
                nextHealthMultiplier = 1.5f;
                break;
            case 1: // +30% Attack Damage
                nextAttackMultiplier = 1.3f;
                break;
            case 2: // +30% Attack Speed
                nextAttackSpeedMultiplier = 1.3f;
                break;
        }

        if (uiController != null)
        {
            uiController.HideEventUI();
        }

        ApplyBuffsForNextRound();

        // 结束事件场景
        EventNode eventNode = FindObjectOfType<EventNode>();
        if (eventNode != null)
        {
            eventNode.EndEvent();
        }
    }

    public void ApplyBuffsForNextRound()
    {
        if (playerStats == null) return;

        playerStats.MaxHP *= nextHealthMultiplier;
        playerStats.CurrentHP = playerStats.MaxHP;
        playerStats.AttackPower *= nextAttackMultiplier;
        playerStats.AttackSpeed *= nextAttackSpeedMultiplier;
    }

    public void ResetBuffs()
    {
        nextHealthMultiplier = 1f;
        nextAttackMultiplier = 1f;
        nextAttackSpeedMultiplier = 1f;
    }
}