using Assets._Scripts.Helpers;
using Assets._Scripts.Leaderboard.DependenciesContainer;
using System;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Leaderboards.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Scripts.Leaderboard
{
    internal class PlayerScoreUI : MonoBehaviour
    {
        public TMP_Text PlayerRankText;
        public TMP_Text PlayerNameText;
        public TMP_Text PlayerScoreText;
        public Image DuckMedal;
        public Button EditNameButton;
        public Color CurrentPlayerColor;

        internal LeaderboardEntry entry;
        private bool isCurrentPlayer = false;


        private void Awake()
        {
            EditNameButton.onClick.AddListener(() => EditNameCanvasController.Instance.DisplayEditNamePanel());
        }

        internal void SetEntryAndIsCurrentPlayer(LeaderboardEntry entry)
        {
            this.entry = entry;
            this.isCurrentPlayer = entry.PlayerId == DependencyContainer.AuthenticationManager.PlayerId;
        }

        public virtual void UpdateUI()
        {
            if (isCurrentPlayer) {
                EditNameButton.gameObject.SetActive(true);
                Image sprite = GetComponent<Image>();
                sprite.color = CurrentPlayerColor;
            }

            if (entry.Rank < 3)
            {
                DuckMedal.gameObject.SetActive(true);
                PlayerRankText.gameObject.SetActive(false);
                DuckMedal.sprite = MedalHolder.Instance.GetMedal(entry.Rank);
            }
            else
            {
                DuckMedal.gameObject.SetActive(false);
                PlayerRankText.gameObject.SetActive(true);
                PlayerRankText.text = (entry.Rank + 1).ToString();
            }
            PlayerNameText.text = StringHelpers.RemoveUGSSuffix(entry.PlayerName);
            PlayerScoreText.text = entry.Score.ToString();
        }
    }
}