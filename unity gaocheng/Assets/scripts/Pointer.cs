using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour
{
    private Node currentNode; // 当前指针指向的节点
    public float offsetDistance = 2.0f; // 指针与节点的垂直偏移距离


    public void SetcurrentNode(Node currentnode)
    {
        currentNode = currentnode;
    }

    public void Initialize(Node initialNode)
    {
        currentNode = initialNode;
        if (currentNode != null)
        {
            // 设置指针位置为初始节点下方一定距离
            Vector3 offset = new Vector3(0, -offsetDistance, 0);
            transform.position = currentNode.transform.position + offset;
            gameObject.transform.position = currentNode.transform.position + offset; // 移动 GameObject
        }
    }

    public void MoveTo(Node targetNode)
    {
        if (targetNode != null)
        {
            // 更新指针位置为目标节点下方一定距离
            Vector3 offset = new Vector3(0, -offsetDistance, 0);
            transform.position = targetNode.transform.position + offset;
            gameObject.transform.position = targetNode.transform.position + offset; // 移动 GameObject
            currentNode = targetNode; // 更新当前节点
        }
    }

    public Node GetCurrentNode()
    {
        return currentNode;
    }
}
