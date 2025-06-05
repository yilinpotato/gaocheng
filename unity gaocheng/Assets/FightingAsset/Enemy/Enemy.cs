using UnityEngine;
using System.Collections;
public abstract class Enemy : Entity
{
    [Header("���˻�������")]
    [SerializeField] protected EnemyData data;
    [SerializeField] protected LayerMask playerLayer;
    protected Transform player;
    protected EnemyState currentState;
    protected BattleNode battleNode;

    protected Entity playerEntity;

    public virtual void SetBattleNode(BattleNode node)
    {
        battleNode = node;
    }

    protected override void Awake()
    {
        base.Awake();

        if (data != null)
        {
            maxHP = data.baseHP;
            moveSpeed = data.moveSpeed;
            attackPower = data.attackDamage;
            currentHP = maxHP;
        }
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerEntity = playerObject.GetComponent<Entity>();
            if (playerEntity == null)
            {
                Debug.LogWarning("�ҵ���Ҷ��󣬵�û��Entity���");
            }
        }
        else
        {
            Debug.LogWarning("�޷��ҵ���Ҷ���");
        }
        // Ϊ�����¼���Ӵ�����
        OnDeath.AddListener(HandleEnemyDeath);
    }

    private void SavePlayerCurrentHP()
    {
        if (playerEntity != null)
        {
            float playerCurrentHP = playerEntity.CurrentHP;
            Debug.Log($"������ҵ�ǰѪ��: {playerCurrentHP}");

            try
            {
                // ����ͨ�� LoadManager �� SaveManager ֱ�ӱ��棬������ GameController
                if (LoadManager.Instance != null && SaveManager.Instance != null)
                {
                    PlayerData currentPlayerData = LoadManager.Instance.GetCurrentPlayerData();
                    if (currentPlayerData != null)
                    {
                        currentPlayerData.health = playerCurrentHP;
                        SaveManager.Instance.QuickSave(currentPlayerData);
                        Debug.Log("��������ѱ��棬Ѫ������Ϊ: " + playerCurrentHP);
                    }
                }
                else
                {
                    Debug.LogWarning("LoadManager �� SaveManager ʵ��������");
                }
            }
            catch (System.Exception e)
            {
                // �����κο��ܵĴ���ȷ����Ӱ�쳡���л�
                Debug.LogError($"�������Ѫ��ʱ����: {e.Message}");
            }
        }
    }

    // ��ӵ�����������
    private void HandleEnemyDeath()
    {
        if (battleNode != null)
        {
            Debug.Log("����������׼������ս��...");

            // ֱ��ʹ�� forTestButton �ķ�ʽ���ص�ͼ
            StartCoroutine(ReturnToMapAfterDelay(2.0f));

            // �����Ȼ��Ҫͨ�� battleNode ����ս�������Ա�������Ĵ���
            // StartCoroutine(EndBattleAfterDelay(2.0f));
        }
    }

    private IEnumerator ReturnToMapAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // ���� forTestButton �ķ�����ֱ�Ӽ��ص�ͼ����
        string mapSceneName = "MapScene";
        Debug.Log("[����] ��ʼִ�з��ص�ͼ����...");

        try
        {
            // ֱ�ӳ��Լ��س��� - ����ʹ�� SceneUtility
            Debug.Log($"[����] ֱ�ӳ��Լ��س���: {mapSceneName}");
            UnityEngine.SceneManagement.SceneManager.LoadScene(mapSceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[����] ���ص�ͼ����ʱ����: {e.Message}\n{e.StackTrace}");
        }
    }

    private IEnumerator EndBattleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (battleNode != null)
        {
            battleNode.EndBattle();
        }
    }

    protected override void Start()
    {
        base.Start();
        if (data != null) LoadFromData(data);
    }

    protected override void Update()
    {
        if (isDead) return;
        UpdateAIState();
    }

    // AI״̬�����߼�
    protected abstract void UpdateAIState();

    protected override void Die()
    {
        base.Die();
        SavePlayerCurrentHP();
        StopAllCoroutines();
    }

    // ���䵯Ļͨ�÷���
    protected Projectile ShootProjectile(Vector2 direction)
    {
        GameObject bulletObj = BulletManager.Instance.GetBullet(
          BulletType.Enemy,
          transform.position,
          Quaternion.identity
        );
        if (bulletObj.TryGetComponent<Projectile>(out var projectile))
        {
            projectile.Initialize(transform, data.attackDamage, direction);
        }
        return projectile;
    }
}
