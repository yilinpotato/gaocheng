using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualNode
{
    public int Id { get; set; }
    public Vector3 Position { get; set; }
    public List<int> Neighbors { get; set; } = new List<int>();
    public string NodeType { get; set; } // 节点类型
}

public class VirtualEdge
{
    public int Node1Id { get; set; }
    public int Node2Id { get; set; }
}
