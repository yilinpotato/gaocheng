using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class BattleNode : Node
{
    [Header("ս���ڵ�����")]
    public bool isBattleActive = false;
    public Enemy enemy;

    protected abstract string BattleSceneName { get; }
    private GameObject playerInstance;
    private bool battleCompleted = false; // ��ӱ�ǣ������ظ�����ս��

    public virtual void StartBattle()
    {
        isBattleActive = true;
        battleCompleted = false; // ����ս����ɱ��

        // ������������
        playerInstance = FindObjectOfType<Player>()?.gameObject;

        // ������ʱ���أ�������״̬
        if (playerInstance != null)
        {
            playerInstance.SetActive(false);
        }

        // ����ս������
        SceneManager.LoadScene(BattleSceneName, LoadSceneMode.Additive);
        SceneManager.sceneLoaded += OnBattleSceneLoaded;
        SceneHider.SetSceneActive("MapScene", false);
        Debug.Log($"{BattleSceneName}ս����ʼ��");

        // �Ƴ��Զ�����ս���ĵ���
        // Invoke("EndBattle", 10f); 
    }

    private void OnBattleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == BattleSceneName)
        {
            SceneManager.sceneLoaded -= OnBattleSceneLoaded;
            SceneManager.SetActiveScene(scene);

            // �ض�λ��ң����裩
            BattleSceneController controller = FindObjectOfType<BattleSceneController>();
            Enemy[] enemies = FindObjectsOfType<Enemy>();

            // ��������ս���ڵ����
            foreach (Enemy enemy in enemies)
            {
                enemy.SetBattleNode(this);
            }

            // �����Ҫ�����Զ�Player���д���
            Player player = FindObjectOfType<Player>();
            if (player != null && controller != null)
            {
                player.transform.position = controller.playerSpawn.position;
            }
        }
    }

    public virtual void EndBattle()
    {
        // ��ֹ�ظ�����
        if (battleCompleted) return;
        battleCompleted = true;

        isBattleActive = false;

        // �����ӵ���ս����Դ
        if (BulletManager.Instance != null)
        {
            BulletManager.Instance.ResetAllBullets();
        }

        // ���Ϊ��ս������
        MapManager.isReturningFromBattle = true;

        // �ָ���ͼ�ϵ����
        if (playerInstance != null)
        {
            playerInstance.SetActive(true);
            playerInstance = null;
        }

        // ж��ս������
        SceneManager.UnloadSceneAsync(BattleSceneName);

        // ȷ����ͼ������Ϊ��Ծ����
        Scene mapScene = SceneManager.GetSceneByName("MapScene");
        if (mapScene.isLoaded)
        {
            SceneManager.SetActiveScene(mapScene);
        }
        else
        {
            Debug.LogWarning("��ͼ����δ����");
        }

        // �ָ���ͼ������ʾ
        SceneHider.SetSceneActive("MapScene", true);
        Debug.Log($"{BattleSceneName}ս��������");

        // ս��ʤ�������߼�
        BattleCompleted();
    }

    protected virtual void BattleCompleted()
    {
        Debug.Log("ս��ʤ����");

        // ���½ڵ�״̬Ϊ�ѷ���
        IsVisited = true;

        // �����������ӽ����߼�
    }
}
