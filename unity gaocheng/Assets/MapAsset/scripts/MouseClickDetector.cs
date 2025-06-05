using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class MouseClickDetector : MonoBehaviour
{
    public Sprite idleSprite;
    public Sprite hoverSprite;
    public Sprite clickSprite;

    private SpriteRenderer sr;

    // 引用节点信息面板
    // public GameObject nodeInfoPanel; // 面板预制体
    private NodeInfoUI nodeInfoUI;   // 面板的脚本组件

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        // 获取当前节点
        Node node = GetComponent<Node>();
        if (node == null)
        {
            Debug.LogWarning($"未找到 Node 组件，GameObject 名称: {gameObject.name}");
            return;
        }

        // 获取节点 ID
        int nodeId = node.Id;

        // 从映射表中获取节点类型
        if (MapManager.Instance != null && MapManager.Instance.nodeTypeMap.TryGetValue(nodeId, out string nodeType))
        {
            // 根据节点类型设置 idleSprite
            switch (nodeType)
            {
                case "BossNode":
                    idleSprite = MapManager.Instance.BossNodeSprite;
                    break;
                case "CombatNode":
                    idleSprite = MapManager.Instance.CombatNodeSprite;
                    break;
                case "EventNode":
                    idleSprite = MapManager.Instance.EventNodeSprite;
                    break;
                case "InitialNode":
                    idleSprite = MapManager.Instance.InitialNodeSprite;
                    break;
                default:
                    Debug.LogWarning($"未知的节点类型: {nodeType}");
                    break;
            }
        }
        else
        {
            if (MapManager.Instance == null)
            {
                Debug.LogError("MouseClickDetector: MapManager.Instance is null!");
            }
            else
            {
                Debug.LogWarning($"节点 ID {nodeId} 未在映射表中找到");
            }
        }

        // 设置初始显示的 Sprite
        if (sr != null && idleSprite != null)
        {
            sr.sprite = idleSprite;
        }
        else
        {
            if(sr == null) Debug.LogWarning($"MouseClickDetector on {gameObject.name}: SpriteRenderer is null.");
            if(idleSprite == null) Debug.LogWarning($"MouseClickDetector on {gameObject.name}: idleSprite is null, nodeType might be missing or incorrect.");
        }

        // 获取面板的脚本组件 - 改为动态查找
        nodeInfoUI = NodeInfoUI.Instance; // 通过单例获取引用
        if (nodeInfoUI == null)
        {
            Debug.LogError($"MouseClickDetector on {gameObject.name}: 未能获取到 NodeInfoUI.Instance！");
        }
        // 不再需要 public GameObject nodeInfoPanel; 字段，可以移除或注释掉
    }

    void OnMouseEnter()
    {
        if (IsPointerOverUI())
        {
            Debug.Log("鼠标点击被 UI 遮挡");
            return; // 如果被 UI 遮挡，直接返回
        }
        sr.sprite = hoverSprite;
        Node node = GetComponent<Node>();
        if (node != null && node.IsVisited)
        {
            sr.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        }
        Debug.Log("鼠标进入");

    }

    void OnMouseExit()
    {
        sr.sprite = idleSprite;
        Node node = GetComponent<Node>();
        if (node != null && node.IsVisited)
        {
            sr.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        }
        Debug.Log("鼠标离开");
    }

    void OnMouseDown()
    {
        if (IsPointerOverUI())
        {
            Debug.Log("鼠标点击被 UI 遮挡");
            return; // 如果被 UI 遮挡，直接返回
        }
        sr.sprite = hoverSprite;

        Debug.Log("鼠标进入");

        // 获取当前节点
        Node node = GetComponent<Node>();
        if (node != null && node.IsVisited)
        {
            sr.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        }
        if (node == null)
        {
            Debug.LogWarning("未找到 Node 组件");
            return;
        }

        // 获取节点 ID
        int nodeId = node.Id;

        // 设置点击后的 Sprite
        sr.sprite = clickSprite;

        // 已访问则直接移动指针
        if (node.IsVisited)
        {
            Pointer pointer = Object.FindFirstObjectByType<Pointer>(); // 替换过时方法
            if (pointer != null)
            {
                pointer.MoveTo(node);
            }
            return;
        }

        // 展示节点信息面板
        if (nodeInfoUI != null)
        {
            nodeInfoUI.ShowPanel(node); // 直接传递 Node 对象
        }
    }

    // 判断鼠标是否点击在 UI 上
    private bool IsPointerOverUI()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        return results.Count > 0; // 如果有 UI 被检测到，返回 true
    }
}
