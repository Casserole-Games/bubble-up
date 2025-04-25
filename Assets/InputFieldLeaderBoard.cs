using Assets._Scripts.Leaderboard;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class InputFieldLeaderBoard : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text scoreText;

    private int playerScore;

    private void Awake()
    {
        button.onClick.AddListener(async () => 
            await LeaderboardUIManager.Instance.UpdateUI(false, playerScore));
        inputField.onEndEdit.AddListener((text) =>
        {
            if (text.Length > 2)
            {
                button.interactable = true;
            }
            else
            {
                button.interactable = false;
            }
        });
    }

    public void UpdateScore(int score)
    {
        playerScore = score;
        scoreText.text = playerScore.ToString();
    }

}
