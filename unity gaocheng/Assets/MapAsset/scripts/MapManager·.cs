using System.Collections.Generic;
using System.Collections;
using System.Security.Cryptography;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement; // 添加场景管理引用


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
    public Sprite InitialNodeSprite;
    public Sprite CombatNodeSprite;
    public Sprite EventNodeSprite;
    public Sprite BossNodeSprite;
    private List<VirtualNode> virtualNodes = new List<VirtualNode>();
    private List<VirtualEdge> virtualEdges = new List<VirtualEdge>();

    // 添加标志位，表示地图是否已经生成
    public static bool isMapGenerated = false;

    // 添加属性来标识是否是从战斗返回
    public static bool isReturningFromBattle = false;

    // 地图节点和边的存储容器 - 改为非静态，避免多个实例间共享
    private Dictionary<int, GameObject> nodeObjects = new Dictionary<int, GameObject>();
    private List<GameObject> edgeObjects = new List<GameObject>();

    // 添加序列化保存的节点位置和类型信息
    [System.Serializable]
    public class SerializedNode
    {
        public int id;
        public string nodeType;
        public Vector3 position;
        public bool isVisited;
    }

    private List<SerializedNode> serializedNodes = new List<SerializedNode>();
    private List<VirtualEdge> serializedEdges = new List<VirtualEdge>();

    public Material ropeMaterial;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 保持MapManager在场景切换时不被销毁
            SceneManager.sceneLoaded += OnSceneLoaded; // 添加场景加载回调
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 如果加载的是地图场景且是从战斗返回
        if (scene.name == "MapScene" && isReturningFromBattle)
        {
            Debug.Log("检测到加载地图场景，准备恢复节点和边");
            StartCoroutine(RestoreMapState());
            isReturningFromBattle = false;
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 添加恢复地图状态的方法
    private IEnumerator RestoreMapState()
    {
        yield return new WaitForSeconds(0.5f); // 给场景加载更多时间

        Debug.Log("开始恢复地图状态");

        if (serializedNodes.Count == 0)
        {
            Debug.LogError("没有可恢复的地图数据!");
            yield break;
        }

        // 修复过时的FindObjectsOfType方法
        foreach (var obj in Object.FindObjectsByType<Node>(FindObjectsSortMode.None))
        {
            if (obj != null && obj.gameObject != null)
            {
                Destroy(obj.gameObject);
            }
        }

        // 清理可能存在的旧边
        foreach (var obj in GameObject.FindGameObjectsWithTag("Edge"))
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }

        // 等待清理完成
        yield return new WaitForSeconds(0.2f);

        // 从序列化数据重新创建节点
        Dictionary<int, Node> reinstantiatedNodes = new Dictionary<int, Node>();
        nodeObjects.Clear();

        foreach (var serializedNode in serializedNodes)
        {
            GameObject roomGO = Instantiate(RoomPrefeb, serializedNode.position, Quaternion.identity);
            roomGO.name = $"Node_{serializedNode.id}";
            nodeObjects[serializedNode.id] = roomGO;

            // 根据节点类型添加对应的组件
            Node node;
            switch (serializedNode.nodeType)
            {
                case "InitialNode":
                    node = roomGO.AddComponent<InitialNode>();
                    // 修复SetNodeStyle调用
                    this.SetNodeStyle(roomGO, InitialNodeSprite);
                    break;
                case "BossNode":
                    node = roomGO.AddComponent<BossNode>();
                    this.SetNodeStyle(roomGO, BossNodeSprite);
                    break;
                case "EventNode":
                    node = roomGO.AddComponent<EventNode>();
                    this.SetNodeStyle(roomGO, EventNodeSprite);
                    break;
                case "CombatNode":
                default:
                    CombatNode combatNode = roomGO.AddComponent<CombatNode>();
                    node = combatNode;
                    this.SetNodeStyle(roomGO, CombatNodeSprite);
                    break;
            }

            // 设置节点属性
            node.SetId(serializedNode.id);
            node.Setnodeid(this.GenerateNodeNumber(serializedNode.id, serializedNode.nodeType));
            node.SetNodeDescription(this.GetDescriptionByNid(node.Nid));
            node.SetNodeName(this.GetNameByNid(node.Nid));
            node.IsVisited = serializedNode.isVisited;

            // 如果节点已访问，更新视觉效果
            if (serializedNode.isVisited)
            {
                var sr = roomGO.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.color = new Color(0.5f, 0.5f, 0.5f, 1f);
                }
            }

            reinstantiatedNodes[serializedNode.id] = node;
            AddNode(node);
        }

        // 重新创建边
        edgeObjects.Clear();
        foreach (var edge in serializedEdges)
        {
            if (reinstantiatedNodes.TryGetValue(edge.Node1Id, out Node node1) &&
                reinstantiatedNodes.TryGetValue(edge.Node2Id, out Node node2))
            {
                AddEdge(node1, node2);
            }
        }

        // 恢复指针位置
        Pointer pointer = Object.FindFirstObjectByType<Pointer>();
        if (pointer != null)
        {
            // 找到当前节点 - 通常是最后一个被访问的节点
            Node currentNode = null;
            foreach (var node in reinstantiatedNodes.Values)
            {
                if (node.IsVisited)
                {
                    currentNode = node;
                    // 如果是初始节点，优先选择
                    if (node is InitialNode)
                        break;
                }
            }

            // 如果找不到已访问节点，使用第一个节点
            if (currentNode == null && reinstantiatedNodes.Count > 0)
            {
                currentNode = reinstantiatedNodes.Values.GetEnumerator().Current;
            }

            if (currentNode != null)
            {
                pointer.SetcurrentNode(currentNode);
                pointer.Initialize(currentNode);
            }
        }

        Debug.Log($"地图状态恢复完成，恢复了{reinstantiatedNodes.Count}个节点和{edgeObjects.Count}条边");
    }

    public void PrepareForBattle()
    {
        isReturningFromBattle = true;

        // 保存节点信息以便稍后恢复
        serializedNodes.Clear();
        foreach (var pair in nodeObjects)
        {
            Node node = pair.Value.GetComponent<Node>();
            if (node != null)
            {
                string nodeType = nodeTypeMap.TryGetValue(node.Id, out string type) ? type : "Unknown";
                SerializedNode serializedNode = new SerializedNode
                {
                    id = node.Id,
                    nodeType = nodeType,
                    position = pair.Value.transform.position,
                    isVisited = node.IsVisited
                };
                serializedNodes.Add(serializedNode);
            }
        }

        // 保存边信息
        serializedEdges.Clear();
        serializedEdges.AddRange(virtualEdges); // 假设virtualEdges没有引用游戏对象，只是纯数据

        Debug.Log($"已保存地图状态：{serializedNodes.Count}个节点和{serializedEdges.Count}条边");
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

            // 添加虚拟边数据（如果需要）
            bool edgeExists = false;
            foreach (var edge in virtualEdges)
            {
                if ((edge.Node1Id == node1.Id && edge.Node2Id == node2.Id) ||
                    (edge.Node1Id == node2.Id && edge.Node2Id == node1.Id))
                {
                    edgeExists = true;
                    break;
                }
            }

            if (!edgeExists)
            {
                virtualEdges.Add(new VirtualEdge { Node1Id = node1.Id, Node2Id = node2.Id });
            }
        }
    }

    // 生成地图
    public void GenerateMap()
    {
        // 如果地图已经生成，直接返回
        if (isMapGenerated)
        {
            Debug.Log("地图已存在，跳过生成");
            return;
        }

        Debug.Log("开始生成新地图");

        // 清空旧数据
        ClearMap();

        // 创建虚拟节点
        int totalNodeCount = Random.Range(10, 16);
        for (int i = 0; i < totalNodeCount; i++)
        {
            VirtualNode virtualNode = new VirtualNode
            {
                Id = currentId++,
                Position = this.GenerateValidPosition() // 使用this引用内部方法
            };
            virtualNodes.Add(virtualNode);
        }

        // 创建虚拟边并形成最小生成树
        this.ConnectVirtualNodesWithMST(); // 使用this引用内部方法

        // 根据连接数分配节点类型
        this.AssignVirtualNodeTypes(); // 使用this引用内部方法

        // 根据虚拟信息实例化节点和边
        InstantiateNodesAndEdges();

        // 标记地图已生成
        isMapGenerated = true;

        Debug.Log("地图生成完成");
    }

    // 将私有方法改为公共方法
    public void SetNodeStyle(GameObject nodeObject, Sprite sprite)
    {
        SpriteRenderer spriteRenderer = nodeObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = nodeObject.AddComponent<SpriteRenderer>();
        }
        spriteRenderer.sprite = sprite;
    }

    public Vector3 GenerateValidPosition()
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

    // 改成public方法
    public void ConnectVirtualNodesWithMST()
    {
        HashSet<VirtualNode> connectedNodes = new HashSet<VirtualNode>();
        List<VirtualNode> unconnectedNodes = new List<VirtualNode>(virtualNodes);

        if (unconnectedNodes.Count == 0)
        {
            Debug.LogError("没有节点可以连接!");
            return;
        }

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

                // 确保Neighbors列表已初始化
                if (fromNode.Neighbors == null)
                    fromNode.Neighbors = new List<int>();
                if (closestNode.Neighbors == null)
                    closestNode.Neighbors = new List<int>();

                fromNode.Neighbors.Add(closestNode.Id);
                closestNode.Neighbors.Add(fromNode.Id);
                connectedNodes.Add(closestNode);
                unconnectedNodes.Remove(closestNode);
            }
        }
    }

    // 改成public方法
    public void AssignVirtualNodeTypes()
    {
        VirtualNode initialNode = null;
        VirtualNode bossNode = null;
        int maxConnections = -1;

        // 找到连接数最多的节点，设置为初始节点
        foreach (VirtualNode node in virtualNodes)
        {
            // 确保Neighbors列表已初始化
            if (node.Neighbors == null)
                node.Neighbors = new List<int>();

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
            if (node.Neighbors != null && node.Neighbors.Count == 1 && node.NodeType == null)
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
                if (node.Neighbors == null)
                    node.Neighbors = new List<int>();

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

            // 保存GameObject引用
            nodeObjects[virtualNode.Id] = roomGO;

            // 根据节点类型添加对应的组件
            Node node;
            switch (virtualNode.NodeType)
            {
                case "InitialNode":
                    node = roomGO.AddComponent<InitialNode>();
                    Pointer pointer = Object.FindFirstObjectByType<Pointer>();

                    if (pointer != null)
                    {
                        pointer.SetcurrentNode(node);
                    }
                    else
                    {
                        Debug.LogError("Pointer instance not found in the scene.");
                    }
                    pointer.Initialize(node); // 初始化指针位置
                    this.SetNodeStyle(roomGO, InitialNodeSprite); // 使用this引用方法
                    break;
                case "BossNode":
                    node = roomGO.AddComponent<BossNode>();
                    this.SetNodeStyle(roomGO, BossNodeSprite); // 使用this引用方法
                    break;
                case "EventNode":
                    node = roomGO.AddComponent<EventNode>();
                    this.SetNodeStyle(roomGO, EventNodeSprite); // 使用this引用方法
                    break;
                case "CombatNode":
                default:
                    CombatNode combatNode = roomGO.AddComponent<CombatNode>();
                    node = combatNode;
                    this.SetNodeStyle(roomGO, CombatNodeSprite); // 使用this引用方法
                    break;
            }

            // 设置节点 ID ，Nid并添加到图中
            node.SetId(virtualNode.Id);
            node.Setnodeid(this.GenerateNodeNumber(virtualNode.Id, virtualNode.NodeType)); // 使用this引用方法
            node.SetNodeDescription(this.GetDescriptionByNid(node.Nid)); // 使用this引用方法
            node.SetNodeName(this.GetNameByNid(node.Nid)); // 使用this引用方法
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
            if (instantiatedNodes.TryGetValue(virtualEdge.Node1Id, out Node node1) &&
                instantiatedNodes.TryGetValue(virtualEdge.Node2Id, out Node node2))
            {
                AddEdge(node1, node2);
            }
            else
            {
                Debug.LogWarning($"创建边失败: 找不到ID为{virtualEdge.Node1Id}或{virtualEdge.Node2Id}的节点");
            }
        }
    }

    // 清除地图
    public void ClearMap()
    {
        // 清理实例化的节点和边
        foreach (var nodeObj in nodeObjects.Values)
        {
            if (nodeObj != null)
            {
                Destroy(nodeObj);
            }
        }

        foreach (var edgeObj in edgeObjects)
        {
            if (edgeObj != null)
            {
                Destroy(edgeObj);
            }
        }

        // 清空数据结构
        nodeObjects.Clear();
        edgeObjects.Clear();
        graph.Clear();
        nodeTypeMap.Clear();
        virtualNodes.Clear();
        virtualEdges.Clear();
        currentId = 0;
    }

    private void DrawEdge(Vector3 start, Vector3 end)
    {
        // 创建一个空的 GameObject 用于存放 LineRenderer
        GameObject lineObject = new GameObject("Edge");
        lineObject.tag = "Edge"; // 添加标签以便能够找到所有边
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

        // 保存边的引用
        edgeObjects.Add(lineObject);
    }

    // 将这些方法改为public
    public int GenerateNodeNumber(int Id, string nodeType)
    {
        // 使用 GameController 的随机种子和节点 ID 初始化随机数生成器
        System.Random random = new System.Random(GameController.Instance.RandomSeed + Id);

        // 根据节点类型生成不同范围的随机编号
        switch (nodeType)
        {
            case "BossNode":
                return random.Next(1, 2); // 修改范围为1-2以避免总是返回1
            case "CombatNode":
                return random.Next(2, 3); // 修改范围为2-3
            case "EventNode":
                return random.Next(3, 4); // 修改范围为3-4
            case "InitialNode":
                return random.Next(4, 5); // 修改范围为4-5
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
