﻿using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Leaderboards;
using UnityEngine;

namespace Assets._Scripts.Leaderboard
{
    internal class UGSLeaderboardSubmitter : ILeaderboardSubmitter
    {
        public async Task SubmitScore(string leaderboardID, int score)
        {
            try
            {
                var response = await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardID, score);
                Debug.Log("Score submitted: " + response.Score);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error while submitting score : " + e);
            }
        }
    }
}
