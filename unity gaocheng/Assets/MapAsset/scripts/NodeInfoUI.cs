using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NodeInfoUI : MonoBehaviour
{
    public Text nodeTypeText;
    public Text nodeNameText;
    public Text nodeIdText;
    public Text nodeNidText;
    public Text nodeDescriptionText;
    public Button confirmButton;
    public Button cancelButton;

    // ���峡�����Ƴ���
    private const string CombatSceneName = "BattleScene";
    private const string BossSceneName = "BossScene";
    private const string EventSceneName = "EventScene";

    private Pointer pointer; // ָ���������




    void Start()
    {
        // ��ʼ�������
        gameObject.SetActive(false);

        // ��ȡ Pointer �ű�
        pointer = FindFirstObjectByType<Pointer>();
    }

    public void ShowPanel(Node targetNode)
    {
        // �����������
        nodeTypeText.text = $"����: {targetNode.nodeName}";
        nodeNameText.text = $"����: {targetNode.nodeName}";
        nodeIdText.text = $"ID: {targetNode.Id}";
        nodeNidText.text = $"���(Nid): {targetNode.Nid}";
        nodeDescriptionText.text = $"����: {targetNode.nodeDescription}";

        // ����Ƿ���ָ�뵱ǰָ��Ľڵ�����
        Node currentNode = pointer.GetCurrentNode();
        bool isConnected = currentNode != null && currentNode.IsNeighbor(targetNode);

        // ֻ��ʾ�����ӽڵ��ȷ�ϰ�ť
        confirmButton.gameObject.SetActive(isConnected);

        // ��ʾ���
        gameObject.SetActive(true);

        // ��Ӱ�ť�¼�
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() =>
        {
            Debug.Log("ȷ�ϰ�ť�����");
            Pointer pointer = FindFirstObjectByType<Pointer>();
            if (pointer != null)
            {
                pointer.MoveTo(targetNode);
            }
            targetNode.IsVisited = true; // ���Ϊ�ѷ���

            Debug.Log($"{targetNode.nodeName} �Ѿ����룬�ڵ�nidΪ {targetNode.Nid}");

            // �жϽڵ����ͣ�������Ӧ����
            string sceneToLoad = "";

            // ս���ڵ㴦��
            CombatNode combatNode = targetNode as CombatNode;
            if (combatNode != null)
            {
                Debug.Log($"���ڼ��س���: {sceneToLoad}");
                // ׼������ս���������ͼ״̬
                MapManager.Instance.PrepareForBattle();
                combatNode.StartCombat();
            }

            // BOSS�ڵ㴦��
            BossNode bossNode = targetNode as BossNode;
            if (bossNode != null)
            {
                bossNode.StartBossBattle();
                sceneToLoad = BossSceneName;
            }

            // �¼��ڵ㴦��
            EventNode eventNode = targetNode as EventNode;
            if (eventNode != null)
            {
                sceneToLoad = EventSceneName;
            }

            // ���ض�Ӧ�ĳ���
            if (!string.IsNullOrEmpty(sceneToLoad))
            {
                // ��ȡ��ǰ�������
                PlayerData currentData = LoadManager.Instance.GetCurrentPlayerData();
                if (currentData != null)
                {
                    // �������µ�����
                    GameController gameController = FindFirstObjectByType<GameController>();
                    if (gameController != null)
                    {
                        gameController.UpdatePlayerDataBeforeSave();
                    }

                    // ������ʱ״̬
                    SaveManager.Instance.SaveGameState(currentData);
                }
                Debug.Log($"���ڼ��س���: {sceneToLoad}");
                SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);
            }

            // ���ýڵ�䰵
            var sr = targetNode.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            }

            gameObject.SetActive(false);
        });

        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(() =>
        {
            Debug.Log("ȡ����ť�����");
            gameObject.SetActive(false); // �������
        });
    }

}
