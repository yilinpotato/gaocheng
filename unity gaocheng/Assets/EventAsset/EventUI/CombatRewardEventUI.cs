// BattleEventUI.cs
using UnityEngine;

public class BattleEventUI : MonoBehaviour
{
    public GameObject canvas;
    protected CombatNode battleNode;

    void Start()
    {
        canvas.SetActive(false);
    }

    public void Show(CombatNode node)
    {
        battleNode = node;
        canvas.SetActive(true);
    }

    public void Escape()
    {
        canvas.SetActive(false);
        Debug.Log("成功逃脱！");
        // 使用统一的事件结束方法
        FindObjectOfType<EventSceneManager>()?.EndEvent();
    }

    public void Fight()
    {
        canvas.SetActive(false);
        battleNode.StartBattle();
    }
}