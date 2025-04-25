using Assets._Scripts.Leaderboard;
using DG.Tweening;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

internal class EditNameCanvasController : SingletonBehaviour<EditNameCanvasController>
{
    [SerializeField] private RectTransform panel;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private Button submitButton;

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
    }

    public async void DisplayEditNameCanvas()
    {
        await AuthenticationManager.SignInAnonymouslyIfNotSignedIn();
        nameInputField.text = AuthenticationService.Instance.PlayerName;

        panel.gameObject.SetActive(true);

        panel.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }

    private async void Submit()
    {
        await LeaderboardManager.Instance.UpdatePlayerName(nameInputField.text);
        panel.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
        {
            panel.gameObject.SetActive(false);
        });
    }
}