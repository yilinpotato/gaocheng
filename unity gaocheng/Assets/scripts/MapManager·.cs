using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor.Experimental.GraphView;
using UnityEngine;



public class MapManager : MonoBehaviour
{
    public GameObject RoomPrefeb;

    public static MapManager Instance { get; private set; }

    // 用于生成唯一的节点 ID
    private int currentId = 0;


    // 保存节点 ID 与类型的映射
    public Dictionary<int, string> nodeTypeMap = new Dictionary<int, string>();

    // 存储图的邻接表
    private Dictionary<Node, List<Node>> graph = new Dictionary<Node, List<Node>>();

    // 为不同节点类型指定图案
    public Sprite  InitialNodeSprite;
    public Sprite CombatNodeSprite;
    public Sprite EventNodeSprite;
    public Sprite BossNodeSprite;
    private List<VirtualNode> virtualNodes = new List<VirtualNode>();
    private List<VirtualEdge> virtualEdges = new List<VirtualEdge>();
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
        // 创建虚拟节点
        int totalNodeCount = Random.Range(10, 16);
        for (int i = 0; i < totalNodeCount; i++)
        {
            VirtualNode virtualNode = new VirtualNode
            {
                Id = currentId++,
                Position = GenerateValidPosition()
            };
            virtualNodes.Add(virtualNode);
        }

        // 创建虚拟边并形成最小生成树
        ConnectVirtualNodesWithMST();

        // 根据连接数分配节点类型
        AssignVirtualNodeTypes();

        // 根据虚拟信息实例化节点和边
        InstantiateNodesAndEdges();
    }


    private bool IsPositionValid(Vector3 position, float minDistance)
    {
        // 检查虚拟节点的位置
        foreach (var virtualNode in virtualNodes)
        {
            if (Vector3.Distance(virtualNode.Position, position) < minDistance)
            {
                return false; // 距离太近，位置无效
            }
        }


        return true; // 位置有效
    }
    public Material ropeMaterial;
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
        lineRenderer.material = ropeMaterial;
        lineRenderer.textureMode = LineTextureMode.Tile;
        float lineLength = Vector3.Distance(start, end);
        lineRenderer.material.mainTextureScale = new Vector2(lineLength / 2, 1);

        // 设置 LineRenderer 的 Sorting Layer 和 Order in Layer
        lineRenderer.sortingLayerName = "Default"; // 确保与节点的 Sorting Layer 一致
        lineRenderer.sortingOrder = -1; // 设置为比节点的 SpriteRenderer 更低的层级
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
    private Vector3 GenerateValidPosition()
    {
        const float minDistance = 3.0f;
        Vector3 position;
        int maxAttempts = 100;
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
        else
        {
            Debug.Log($"生成节点位置: {position}");
        }

        return position;
    }
    private void ConnectVirtualNodesWithMST()
    {
        HashSet<VirtualNode> connectedNodes = new HashSet<VirtualNode>();
        List<VirtualNode> unconnectedNodes = new List<VirtualNode>(virtualNodes);

        VirtualNode startNode = unconnectedNodes[0];
        connectedNodes.Add(startNode);
        unconnectedNodes.Remove(startNode);

        while (unconnectedNodes.Count > 0)
        {
            VirtualNode closestNode = null;
            VirtualNode fromNode = null;
            float minDistance = float.MaxValue;

            foreach (VirtualNode connectedNode in connectedNodes)
            {
                foreach (VirtualNode unconnectedNode in unconnectedNodes)
                {
                    float distance = Vector3.Distance(connectedNode.Position, unconnectedNode.Position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestNode = unconnectedNode;
                        fromNode = connectedNode;
                    }
                }
            }

            if (closestNode != null && fromNode != null)
            {
                virtualEdges.Add(new VirtualEdge { Node1Id = fromNode.Id, Node2Id = closestNode.Id });
                fromNode.Neighbors.Add(closestNode.Id);
                closestNode.Neighbors.Add(fromNode.Id);
                connectedNodes.Add(closestNode);
                unconnectedNodes.Remove(closestNode);
            }
        }
    }
    private void AssignVirtualNodeTypes()
    {
        VirtualNode initialNode = null;
        VirtualNode bossNode = null;
        int maxConnections = -1;

        // 找到连接数最多的节点，设置为初始节点
        foreach (VirtualNode node in virtualNodes)
        {
            if (node.Neighbors.Count > maxConnections)
            {
                maxConnections = node.Neighbors.Count;
                initialNode = node;
            }
        }

        if (initialNode != null)
        {
            initialNode.NodeType = "InitialNode";
        }

        // 找到一个叶子节点作为 Boss 节点
        foreach (VirtualNode node in virtualNodes)
        {
            if (node.Neighbors.Count == 1 && node.NodeType == null)
            {
                bossNode = node;
                break;
            }
        }

        if (bossNode != null)
        {
            bossNode.NodeType = "BossNode";
        }

        // 将剩余节点分配为事件节点或战斗节点
        foreach (VirtualNode node in virtualNodes)
        {
            if (node.NodeType == null)
            {
                node.NodeType = node.Neighbors.Count == 1 ? "EventNode" : "CombatNode";
            }
        }
    }
    private void InstantiateNodesAndEdges()
    {
        Dictionary<int, Node> instantiatedNodes = new Dictionary<int, Node>();

        foreach (VirtualNode virtualNode in virtualNodes)
        {
            // 统一使用 RoomPrefeb 进行实例化
            GameObject roomGO = Instantiate(RoomPrefeb, virtualNode.Position, Quaternion.identity);
            roomGO.name = $"Node_{virtualNode.Id}";

            // 根据节点类型添加对应的组件
            Node node;
            switch (virtualNode.NodeType)
            {
                case "InitialNode":
                    node = roomGO.AddComponent<InitialNode>();
                    Pointer pointer = FindObjectOfType<Pointer>();
                    
                    if (pointer != null)
                    {
                        pointer.SetcurrentNode(node);
                    }
                    else
                    {
                        Debug.LogError("Pointer instance not found in the scene.");
                    }
                    pointer.Initialize(node); // 初始化指针位置
                    SetNodeStyle(roomGO, InitialNodeSprite); // 可为初始节点设置独特样式
                    break;
                case "BossNode":
                    node = roomGO.AddComponent<BossNode>();
                    SetNodeStyle(roomGO, BossNodeSprite);
                    break;
                case "EventNode":
                    node = roomGO.AddComponent<EventNode>();
                    SetNodeStyle(roomGO, EventNodeSprite);
                    break;
                case "CombatNode":
                default:
                    node = roomGO.AddComponent<CombatNode>();
                    SetNodeStyle(roomGO, CombatNodeSprite);
                    break;
            }
            MapManager.Instance.nodeTypeMap.TryGetValue(node.Id, out string nodeType);
            // 设置节点 ID ，Nid并添加到图中
            node.SetId(virtualNode.Id);
            node.Setnodeid(GenerateNodeNumber(node.Id, virtualNode.NodeType));
            node.SetNodeDescription(GetDescriptionByNid(node.Nid));
            node.SetNodeName(GetNameByNid(node.Nid));
            instantiatedNodes[virtualNode.Id] = node;
            AddNode(node);

            // 将节点 ID 和类型添加到映射表
            if (!nodeTypeMap.ContainsKey(virtualNode.Id))
            {
                nodeTypeMap.Add(virtualNode.Id, virtualNode.NodeType);
            }
        }

        // 创建边
        foreach (VirtualEdge virtualEdge in virtualEdges)
        {
            Node node1 = instantiatedNodes[virtualEdge.Node1Id];
            Node node2 = instantiatedNodes[virtualEdge.Node2Id];
            AddEdge(node1, node2);
        }
    }

    public int GenerateNodeNumber(int Id, string nodeType)
    {
        // 使用 GameController 的随机种子和节点 ID 初始化随机数生成器
        System.Random random = new System.Random(GameController.Instance.RandomSeed + Id);

        // 根据节点类型生成不同范围的随机编号
        switch (nodeType)
        {
            case "BossNode":
                return random.Next(1, 1); 
            case "CombatNode":
                return random.Next(2, 2);
            case "EventNode":
                return random.Next(3, 3); 
            case "InitialNode":
                return random.Next(4, 4); 
            default:
                return random.Next(300, 400); // 默认节点范围 300 到 399
        }
    }
    public string GetDescriptionByNid(int Nid)
    {
        // 使用 Nid 作为键来分配描述文本
        switch (Nid)
        {
            case 4:
                return "这是一个初始节点，代表冒险的起点。";
            case 2:
                return "这是一个战斗节点，准备迎接挑战！";
            case 3:
                return "这是一个事件节点，可能会发生一些有趣的事情。";
            case 1:
                return "这是一个Boss节点，最终的考验在这里等待着你。";
            default:
                return "这是一个普通节点，没有特别的描述。";
        }
    }

    public string GetNameByNid(int Nid)
    {
        // 使用 Nid 作为键来分配名称
        switch (Nid)
        {
            case 4:
                return "初始节点";
            case 2:
                return "战斗节点";
            case 3:
                return "事件节点";
            case 1:
                return "Boss节点";
            default:
                return "普通节点";
        }
    }
}
