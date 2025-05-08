using System.Collections.Generic;
using UnityEngine;



public class MapManager : MonoBehaviour
{
    public GameObject RoomPrefab;
    public static MapManager Instance { get; private set; }

    // 用于生成唯一的节点 ID
    private int currentId = 0;


    // 保存节点 ID 与类型的映射
    public Dictionary<int, string> nodeTypeMap = new Dictionary<int, string>();

    // 存储图的邻接表
    private Dictionary<Node, List<Node>> graph = new Dictionary<Node, List<Node>>();

    // 为不同节点类型指定图案
    public Sprite CombatNodeSprite;
    public Sprite EventNodeSprite;
    public Sprite BossNodeSprite;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

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

        if (!graph[node1].Contains(node2) && graph[node1].Count < 4 && graph[node2].Count < 4)
        {
            graph[node1].Add(node2);
            graph[node2].Add(node1);
            node1.AddNeighbor(node2); // 更新 Node 类的邻居
            node2.AddNeighbor(node1); // 更新 Node 类的邻居


            // 在屏幕上绘制边
            DrawEdge(node1.transform.position, node2.transform.position);
        }
    }

    // 生成地图
    public void GenerateMap()
    {
        List<Node> allNodes = new List<Node>();

        // 创建 10~15 个节点
        int totalNodeCount = Random.Range(10, 16);
        for (int i = 0; i < totalNodeCount; i++)
        {
            Node node = CreateNode<Node>($"Node_{i}");
            allNodes.Add(node);
        }

        // 形成最小生成树
        ConnectNodesWithMST(allNodes);

        // 根据连接数分配节点类型
        AssignNodeTypes(allNodes);
    }

    // 创建节点并实例化对应的房间
    private T CreateNode<T>(string nodeName) where T : Node
    {
        const float minDistance = 2.0f; // 节点之间的最小距离
        Vector3 position;

        // 尝试生成一个满足条件的位置
        int maxAttempts = 100; // 最大尝试次数，防止死循环
        int attempts = 0;
        do
        {
            position = new Vector3(Random.Range(-8, 8), Random.Range(-6, 6), 0);
            attempts++;
        } while (!IsPositionValid(position, minDistance) && attempts < maxAttempts);

        if (attempts >= maxAttempts)
        {
            Debug.LogWarning($"无法找到满足条件的位置，使用最后尝试的位置: {position}");
        }

        // 实例化房间
        GameObject roomGO = Instantiate(RoomPrefab, position, Quaternion.identity);
        roomGO.name = nodeName;

        // 添加节点组件
        T node = roomGO.AddComponent<T>();
        node.SetId(currentId++); // 分配唯一 ID
        AddNode(node);
        return node;
    }


    private bool IsPositionValid(Vector3 position, float minDistance)
    {
        foreach (var node in graph.Keys)
        {
            if (Vector3.Distance(node.transform.position, position) < minDistance)
            {
                return false; // 距离太近，位置无效
            }
        }
        return true; // 位置有效
    }
    private void DrawEdge(Vector3 start, Vector3 end)
    {
        // 创建一个空的 GameObject 用于存放 LineRenderer
        GameObject lineObject = new GameObject("Edge");
        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();

        // 设置 LineRenderer 的属性
        lineRenderer.startWidth = 0.1f; // 线条起点宽度
        lineRenderer.endWidth = 0.1f;   // 线条终点宽度
        lineRenderer.positionCount = 2; // 两个点
        lineRenderer.SetPosition(0, start); // 起点
        lineRenderer.SetPosition(1, end);   // 终点

        // 设置线条的材质和颜色
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
    }
    private float GetDistance(Node node1, Node node2)
    {
        return Vector3.Distance(node1.transform.position, node2.transform.position);
    }
    private void ConnectNodesWithMST(List<Node> allNodes)
    {
        HashSet<Node> connectedNodes = new HashSet<Node>();
        List<Node> unconnectedNodes = new List<Node>(allNodes);

        // 从第一个节点开始
        Node startNode = unconnectedNodes[0];
        connectedNodes.Add(startNode);
        unconnectedNodes.Remove(startNode);

        while (unconnectedNodes.Count > 0)
        {
            Node closestNode = null;
            Node fromNode = null;
            float minDistance = float.MaxValue;

            // 找到最近的未连接节点
            foreach (Node connectedNode in connectedNodes)
            {
                foreach (Node unconnectedNode in unconnectedNodes)
                {
                    float distance = GetDistance(connectedNode, unconnectedNode);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestNode = unconnectedNode;
                        fromNode = connectedNode;
                    }
                }
            }

            // 连接最近的节点
            if (closestNode != null && fromNode != null)
            {
                AddEdge(fromNode, closestNode);
                connectedNodes.Add(closestNode);
                unconnectedNodes.Remove(closestNode);
            }
        }
    }
    private void AssignNodeTypes(List<Node> allNodes)
    {
        Node initialNode = null;
        Node bossNode = null;
        int maxConnections = -1;

        // 找到连接数最多的节点，设置为初始节点
        foreach (Node node in allNodes)
        {
            int connectionCount = graph[node].Count;
            if (connectionCount > maxConnections)
            {
                maxConnections = connectionCount;
                initialNode = node;
            }
        }

        if (initialNode != null)
        {
            Debug.Log($"{initialNode.name} 被分配为初始节点");
            // 这里可以为初始节点添加特殊逻辑或标记
        }

        // 找到一个叶子节点作为 Boss 节点
        foreach (Node node in allNodes)
        {
            if (graph[node].Count == 1) // 叶子节点
            {
                bossNode = node;
                break;
            }
        }

        if (bossNode != null)
        {
            BossNode boss = bossNode.gameObject.AddComponent<BossNode>();
            SetNodeStyle(bossNode.gameObject, MapManager.Instance.BossNodeSprite); // 设置样式
            bossNode.gameObject.name = $"BossNode_{bossNode.name}"; // 更新名称
            nodeTypeMap[bossNode.Id] = "BossNode"; // 保存到映射表
            Destroy(bossNode); // 移除原始 Node 脚本
            Debug.Log($"{boss.name} 被分配为 Boss 节点");
            allNodes.Remove(bossNode); // 移除已分配的 Boss 节点
        }

        // 将非 Boss 的叶子节点分配为事件节点
        foreach (Node node in allNodes)
        {
            if (graph[node].Count == 1 && node.GetComponent<BossNode>() == null) // 叶子节点且不是 Boss 节点
            {
                EventNode eventNode = node.gameObject.AddComponent<EventNode>();
                SetNodeStyle(node.gameObject, MapManager.Instance.EventNodeSprite); // 设置样式
                node.gameObject.name = $"EventNode_{node.name}"; // 更新名称
                nodeTypeMap[node.Id] = "EventNode"; // 保存到映射表
                Destroy(node); // 移除原始 Node 脚本
                Debug.Log($"{eventNode.name} 被分配为事件节点");
            }
        }

        // 将剩余节点分配为战斗节点
        foreach (Node node in allNodes)
        {
            if (node != null && node.GetComponent<BossNode>() == null && node.GetComponent<EventNode>() == null)
            {
                CombatNode combatNode = node.gameObject.AddComponent<CombatNode>();
                SetNodeStyle(node.gameObject, MapManager.Instance.CombatNodeSprite); // 设置样式
                node.gameObject.name = $"CombatNode_{node.name}"; // 更新名称
                nodeTypeMap[node.Id] = "CombatNode"; // 保存到映射表
                Destroy(node); // 移除原始 Node 脚本
                Debug.Log($"{combatNode.name} 被分配为战斗节点");
            }
        }
    }
    private void SetNodeStyle(GameObject nodeObject, Sprite sprite)
    {
        SpriteRenderer spriteRenderer = nodeObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = nodeObject.AddComponent<SpriteRenderer>();
        }
        spriteRenderer.sprite = sprite;
    }
}
