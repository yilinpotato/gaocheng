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
        Debug.Log("=== GrowthEventUIController Start ===");
        
        // 自动查找组件（如果引用为空）
        FindUIComponents();
        
        // 检查按钮引用
        Debug.Log($"shieldButton: {(shieldButton != null ? "存在" : "为空")}");
        Debug.Log($"hammerButton: {(hammerButton != null ? "存在" : "为空")}");
        Debug.Log($"phantomButton: {(phantomButton != null ? "存在" : "为空")}");
        Debug.Log($"eventPanel: {(eventPanel != null ? "存在" : "为空")}");
        
        // 检查EventSystem
        if (UnityEngine.EventSystems.EventSystem.current == null)
        {
            Debug.LogError("场景中没有EventSystem！按钮将无法点击");
        }
        else
        {
            Debug.Log("EventSystem 存在");
        }
        
        // 绑定按钮事件（添加空值检查）
        SetupButtons();

        // 重要：直接显示UI，不隐藏
        ShowEventUIDirectly();
    }

    private void FindUIComponents()
    {
        // 自动查找eventPanel
        if (eventPanel == null)
        {
            eventPanel = transform.Find("MainPanel")?.gameObject;
            if (eventPanel == null)
            {
                // 在父对象中查找
                eventPanel = GetComponentInChildren<Transform>().Find("MainPanel")?.gameObject;
            }
            Debug.Log($"自动查找 eventPanel: {(eventPanel != null ? "成功" : "失败")}");
        }

        // 自动查找按钮
        if (shieldButton == null && eventPanel != null)
        {
            shieldButton = eventPanel.transform.Find("ButtonA")?.GetComponent<Button>();
            Debug.Log($"自动查找 shieldButton: {(shieldButton != null ? "成功" : "失败")}");
        }

        if (hammerButton == null && eventPanel != null)
        {
            hammerButton = eventPanel.transform.Find("ButtonB")?.GetComponent<Button>();
            Debug.Log($"自动查找 hammerButton: {(hammerButton != null ? "成功" : "失败")}");
        }

        if (phantomButton == null && eventPanel != null)
        {
            phantomButton = eventPanel.transform.Find("ButtonC")?.GetComponent<Button>();
            Debug.Log($"自动查找 phantomButton: {(phantomButton != null ? "成功" : "失败")}");
        }
    }

    private void SetupButtons()
    {
        if (shieldButton != null)
        {
            shieldButton.onClick.RemoveAllListeners();
            shieldButton.onClick.AddListener(() => OnButtonClicked(0));
            shieldButton.interactable = true;
            Debug.Log("shieldButton 事件绑定成功");
        }
        else
        {
            Debug.LogError("shieldButton 为空，无法绑定事件！");
        }
        
        if (hammerButton != null)
        {
            hammerButton.onClick.RemoveAllListeners();
            hammerButton.onClick.AddListener(() => OnButtonClicked(1));
            hammerButton.interactable = true;
            Debug.Log("hammerButton 事件绑定成功");
        }
        else
        {
            Debug.LogError("hammerButton 为空，无法绑定事件！");
        }
        
        if (phantomButton != null)
        {
            phantomButton.onClick.RemoveAllListeners();
            phantomButton.onClick.AddListener(() => OnButtonClicked(2));
            phantomButton.interactable = true;
            Debug.Log("phantomButton 事件绑定成功");
        }
        else
        {
            Debug.LogError("phantomButton 为空，无法绑定事件！");
        }
    }

    // 直接显示UI，不需要回调
    private void ShowEventUIDirectly()
    {
        Debug.Log("=== 直接显示EventUI ===");
        
        if (eventPanel != null)
        {
            eventPanel.SetActive(true);
            Debug.Log("eventPanel 已激活");
            
            // 检查Canvas设置
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                Debug.Log($"Canvas找到 - Render Mode: {canvas.renderMode}");
                Debug.Log($"Canvas Sort Order: {canvas.sortingOrder}");
                
                // 检查GraphicRaycaster
                var raycaster = canvas.GetComponent<GraphicRaycaster>();
                if (raycaster == null)
                {
                    Debug.LogError("Canvas缺少GraphicRaycaster！");
                    canvas.gameObject.AddComponent<GraphicRaycaster>();
                    Debug.Log("已添加GraphicRaycaster");
                }
                else
                {
                    Debug.Log($"GraphicRaycaster存在，启用: {raycaster.enabled}");
                }
            }
            
            // 检查按钮状态
            CheckButtonInteractabilityDetailed();
        }
        else
        {
            Debug.LogError("eventPanel 为空！无法显示UI");
        }
    }

    // 保留原有的ShowEventUI方法，但去掉隐藏逻辑
    public void ShowEventUI(Action<int> callback)
    {
        Debug.Log("=== ShowEventUI 被调用 ===");
        
        onOptionSelected = callback;
        ShowEventUIDirectly();
    }

    // 修改HideEventUI方法，添加调试信息但不实际隐藏
    public void HideEventUI()
    {
        Debug.Log("HideEventUI 被调用，但不执行隐藏操作");
        // 注释掉隐藏逻辑
        /*
        if (eventPanel != null)
        {
            eventPanel.SetActive(false);
            Debug.Log("eventPanel 已隐藏");
        }
        */
    }

    private void OnButtonClicked(int option)
    {
        Debug.Log($"=== 按钮被点击！选项: {option} ===");
        
        if (onOptionSelected != null)
        {
            Debug.Log("调用 onOptionSelected 回调");
            onOptionSelected.Invoke(option);
        }
        else
        {
            Debug.LogWarning("onOptionSelected 回调为空，直接返回地图");
            ShowMapPanel();
        }
    }
    
    // 临时测试方法保持不变
    void Update()
    {
        // 临时测试：按数字键1,2,3来模拟按钮点击
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("键盘测试：按下1键");
            OnButtonClicked(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("键盘测试：按下2键");
            OnButtonClicked(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("键盘测试：按下3键");
            OnButtonClicked(2);
        }
    }

    private void CheckButtonInteractabilityDetailed()
    {
        Debug.Log("=== 详细检查按钮状态 ===");

        CheckSingleButton(shieldButton, "shieldButton");
        CheckSingleButton(hammerButton, "hammerButton");
        CheckSingleButton(phantomButton, "phantomButton");
    }

    private void ShowMapPanel()
    {
        // 直接加载地图场景
        try
        {
            Debug.Log("返回地图场景...");
            UnityEngine.SceneManagement.SceneManager.LoadScene("MapScene");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load MapScene: {e.Message}");
        }
    }

    private void CheckSingleButton(Button button, string buttonName)
    {
        if (button != null)
        {
            Debug.Log($"=== {buttonName} 详细检查 ===");
            Debug.Log($"  GameObject激活: {button.gameObject.activeInHierarchy}");
            Debug.Log($"  Button可交互: {button.interactable}");
            
            // 检查Image组件
            var image = button.GetComponent<Image>();
            if (image != null)
            {
                Debug.Log($"  Image Raycast Target: {image.raycastTarget}");
                image.raycastTarget = true; // 强制启用
            }
            else
            {
                Debug.LogWarning($"  {buttonName} 没有Image组件！");
            }
            
            // 强制设置按钮状态
            button.interactable = true;
            
            Debug.Log($"  {buttonName} 检查完成");
        }
        else
        {
            Debug.LogError($"{buttonName} 为空！");
        }
    }
}