using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine.UI;
using System;

public class GrowthEventUIController : MonoBehaviour
{
    public GameObject eventPanel;
    public Button shieldButton;
    public Button hammerButton;
    public Button phantomButton;

    private Action<int> onOptionSelected;

    void Start()
    {
        // 绑定按钮事件
        shieldButton.onClick.AddListener(() => OnButtonClicked(0));
        hammerButton.onClick.AddListener(() => OnButtonClicked(1));
        phantomButton.onClick.AddListener(() => OnButtonClicked(2));

        HideEventUI();
    }

    public void ShowEventUI(Action<int> callback)
    {
        onOptionSelected = callback;
        eventPanel.SetActive(true);
    }

    public void HideEventUI()
    {
        eventPanel.SetActive(false);
    }

    private void OnButtonClicked(int option)
    {
        onOptionSelected?.Invoke(option);
    }
}