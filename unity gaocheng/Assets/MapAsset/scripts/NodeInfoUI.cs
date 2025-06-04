using UnityEngine;
using UnityEngine.UI;

public class NodeInfoUI : MonoBehaviour
{
    public Text nodeTypeText;
    public Text nodeNameText;
    public Text nodeIdText;
    public Text nodeNidText;
    public Text nodeDescriptionText;
    public Button confirmButton;
    public Button cancelButton;

    private Pointer pointer; // 引用指针对象

    void Start()
    {
        // 隐藏面板
        gameObject.SetActive(false);

        // 获取 Pointer 脚本
        pointer = FindObjectOfType<Pointer>();
    }

    public void ShowPanel(Node targetNode)
    {
        // 设置面板内容
        nodeTypeText.text = $"类型: {targetNode.nodeName}";
        nodeNameText.text = $"名称: {targetNode.nodeName}";
        nodeIdText.text = $"ID: {targetNode.Id}";
        nodeNidText.text = $"编号(Nid): {targetNode.Nid}";
        nodeDescriptionText.text = $"描述: {targetNode.nodeDescription}";

        // 检查是否与指针当前指向的节点相连
        Node currentNode = pointer.GetCurrentNode();
        bool isConnected = currentNode != null && currentNode.IsNeighbor(targetNode);

        // 显示或隐藏确认按钮
        confirmButton.gameObject.SetActive(isConnected);

        // 显示面板
        gameObject.SetActive(true);

        // 添加按钮事件
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() =>
        {
            Debug.Log("确认按钮被点击");
            Pointer pointer = FindObjectOfType<Pointer>();
            if (pointer != null)
            {
                pointer.MoveTo(targetNode);

            }
            targetNode.IsVisited = true; // 标记为已访问

        
            Debug.Log("确认按钮被点击");
            Debug.Log($"{targetNode.nodeName} 已经进入，节点nid是 {targetNode.Nid}");
            // 此处加入事件，战斗场景的启动接口——————————————————————————————————————
            CombatNode combatNode = targetNode as CombatNode;
            if (combatNode != null)
            {
                combatNode.StartCombat();
            }
            // ======= BOSS节点启动接口 ======= 
            BossNode bossNode = targetNode as BossNode;
            if (bossNode != null)
            {
                bossNode.StartBossBattle();
            }
            // 立即将节点变暗
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
