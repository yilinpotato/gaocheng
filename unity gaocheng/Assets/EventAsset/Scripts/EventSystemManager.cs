using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class EventSystemManager : MonoBehaviour
{
    public EventUIController uiController;
    public EnemySpawner enemySpawner;
    public WeaponManager weaponManager;
    public PlayerStats playerStats;

    void Start()
    {
        uiController.ShowChoicePanel(HandlePlayerChoice);
    }

    void HandlePlayerChoice(string choice)
    {
        // 随机判断宝藏或敌人
        bool isTreasure = Random.value > 0.5f;

        if (isTreasure)
        {
            Debug.Log("触发宝藏事件");
            playerStats.RestoreHealth();
            weaponManager.GiveRandomWeapon();
        }
        else
        {
            Debug.Log("触发敌人事件");
            enemySpawner.SpawnEnemy();
        }

        uiController.HideChoicePanel();
    }
}
