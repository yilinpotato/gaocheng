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
        GameObject enemyObj = EnemyPoolManager.Instance.GetEnemyFromPool(enemySpawn.position, Quaternion.identity);
        if (enemyObj == null)
        {
            Debug.LogError("没有成功获取敌人！");
            return;
        }

        // 尝试获取战斗节点并设置关联
        // 如果玩家预制体存在，则实例化
        if (playerPrefab != null)
        {
            GameObject playerObj = Instantiate(playerPrefab, playerSpawn.position, Quaternion.identity);
            if (!playerObj.activeSelf)
            {
                playerObj.SetActive(true);
            }
        }
        Enemy enemy = enemyObj.GetComponent<Enemy>();
        if (enemy != null)
        {
            // 查找当前的战斗节点
            BattleNode battleNode = FindObjectOfType<BattleNode>();
            if (battleNode != null && battleNode.isBattleActive)
            {
                enemy.SetBattleNode(battleNode);
                Debug.Log("敌人与战斗节点关联成功");
            }
        }

        // 如果玩家预制体存在，则实例化
        if (playerPrefab != null)
        {
            Instantiate(playerPrefab, playerSpawn.position, Quaternion.identity);
        }

        Debug.Log("战斗开始！");
    }
}


