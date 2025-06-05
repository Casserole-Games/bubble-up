using Assets._Scripts;
using Assets._Scripts.Helpers;
using Assets._Scripts.Leaderboard;
using Assets._Scripts.Leaderboard.DependenciesContainer;
using DG.Tweening;
using EasyTextEffects.Editor.MyBoxCopy.Extensions;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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

        submitButton.onClick.AddListener(() => {
            SFXManager.Instance.PlayOneShot("button", GameParameters.Instance.UIClickVolume, 1.1f, 1.1f);
            Submit();
        });
        exitButton.onClick.AddListener(() => {
            SFXManager.Instance.PlayOneShot("button", GameParameters.Instance.UIClickVolume, 0.9f, 0.9f);
            ExitPanel(); 
        });
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) &&
            EventSystem.current.currentSelectedGameObject == nameInputField.gameObject)
        {
            submitButton.onClick.Invoke();
        }

        string input = Input.inputString;
        if (!string.IsNullOrEmpty(input))
        {
            foreach (char c in input)
            {
                if (char.IsLetter(c))
                {
                    EventSystem.current.SetSelectedGameObject(nameInputField.gameObject);
                    if (!nameInputField.isFocused)
                    {
                        nameInputField.ActivateInputField();
                        nameInputField.text = c.ToString();
                        nameInputField.selectionFocusPosition = 0;
                        nameInputField.MoveTextEnd(false);
                    }
                    return;
                }
            }
        }
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