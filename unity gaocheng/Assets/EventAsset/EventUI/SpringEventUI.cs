// SpringEventUI.cs
using UnityEngine;

public class SpringEventUI : MonoBehaviour
{
    public GameObject canvas;
    private PlayerStats stats;

    void Start()
    {
        stats = FindObjectOfType<PlayerStats>();
        canvas.SetActive(false);
    }

    public void Show()
    {
        Debug.Log("显示温泉事件");
        canvas.SetActive(true);
    }

    public void Hide()
    {
        canvas.SetActive(false);
        // 使用统一的事件结束方法
        FindObjectOfType<EventSceneManager>()?.EndEvent();
    }

    public void Heal30Percent()
    {
        stats.CurrentHP += stats.MaxHP * 0.3f;
        stats.CurrentHP = Mathf.Min(stats.CurrentHP, stats.MaxHP);
        Hide();
    }

    public void BoostMaxHP()
    {
        float boost = stats.MaxHP * 0.1f;
        stats.MaxHP += boost;
        stats.CurrentHP += boost;
        Hide();
    }
}
