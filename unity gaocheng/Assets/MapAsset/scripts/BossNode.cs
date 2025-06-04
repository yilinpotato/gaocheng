using UnityEngine;

public class BossNode : BattleNode
{
    //指定战斗场景名称
    protected override string BattleSceneName => "BossScene";

    public void StartBossBattle()
    {
        StartBattle();
    }
    public void EndBossBattle()
    {
        EndBattle();
    }

    protected override void BattleCompleted()
    {
        base.BattleCompleted();
        Debug.Log("恭喜！你击败了关底BOSS！");
    }
}