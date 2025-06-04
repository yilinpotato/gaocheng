using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour
{
    private Node currentNode; // ��ǰָ��ָ��Ľڵ�
    public float offsetDistance = 2.0f; // ָ����ڵ�Ĵ�ֱƫ�ƾ���


    public void SetcurrentNode(Node currentnode)
    {
        currentNode = currentnode;
    }

    public void Initialize(Node initialNode)
    {
        currentNode = initialNode;
        if (currentNode != null)
        {
            // ����ָ��λ��Ϊ��ʼ�ڵ��·�һ������
            Vector3 offset = new Vector3(0, -offsetDistance, 0);
            transform.position = currentNode.transform.position + offset;
            gameObject.transform.position = currentNode.transform.position + offset; // �ƶ� GameObject
        }
    }

    public void MoveTo(Node targetNode)
    {
        if (targetNode != null)
        {
            // ����ָ��λ��ΪĿ��ڵ��·�һ������
            Vector3 offset = new Vector3(0, -offsetDistance, 0);
            transform.position = targetNode.transform.position + offset;
            gameObject.transform.position = targetNode.transform.position + offset; // �ƶ� GameObject
            currentNode = targetNode; // ���µ�ǰ�ڵ�
        }
    }

    public Node GetCurrentNode()
    {
        return currentNode;
    }
}
