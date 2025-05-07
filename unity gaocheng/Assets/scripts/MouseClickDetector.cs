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

        // 根据节点类型设置 idleSprite
        Node node = GetComponent<Node>();
        if (node is CombatNode)
        {
            idleSprite = MapManager.Instance.CombatNodeSprite; // 假设 MapManager 是单例
        }
        else if (node is EventNode)
        {
            idleSprite = MapManager.Instance.EventNodeSprite;
        }
        else if (node is BossNode)
        {
            idleSprite = MapManager.Instance.BossNodeSprite;
        }

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