using UnityEngine;
public class CombatNode : BattleNode
{

    // 指定战斗场景名称
    protected override string BattleSceneName => "BattleScene";
    public void StartCombat()
    {
        StartBattle();
    }
    public void EndCombat()
    {
        EndBattle();
    }
}
