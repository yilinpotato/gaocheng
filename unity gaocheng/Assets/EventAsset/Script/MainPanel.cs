using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : BasePanel
{

    private Transform UILottery;
    private Transform UIPackage;
    private Transform UIQuitBtn;

    protected override void Awake()
    {
        base.Awake();
        InitUI();
    }

    private void InitUI()
    {
        UILottery = transform.Find("Top/LotteryBtn");
        UIPackage = transform.Find("Top/PackageBtn");
        UIQuitBtn = transform.Find("BottomLeft/QuitBtn");
        UILottery.GetComponent<Button>().onClick.AddListener(OnBtnLottery);
        UIPackage.GetComponent<Button>().onClick.AddListener(OnBtnPackage);
        UIQuitBtn.GetComponent<Button>().onClick.AddListener(OnQuitGame);
    }

    private void OnBtnLottery()
    {
        print(">>>>> OnBtnLottery");
        UIManager.Instance.OpenPanel(UIConst.LotteryPanel);
        ClosePanel();
    }

    private void OnBtnPackage()
    {
        print(">>>>> OnBtnPackage");
        UIManager.Instance.OpenPanel(UIConst.PackagePanel);
        ClosePanel();
    }

    private void OnQuitGame()
    {
        print(">>>>> OnQuitGame");
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
