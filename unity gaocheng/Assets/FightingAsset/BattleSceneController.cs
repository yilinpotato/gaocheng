using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleSceneController : MonoBehaviour
{
    public Transform enemySpawn;
    public Transform playerSpawn;
    public GameObject playerPrefab;
    void Start()
    {
        // 生成敌人
        GameObject enemy = EnemyPoolManager.Instance.GetEnemyFromPool(enemySpawn.position, Quaternion.identity);
        if (enemy == null)
        {
            Debug.LogError("没有成功获取敌人！");
        }
        
        if (playerPrefab != null)
        {
            Instantiate(playerPrefab, playerSpawn.position, Quaternion.identity);
        }

        Debug.Log("战斗开始！");
    }

}


