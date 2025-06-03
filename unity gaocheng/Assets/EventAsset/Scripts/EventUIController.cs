using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine.UI;
using System;

public class EventUIController : MonoBehaviour
{
    public GameObject choicePanel;
    public Button choiceAButton;
    public Button choiceBButton;

    private Action<string> onChoiceSelected;

    void Start()
    {
        choicePanel.SetActive(false);
    }

    public void ShowChoicePanel(Action<string> onChoice)
    {
        choicePanel.SetActive(true);
        onChoiceSelected = onChoice;

        choiceAButton.onClick.AddListener(() => SelectChoice("A"));
        choiceBButton.onClick.AddListener(() => SelectChoice("B"));
    }

    public void HideChoicePanel()
    {
        choicePanel.SetActive(false);
        choiceAButton.onClick.RemoveAllListeners();
        choiceBButton.onClick.RemoveAllListeners();
    }

    void SelectChoice(string choice)
    {
        onChoiceSelected?.Invoke(choice);
    }
}
