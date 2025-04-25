using System;
using TMPro;
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

        internal LeaderboardEntry entry;

        internal void SetEntry(LeaderboardEntry entry)
        {
            this.entry = entry;
        }

        public virtual void UpdateUI()
        {
            if (entry.Rank < 3)
            {
                DuckMedal.enabled = true;
                PlayerRankText.enabled = false;
                DuckMedal.sprite = MedalHolder.Instance.GetMedal(entry.Rank);
            }
            else
            {
                DuckMedal.enabled = false;
                PlayerRankText.enabled = true;
                PlayerRankText.text = (entry.Rank + 1).ToString();
            }
            PlayerNameText.text = entry.PlayerName;
            PlayerScoreText.text = entry.Score.ToString();
        }
    }
}