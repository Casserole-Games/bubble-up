using System;
using TMPro;
using Unity.Services.Leaderboards.Models;
using UnityEngine;

namespace Assets._Scripts.Leaderboard
{
    internal class PlayerScoreUI : MonoBehaviour
    {
        public TMP_Text PlayerRankText;
        public TMP_Text PlayerNameText;
        public TMP_Text PlayerScoreText;

        internal LeaderboardEntry entry;

        internal void SetPlayerName(string playerName)
        {
            throw new NotImplementedException();
        }

        internal void SetPlayerScore(double score)
        {
            throw new NotImplementedException();
        }

        internal void SetEntry(LeaderboardEntry entry)
        {
            this.entry = entry;
        }

        public virtual void UpdateUI()
        {
            if (PlayerRankText != null)
                PlayerRankText.text = (entry.Rank + 1).ToString();
            PlayerNameText.text = entry.PlayerName;
            PlayerScoreText.text = entry.Score.ToString();
        }
    }
}