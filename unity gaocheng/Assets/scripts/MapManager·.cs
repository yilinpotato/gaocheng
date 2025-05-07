using System.Collections.Generic;
using UnityEngine;



public class MapManager : MonoBehaviour
{
    public GameObject RoomPrefab;

    // 存储图的邻接表
    private Dictionary<Node, List<Node>> graph = new Dictionary<Node, List<Node>>();

    // 添加节点
    public void AddNode(Node node)
    {
        if (!graph.ContainsKey(node))
        {
            graph[node] = new List<Node>();
        }
    }

    // 添加边（无向）
    public void AddEdge(Node node1, Node node2)
    {
        if (!graph.ContainsKey(node1))
        {
            AddNode(node1);
        }
        if (!graph.ContainsKey(node2))
        {
            AddNode(node2);
        }

        if (!graph[node1].Contains(node2) && graph[node1].Count < 3 && graph[node2].Count < 3)
        {
            graph[node1].Add(node2);
            graph[node2].Add(node1);
            node1.AddNeighbor(node2); // 更新 Node 类的邻居
            node2.AddNeighbor(node1); // 更新 Node 类的邻居
        }
    }

    // 生成地图
    public void GenerateMap()
    {
        List<Node> allNodes = new List<Node>();

        // 1. 创建 5~7 个战斗节点
        int combatNodeCount = Random.Range(5, 8);
        for (int i = 0; i < combatNodeCount; i++)
        {
            CombatNode combatNode = CreateNode<CombatNode>($"CombatNode_{i}");
            allNodes.Add(combatNode);
        }

        // 2. 创建 2 个事件节点
        for (int i = 0; i < 2; i++)
        {
            EventNode eventNode = CreateNode<EventNode>($"EventNode_{i}");
            allNodes.Add(eventNode);
        }

        // 3. 创建 1 个 Boss 节点
        BossNode bossNode = CreateNode<BossNode>("BossNode");
        allNodes.Add(bossNode);

        // 4. 连接节点
        ConnectNodes(allNodes, bossNode);
    }

    // 创建节点并实例化对应的房间
    private T CreateNode<T>(string nodeName) where T : Node
    {
        Vector3 position = new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), 0); // 随机位置
        GameObject roomGO = Instantiate(RoomPrefab, position, Quaternion.identity);
        roomGO.name = nodeName;

        T node = roomGO.AddComponent<T>();
        AddNode(node);
        return node;
    }

    // 连接所有节点，确保满足条件
    private void ConnectNodes(List<Node> allNodes, BossNode bossNode)
    {
        // 1. 确保 Boss 节点只有一条边
        Node bossNeighbor = allNodes[Random.Range(0, allNodes.Count - 1)];
        AddEdge(bossNode, bossNeighbor);

        // 2. 随机连接其他节点，确保每个节点最多 3 条边
        List<Node> unconnectedNodes = new List<Node>(allNodes);
        unconnectedNodes.Remove(bossNode);

        while (unconnectedNodes.Count > 0)
        {
            Node node1 = unconnectedNodes[0];
            Node node2 = unconnectedNodes[Random.Range(1, unconnectedNodes.Count)];

            AddEdge(node1, node2);

            // 如果节点的边已达到 3 条，移出未连接列表
            if (graph[node1].Count >= 3)
            {
                unconnectedNodes.Remove(node1);
            }
            if (graph[node2].Count >= 3)
            {
                unconnectedNodes.Remove(node2);
            }
        }
    }
}
