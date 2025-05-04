using Assets._Scripts.Helpers;
using Assets._Scripts.Leaderboard;
using Assets._Scripts.Leaderboard.DependenciesContainer;
using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

internal class EditNameCanvasController : SingletonBehaviour<EditNameCanvasController>
{
    [SerializeField] private RectTransform panel;
    [SerializeField] private RectTransform scoreContainer;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private Button submitButton;
    [SerializeField] private Button exitButton;

    public event Action OnEditNamePanelClose;

    protected override void Awake()
    {
        base.Awake();

        nameInputField.onValueChanged.AddListener((playerName) =>
        {
            if (playerName.Length < 3)
            {
                submitButton.interactable = false;
            }
            else
            {
                submitButton.interactable = true;
            }
        });

        submitButton.onClick.AddListener(Submit);
        exitButton.onClick.AddListener(ExitPanel);
    }

    public void DisplayEditNamePanel()
    {
        nameInputField.text = StringHelpers.RemoveUGSSuffix(DependencyContainer.AuthenticationManager.PlayerName);

        panel.localScale = Vector3.zero;

        panel.gameObject.SetActive(true);

        scoreContainer.DOScale(Vector3.zero, 0.3f)//.SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                panel.DOScale(Vector3.one, 0.5f).SetEase(Ease.InBack);
            });
    }

    private async void Submit()
    {
        await LeaderboardManager.Instance.UpdatePlayerName(nameInputField.text);
        ExitPanel();
    }

    private void ExitPanel()
    {
        panel.DOScale(Vector3.zero, 0.3f)
            .OnComplete(() =>
            {
                OnEditNamePanelClose?.Invoke();
                panel.gameObject.SetActive(false);
            });
    }
}