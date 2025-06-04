using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance;

    [Header("被动道具槽位")]
    public int passiveSlotLimit = 6;

    // 当前持有道具
    public List<ItemData> PassiveItems { get; private set; } = new List<ItemData>();
    public ItemData ActiveItem { get; private set; }

    private PlayerStats playerStats;

    private void Awake()
    {
        Instance = this;
        playerStats = GetComponent<PlayerStats>();
        LoadInventory();
    }

    // 拾取道具
    public void PickupItem(ItemData item)
    {
        switch (item.type)
        {
            case ItemType.Passive:
                if (PassiveItems.Count < passiveSlotLimit)
                {
                    PassiveItems.Add(item);
                    item.ApplyEffect(playerStats);
                    SaveInventory();
                }
                break;
            case ItemType.Active:
                
                if (ActiveItem != null) ActiveItem.RemoveEffect(playerStats);
                ActiveItem = item;
                item.ApplyEffect(playerStats);
                SaveInventory();
                break;
        }

       
    }

    // 数据持久化
    private void SaveInventory()
    {
        // 实现JSON序列化保存逻辑
    }

    private void LoadInventory()
    {
        // 实现加载逻辑
    }
}