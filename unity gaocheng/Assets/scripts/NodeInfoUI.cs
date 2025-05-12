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
            pointer.MoveTo(targetNode); // 移动指针到目标节点
            gameObject.SetActive(false); // 隐藏面板
        });

        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(() =>
        {
            Debug.Log("取消按钮被点击");
            gameObject.SetActive(false); // 隐藏面板
        });
    }
}
