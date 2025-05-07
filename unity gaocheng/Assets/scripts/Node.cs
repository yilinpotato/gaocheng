using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    // 节点的唯一标识
    public int Id { get; private set; }

    // 节点的邻居列表
    private List<Node> neighbors = new List<Node>();

    // 设置节点的唯一标识
    public void SetId(int id)
    {
        Id = id;
    }
    
    // 添加邻居节点
    public void AddNeighbor(Node neighbor)
    {
        if (!neighbors.Contains(neighbor))
        {
            neighbors.Add(neighbor);
        }
    }

    // 移除邻居节点
    public void RemoveNeighbor(Node neighbor)
    {
        if (neighbors.Contains(neighbor))
        {
            neighbors.Remove(neighbor);
        }
    }

    // 获取所有邻居节点
    public List<Node> GetNeighbors()
    {
        return new List<Node>(neighbors);
    }

    // 检查是否是邻居
    public bool IsNeighbor(Node node)
    {
        return neighbors.Contains(node);
    }
}

// 战斗节点
public class CombatNode : Node
{



    // 开始战斗
    public void StartCombat()
    {
        Debug.Log($"开始战斗");
        // 在这里实现战斗逻辑
    }
}

// 事件节点
public class EventNode : Node
{
    // 事件描述
    public string EventDescription { get; private set; }

    // 设置事件描述
    public void SetEventDescription(string description)
    {
        EventDescription = description;
    }

    // 触发事件
    public void TriggerEvent()
    {
        Debug.Log($"触发事件: {EventDescription}");
        // 在这里实现事件逻辑
    }
}

// Boss 战节点
public class BossNode : CombatNode
{
    // Boss 名称
    public string BossName { get; private set; }

    // Boss 血量
    public int BossHealth { get; private set; }

    // 设置 Boss 信息
    public void SetBossInfo(string name, int health)
    {
        BossName = name;
        BossHealth = health;
    }

    // 开始 Boss 战
    public void StartBossBattle()
    {
        Debug.Log($"开始 Boss 战: {BossName}，血量: {BossHealth}");
        // 在这里实现 Boss 战逻辑
    }
}

// 已完成节点
public class CompletedNode : Node
{
    // 是否已完成
    public bool IsCompleted { get; private set; }

    // 标记为已完成
    public void MarkAsCompleted()
    {
        IsCompleted = true;
        Debug.Log($"节点 {Id} 已完成");
    }
}
