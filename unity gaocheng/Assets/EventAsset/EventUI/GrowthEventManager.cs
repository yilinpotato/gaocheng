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
        Debug.Log("=== GrowthEventManager Start ===");
        
        FindComponents();
        
        if (uiController != null)
        {
            Debug.Log("直接显示Growth事件UI");
            ShowGrowthEvent();
        }
        else
        {
            Debug.LogError("uiController 仍然为空！");
        }
    }

    private void FindComponents()
    {
        if (playerStats == null)
        {
            playerStats = FindObjectOfType<PlayerStats>();
            Debug.Log($"自动查找 PlayerStats: {(playerStats != null ? "成功" : "失败")}");
        }

        if (uiController == null)
        {
            uiController = FindObjectOfType<GrowthEventUIController>();
            Debug.Log($"自动查找 GrowthEventUIController: {(uiController != null ? "成功" : "失败")}");
            
            if (uiController == null)
            {
                uiController = GetComponent<GrowthEventUIController>();
                Debug.Log($"在当前GameObject查找 GrowthEventUIController: {(uiController != null ? "成功" : "失败")}");
            }
        }
    }

    public void ShowGrowthEvent()
    {
        Debug.Log("=== ShowGrowthEvent 被调用 ===");
        
        if (uiController != null)
        {
            Debug.Log("调用 uiController.ShowEventUI");
            uiController.ShowEventUI(OnOptionSelected);
        }
        else
        {
            Debug.LogError("uiController 为空！尝试直接激活UI");
            
            GameObject eventPanel = GameObject.Find("MainPanel");
            if (eventPanel != null)
            {
                eventPanel.SetActive(true);
                Debug.Log("直接激活 MainPanel");
            }
            else
            {
                Debug.LogError("也找不到 MainPanel！");
            }
        }
    }

    private void OnOptionSelected(int option)
    {
        Debug.Log($"=== OnOptionSelected: {option} ===");
        
        ResetBuffs();

        switch (option)
        {
            case 0: // +50% Max HP
                nextHealthMultiplier = 1.5f;
                Debug.Log("选择：生命值 +50%");
                break;
            case 1: // +30% Attack Damage
                nextAttackMultiplier = 1.3f;
                Debug.Log("选择：攻击力 +30%");
                break;
            case 2: // +30% Attack Speed
                nextAttackSpeedMultiplier = 1.3f;
                Debug.Log("选择：攻击速度 +30%");
                break;
        }

        // 先应用属性提升
        ApplyBuffsForNextRound();

        // 结束事件场景
        EndEventScene();
    }

    private void EndEventScene()
    {
        Debug.Log("=== 结束事件场景，卸载EventScene ===");
        
        try
        {
            // 重要：卸载EventScene而不是重新加载MapScene
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("EventScene");
            Debug.Log("EventScene 卸载完成");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"卸载EventScene失败: {e.Message}");
        }
    }

    public void ApplyBuffsForNextRound()
    {
        Debug.Log("=== ApplyBuffsForNextRound 开始 ===");
        
        if (playerStats == null) 
        {
            Debug.LogError("playerStats 为空，无法应用属性提升");
            return;
        }

        Debug.Log($"应用前 - MaxHP: {playerStats.MaxHP}, AttackPower: {playerStats.AttackPower}, AttackSpeed: {playerStats.AttackSpeed}");
        
        playerStats.MaxHP *= nextHealthMultiplier;
        playerStats.CurrentHP = playerStats.MaxHP;
        playerStats.AttackPower *= nextAttackMultiplier;
        playerStats.AttackSpeed *= nextAttackSpeedMultiplier;
        
        Debug.Log($"应用后 - MaxHP: {playerStats.MaxHP}, AttackPower: {playerStats.AttackPower}, AttackSpeed: {playerStats.AttackSpeed}");
    }

    public void ResetBuffs()
    {
        nextHealthMultiplier = 1f;
        nextAttackMultiplier = 1f;
        nextAttackSpeedMultiplier = 1f;
        Debug.Log("属性倍数已重置");
    }
}