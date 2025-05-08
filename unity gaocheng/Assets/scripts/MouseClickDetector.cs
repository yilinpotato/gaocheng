using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MouseClickDetector : MonoBehaviour
{
    public Sprite idleSprite;
    public Sprite hoverSprite;
    public Sprite clickSprite;

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        // 获取当前节点
        Node node = GetComponent<Node>();
        if (node == null)
        {
            Debug.LogWarning("未找到 Node 组件");
            return;
        }

        // 获取节点 ID
        int nodeId = node.Id;

        // 从映射表中获取节点类型
        if (MapManager.Instance.nodeTypeMap.TryGetValue(nodeId, out string nodeType))
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
                default:
                    Debug.LogWarning($"未知的节点类型: {nodeType}");
                    break;
            }
        }
        else
        {
            Debug.LogWarning($"节点 ID {nodeId} 未在映射表中找到");
        }

        // 设置初始显示的 Sprite
        sr.sprite = idleSprite;
    }

    void OnMouseEnter()
    {
        sr.sprite = hoverSprite;
        Debug.Log("鼠标进入");
    }

    void OnMouseExit()
    {
        sr.sprite = idleSprite;
        Debug.Log("鼠标离开");
    }

    void OnMouseDown()
    {
        sr.sprite = clickSprite;
        Debug.Log("鼠标点击");
    }
}
