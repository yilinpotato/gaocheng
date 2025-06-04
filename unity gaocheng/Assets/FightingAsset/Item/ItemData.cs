using UnityEngine;

public enum ItemType { Passive, Active, Consumable }

public abstract class ItemData : ScriptableObject
{
    [Header("基础属性")]
    public string itemName;
    public Sprite icon;
    public ItemType type;
    [TextArea] public string description;

    // 应用效果
    public abstract void ApplyEffect(PlayerStats stats);

    // 移除效果
    public virtual void RemoveEffect(PlayerStats stats) { }
}