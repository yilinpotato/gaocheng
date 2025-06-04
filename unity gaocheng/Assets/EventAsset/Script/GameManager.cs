using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    private PackageTable packageTable;

    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // todo
        UIManager.Instance.OpenPanel(UIConst.MainPanel);
        // print(GetPackageLocalData().Count);
        // print(GetPackageTable().DataList.Count);
    }

    public void DeletePackageItems(List<string> uids)
    {
        foreach (string uid in uids)
        {
            DeletePackageItem(uid, false);
        }
        PackageLocalData.Instance.SavePackage();
    }

    public void DeletePackageItem(string uid, bool needSave = true)
    {
        PackageLocalItem packageLocalItem = GetPackageLocalItemByUId(uid);
        if (packageLocalItem == null)
            return;
        PackageLocalData.Instance.items.Remove(packageLocalItem);
        if (needSave)
        {
            PackageLocalData.Instance.SavePackage();
        }
    }

    public PackageTable GetPackageTable()
    {
        if (packageTable == null)
        {
            packageTable = Resources.Load<PackageTable>("TableData/PackageTable");
        }
        return packageTable;
    }

    // 1: 武器， 2：食物
    // 根据类型获取配置的表格数据
    public List<PackageTableItem> GetPackageTableByType(int type)
    {
        List<PackageTableItem> packageItems = new List<PackageTableItem>();
        foreach (PackageTableItem packageItem in GetPackageTable().DataList)
        {
            if (packageItem.type == type)
            {
                packageItems.Add(packageItem);
            }
        }
        return packageItems;
    }

    // 随机抽卡，获得一件武器
    public PackageLocalItem GetLotteryRandom1()
    {
        List<PackageTableItem> packageItems = GetPackageTableByType(GameConst.PackageTypeWeapon);   
        int index = Random.Range(0, packageItems.Count);
        PackageTableItem packageItem = packageItems[index];
        PackageLocalItem packageLocalItem = new()
        {
            uid = System.Guid.NewGuid().ToString(),
            id = packageItem.id,
            num = 1,
            level = 1,
            isNew = CheckWeaponIsNew(packageItem.id),
        };
        PackageLocalData.Instance.items.Add(packageLocalItem);
        PackageLocalData.Instance.SavePackage();
        return packageLocalItem;
    }

    // 随机抽卡，获得十件武器
    public List<PackageLocalItem> GetLotteryRandom10(bool sort = false)
    {
        // 随机抽卡
        List<PackageLocalItem> packageLocalItems = new();
        for (int i = 0; i < 10; i++)
        {
            PackageLocalItem packageLocalItem = GetLotteryRandom1();
            packageLocalItems.Add(packageLocalItem);
        }
        // 武器排序
        if (sort)
        {
            packageLocalItems.Sort(new PackageItemComparer());
        }
        return packageLocalItems;
    }

    public bool CheckWeaponIsNew(int id)
    {
        foreach (PackageLocalItem packageLocalItem in GetPackageLocalData())
        {
            if (packageLocalItem.id == id)
            {
                return false;
            }
        }
        return true;
    }


    public List<PackageLocalItem> GetPackageLocalData()
    {
        return PackageLocalData.Instance.LoadPackage();
    }

    public PackageTableItem GetPackageItemById(int id)
    {
        List<PackageTableItem> packageDataList = GetPackageTable().DataList;
        foreach (PackageTableItem item in packageDataList)
        {
            if (item.id == id)
            {
                return item;
            }
        }
        return null;
    }

    public PackageLocalItem GetPackageLocalItemByUId(string uid)
    {
        List<PackageLocalItem> packageDataList = GetPackageLocalData();
        foreach (PackageLocalItem item in packageDataList)
        {
            if (item.uid == uid)
            {
                return item;
            }
        }
        return null;
    }


    public List<PackageLocalItem> GetSortPackageLocalData()
    {
        List<PackageLocalItem> localItems = PackageLocalData.Instance.LoadPackage();
        localItems.Sort(new PackageItemComparer());
        return localItems;
    }

}


public class PackageItemComparer : IComparer<PackageLocalItem>
{
    public int Compare(PackageLocalItem a, PackageLocalItem b)
    {
        PackageTableItem x = GameManager.Instance.GetPackageItemById(a.id);
        PackageTableItem y = GameManager.Instance.GetPackageItemById(b.id);
        // 首先按star从大到小排序
        int starComparison = y.star.CompareTo(x.star);

        // 如果star相同，则按id从大到小排序
        if (starComparison == 0)
        {
            int idComparison = y.id.CompareTo(x.id);
            if (idComparison == 0)
            {
                return b.level.CompareTo(a.level);
            }
            return idComparison;
        }

        return starComparison;
    }
}

public class GameConst
{
    // 武器类型
    public const int PackageTypeWeapon = 1;
    // 食物类型
    public const int PackageTypeFood = 2;
}