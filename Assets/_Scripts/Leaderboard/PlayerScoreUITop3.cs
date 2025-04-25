
using Unity.Services.Leaderboards.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Scripts.Leaderboard
{
    internal class PlayerScoreUITop3 : PlayerScoreUI
    {
        public Image DuckMedal;

        public override void UpdateUI()
        {
            base.UpdateUI();
            DuckMedal.sprite = MedalHolder.Instance.GetMedal(entry.Rank);
        }
    }
}