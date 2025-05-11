using UnityEngine;
using UnityEngine.UI;

public class NodeInfoUI : MonoBehaviour
{
    public Text nodeTypeText;
    public Text nodeNameText;
    public Text nodeIdText;
    public Text nodeDescriptionText;
    public Button confirmButton;
    public Button cancelButton;
    public Text nodeNidText;

    public void ShowPanel(string nodeType, string nodeName, int nodeId, int nodeNid, string nodeDescription)
    {
        // 设置面板内容
        nodeTypeText.text = $"类型: {nodeType}";
        nodeNameText.text = $"名称: {nodeName}";
        nodeIdText.text = $"ID: {nodeId}";
        nodeNidText.text = $"编号(Nid): {nodeNid}";
        nodeDescriptionText.text = $"描述: {nodeDescription}";

        // 显示面板
        gameObject.SetActive(true);

        // 添加按钮事件
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() =>
        {
            Debug.Log("确认按钮被点击");
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
