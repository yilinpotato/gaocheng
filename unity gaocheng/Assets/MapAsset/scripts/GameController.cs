using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public string LastMapSceneName { get; set; }

    public MapManager mapManager;

    public static GameController Instance { get; private set; }

    // �������
    public int RandomSeed { get; private set; }

    // Start is called before the first frame update  
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // �����������
        RandomSeed = GenerateRandomSeed();
        Debug.Log("�������: " + RandomSeed);

        // ֻ���ڵ�ͼδ���ɵ�����²����ɵ�ͼ
        if (!MapManager.isMapGenerated)
        {
            mapManager.GenerateMap();
        }
        else
        {
            Debug.Log("��ͼ�Ѵ��ڣ�������������");
        }
    }

    // Update is called once per frame  
    void Update()
    {

    }

    // ����������ӵķ���
    private int GenerateRandomSeed()
    {
        return System.DateTime.Now.Millisecond; // ʹ�õ�ǰʱ����������
    }
}
