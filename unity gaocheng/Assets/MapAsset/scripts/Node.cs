using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Node : MonoBehaviour
{
    // 节点的唯一标识
    public int Id { get; private set; }
    public int Nid { get; private set; }

    public string nodeDescription { get; private set; }
    public string nodeName { get; private set; }
    public bool IsVisited { get; set; } = false;

    // 节点的邻居列表
    private List<Node> neighbors = new List<Node>();





    // 设置节点的唯一标识
    public void SetId(int id)
    {
        Id = id;
    }

    public void Setnodeid(int nid)
    {
        Nid = nid;
    }

    public void SetNodeDescription(string description)
    {
        nodeDescription = description;
    }

    public void SetNodeName(string name)
    {
        nodeName = name;
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
