[System.Serializable]
public class PassiveItemEntry
{
    public string itemName;
    public string type;
    public string description;
    public float healthBoost;
    public float speedBoost;
    public float damageBoost;
}

[System.Serializable]
public class PassiveItemList
{
    public PassiveItemEntry[] items;
}
