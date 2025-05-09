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
