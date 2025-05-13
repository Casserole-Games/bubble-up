using Assets._Scripts.Helpers;
using Assets._Scripts.Leaderboard;
using Assets._Scripts.Leaderboard.DependenciesContainer;
using DG.Tweening;
using EasyTextEffects.Editor.MyBoxCopy.Extensions;
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

    public void DisplayEditNamePanel(bool hideExitButton = false)
    {
        LeaderboardUIManager.Instance.DisplayLeaderboardFrame();
        exitButton.gameObject.SetActive(!hideExitButton);
        nameInputField.text = StringHelpers.RemoveUGSSuffix(DependencyContainer.AuthenticationManager.PlayerName);
        panel.localScale = Vector3.zero;
        panel.gameObject.SetActive(true);
        scoreContainer.DOScale(Vector3.zero, LeaderboardUIManager.Instance.WindowCloseAnimTime).SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                panel.DOScale(Vector3.one, LeaderboardUIManager.Instance.WindowShowAnimTime).SetEase(Ease.OutBack);
            });
    }

    private async void Submit()
    {
        await LeaderboardManager.Instance.UpdatePlayerName(nameInputField.text);
        ExitPanel();
    }

    private void ExitPanel()
    {
        panel.DOScale(Vector3.zero, LeaderboardUIManager.Instance.WindowCloseAnimTime).SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                OnEditNamePanelClose?.Invoke();
                panel.gameObject.SetActive(false);
            });
    }
}