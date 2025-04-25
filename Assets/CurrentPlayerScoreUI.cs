using Assets._Scripts.Leaderboard;
using UnityEngine;
using UnityEngine.UI;

internal class CurrentPlayerScoreUI : PlayerScoreUI
{
    [SerializeField] private Button editNameButton;
    private Canvas editNameCanvas;

    private void Awake()
    {
        editNameButton.onClick.AddListener(DisplayEditNameCanvas);
    }

    private void DisplayEditNameCanvas()
    {
        if (editNameCanvas != null)
        {
            EditNameCanvasController.Instance.DisplayEditNameCanvas();
        }
    }
}
