using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PackageMode
{
    normal,
    delete,
    sort,
}


public class PackagePanel : BasePanel
{
    private Transform UIMenu;
    private Transform UIMenuWeapon;
    private Transform UIMenuFood;
    private Transform UITabName;
    private Transform UICloseBtn;
    private Transform UICenter;
    private Transform UIScrollView;
    private Transform UIDetailPanel;
    private Transform UILeftBtn;
    private Transform UIRightBtn;
    private Transform UIDeletePanel;
    private Transform UIDeleteBackBtn;
    private Transform UIDeleteInfoText;
    private Transform UIDeleteConfirmBtn;
    private Transform UIBottomMenus;
    private Transform UIDeleteBtn;
    private Transform UIDetailBtn;

    public GameObject PackageUIItemPrefab;

    // 当前界面处于什么模式？
    public PackageMode curMode = PackageMode.normal;
    public List<string> deleteChooseUid;

    private string _chooseUid;
    public string chooseUID
    {
        get
        {
            return _chooseUid;
        }
        set
        {
            _chooseUid = value;
            RefreshDetail();
        }
    }

    // 添加删除选中项
    public void AddChooseDeleteUid(string uid)
    {
        this.deleteChooseUid ??= new List<string>();
        if (!this.deleteChooseUid.Contains(uid))
        {
            this.deleteChooseUid.Add(uid);
        }
        else
        {
            this.deleteChooseUid.Remove(uid);
        }
        RefreshDeletePanel();
    }

    private void RefreshDeletePanel()
    {
        RectTransform scrollContent = UIScrollView.GetComponent<ScrollRect>().content;
        foreach (Transform cell in scrollContent)
        {
            PackageCell packageCell = cell.GetComponent<PackageCell>();
            packageCell.RefreshDeleteState();
        }
    }


    override protected void Awake()
    {
        base.Awake();
        InitUI();
    }

    private void Start() {
        RefreshUI();
    }

    private void InitUI()
    {
        InitUIName();
        InitClick();
    }

    private void RefreshUI()
    {
        RefreshScroll();
    }

    private void RefreshDetail()
    {
        // 找到uid对应的动态数据
        PackageLocalItem localItem = GameManager.Instance.GetPackageLocalItemByUId(chooseUID);
        // 刷新详情界面
        UIDetailPanel.GetComponent<PackageDetail>().Refresh(localItem, this);
    }

    private void RefreshScroll()
    {
        // 清理滚动容器中原本的物品
        RectTransform scrollContent = UIScrollView.GetComponent<ScrollRect>().content;
        for (int i = 0; i < scrollContent.childCount; i++)
        {
            Destroy(scrollContent.GetChild(i).gameObject);
        }
        foreach (PackageLocalItem localData in GameManager.Instance.GetSortPackageLocalData())
        {
            Transform PackageUIItem = Instantiate(PackageUIItemPrefab.transform, scrollContent) as Transform;
            PackageCell packageCell = PackageUIItem.GetComponent<PackageCell>();
            packageCell.Refresh(localData, this);
        }

    }

    private void InitUIName()
    {
        UIMenu = transform.Find("TopCenter/Menu");
        UIMenuWeapon = transform.Find("TopCenter/Menus/Weapon");
        UIMenuFood = transform.Find("TopCenter/Menus/Food");
        UITabName = transform.Find("LeftTop/TabName");
        UICloseBtn = transform.Find("RightTop/Close");
        UICenter = transform.Find("Center");
        UIScrollView = transform.Find("Center/Scroll View");
        UIDetailPanel = transform.Find("Center/DetailPanel");
        UILeftBtn = transform.Find("Left/Button");
        UIRightBtn = transform.Find("Right/Button");

        UIDeletePanel = transform.Find("Bottom/DeletePanel");
        UIDeleteBackBtn = transform.Find("Bottom/DeletePanel/Back");
        UIDeleteInfoText = transform.Find("Bottom/DeletePanel/InfoText");
        UIDeleteConfirmBtn = transform.Find("Bottom/DeletePanel/ConfirmBtn");
        UIBottomMenus = transform.Find("Bottom/BottomMenus");
        UIDeleteBtn = transform.Find("Bottom/BottomMenus/DeleteBtn");
        UIDetailBtn = transform.Find("Bottom/BottomMenus/DetailBtn");

        UIDeletePanel.gameObject.SetActive(false);
        UIBottomMenus.gameObject.SetActive(true);
    }

    private void InitClick()
    {
        UIMenuWeapon.GetComponent<Button>().onClick.AddListener(OnClickWeapon);
        UIMenuFood.GetComponent<Button>().onClick.AddListener(OnClickFood);
        UICloseBtn.GetComponent<Button>().onClick.AddListener(OnClickClose);
        UILeftBtn.GetComponent<Button>().onClick.AddListener(OnClickLeft);
        UIRightBtn.GetComponent<Button>().onClick.AddListener(OnClickRight);

        UIDeleteBackBtn.GetComponent<Button>().onClick.AddListener(OnDeleteBack);
        UIDeleteConfirmBtn.GetComponent<Button>().onClick.AddListener(OnDeleteConfirm);
        UIDeleteBtn.GetComponent<Button>().onClick.AddListener(OnDelete);
        UIDetailBtn.GetComponent<Button>().onClick.AddListener(OnDetail);

    }
    private void OnClickWeapon()
    {
        print(">>>>> OnClickWeapon");
    }

    private void OnClickFood()
    {
        print(">>>>> OnClickFood");
    }

    private void OnClickClose()
    {
        print(">>>>> OnClickClose");
        ClosePanel();
        UIManager.Instance.OpenPanel(UIConst.MainPanel);
    }

    private void OnClickLeft()
    {
        print(">>>>> OnClickLeft");
    }

    private void OnClickRight()
    {
        print(">>>>> OnClickRight");
    }

    // 退出删除模式
    private void OnDeleteBack()
    {
        print(">>>>> onDeleteBack");
        curMode = PackageMode.normal;
        UIDeletePanel.gameObject.SetActive(false);
        // 重置选中的删除列表
        deleteChooseUid = new List<string>();
        // 刷新选中状态
        RefreshDeletePanel();
    }

    // 确认删除
    private void OnDeleteConfirm()
    {
        print(">>>>> OnDeleteConfirm");
        if (this.deleteChooseUid == null)
        {
            return;
        }
        if (this.deleteChooseUid.Count == 0)
        {
            return;
        }
        GameManager.Instance.DeletePackageItems(this.deleteChooseUid);
        // 删除完后刷新下整个背包页面
        RefreshUI();
    }

    // 进入删除模式: 左下角的删除按钮
    private void OnDelete()
    {
        print(">>>>> OnDelete OnDelete OnDelete");
        curMode = PackageMode.delete;
        UIDeletePanel.gameObject.SetActive(true);
    }

    private void OnDetail()
    {
        print(">>>>> OnDetail");
    }
}
