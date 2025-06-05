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

    // 定义场景名称常量
    private const string CombatSceneName = "BattleScene";
    private const string BossSceneName = "BossScene";
    private const string EventSceneName = "EventScene";

    private Pointer pointer; // 指针对象引用




    void Start()
    {
        // 初始隐藏面板
        gameObject.SetActive(false);

        // 获取 Pointer 脚本
        pointer = FindFirstObjectByType<Pointer>();
    }

    public void ShowPanel(Node targetNode)
    {
        // 设置面板内容
        nodeTypeText.text = $"类型: {targetNode.nodeName}";
        nodeNameText.text = $"名称: {targetNode.nodeName}";
        nodeIdText.text = $"ID: {targetNode.Id}";
        nodeNidText.text = $"编号(Nid): {targetNode.Nid}";
        nodeDescriptionText.text = $"描述: {targetNode.nodeDescription}";

        // 检查是否与指针当前指向的节点相邻
        Node currentNode = pointer.GetCurrentNode();
        bool isConnected = currentNode != null && currentNode.IsNeighbor(targetNode);

        // 只显示可连接节点的确认按钮
        confirmButton.gameObject.SetActive(isConnected);

        // 显示面板
        gameObject.SetActive(true);

        // 添加按钮事件
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() =>
        {
            Debug.Log("确认按钮被点击");
            Pointer pointer = FindFirstObjectByType<Pointer>();
            if (pointer != null)
            {
                pointer.MoveTo(targetNode);
            }
            targetNode.IsVisited = true; // 标记为已访问

            Debug.Log($"{targetNode.nodeName} 已经进入，节点nid为 {targetNode.Nid}");

            // 判断节点类型，加载相应场景
            string sceneToLoad = "";

            // 战斗节点处理
            CombatNode combatNode = targetNode as CombatNode;
            if (combatNode != null)
            {
                Debug.Log($"正在加载场景: {sceneToLoad}");
                // 准备进入战斗，保存地图状态
                MapManager.Instance.PrepareForBattle();
                combatNode.StartCombat();
            }

            // BOSS节点处理
            BossNode bossNode = targetNode as BossNode;
            if (bossNode != null)
            {
                bossNode.StartBossBattle();
                sceneToLoad = BossSceneName;
            }

            // 事件节点处理
            EventNode eventNode = targetNode as EventNode;
            if (eventNode != null)
            {
                sceneToLoad = EventSceneName;
            }

            // 加载对应的场景
            if (!string.IsNullOrEmpty(sceneToLoad))
            {
                // 获取当前玩家数据
                PlayerData currentData = LoadManager.Instance.GetCurrentPlayerData();
                if (currentData != null)
                {
                    // 更新最新的数据
                    GameController gameController = FindFirstObjectByType<GameController>();
                    if (gameController != null)
                    {
                        gameController.UpdatePlayerDataBeforeSave();
                    }

                    // 保存临时状态
                    SaveManager.Instance.SaveGameState(currentData);
                }
                Debug.Log($"正在加载场景: {sceneToLoad}");
                SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);
            }

            // 设置节点变暗
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
            Debug.Log("取消按钮被点击");
            gameObject.SetActive(false); // 隐藏面板
        });
    }

}
