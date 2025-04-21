using Unity.Services.Core;
using UnityEngine;
using Unity.Services.Core.Environments;
using System.Threading.Tasks;
using Unity.Services.Leaderboards.Models;
using Unity.Services.Leaderboards;
using UnityEngine.SocialPlatforms.Impl;
using System;
using System.ComponentModel;
using Mono.Cecil.Cil;
using Unity.VisualScripting;

namespace Assets._Scripts.Leaderboard
{


    public class LeaderboardManager : SingletonBehaviour<LeaderboardManager>
    {
        public bool UseMock = false;
        
        private const string k_leaderboardID = "global-leaderboard";

        private ILeaderboardRetriever leaderboardRetriever;
        private ILeaderboardSubmitter leaderboardSubmitter;

        [Header("MockData")]

        [SerializeField]
        private int mockScoreMin;

        [SerializeField]
        private int mockScoreMax;

        [SerializeField]
        private int mockCount;

        [SerializeField]
        private int mockPlayerScore;


        protected async override void Awake()
        {
            base.Awake();

            if (UseMock)
            {
                leaderboardRetriever = new MockLeaderboardRetriever(mockScoreMin, mockScoreMax, mockCount);
                leaderboardSubmitter = new MockLeaderboardSubmitter();
            }
            else
            {
                leaderboardRetriever = new UGSLeaderboardRetriever();
                leaderboardSubmitter = new UGSLeaderboardSubmitter();
                await UnityServices.InitializeAsync(new InitializationOptions()
                    .SetEnvironmentName("production"));
            }
        }

        public async Task<LeaderboardScoresPage> GetLeaderboardTop7()
        {
            try
            {
                GetScoresOptions options = new()
                {
                    Limit = 7,
                };

                LeaderboardScoresPage scoresResponse = await leaderboardRetriever.GetScoresAsync(k_leaderboardID, options);

                foreach (LeaderboardEntry entry in scoresResponse.Results)
                {
                    Debug.Log($"{entry.Rank + 1}: {entry.PlayerName ?? "Anonyme"} - {entry.Score}");
                }

                return scoresResponse;
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error while getting scores : " + e);
                return null;
            }
        }

        [ContextMenu("GetLeaderboardPotentialRank")]
        public void GetLeaderboardPotentialRankMock()
        {
            leaderboardRetriever = new MockLeaderboardRetriever(mockScoreMin, mockScoreMax, mockCount);
            GetLeaderboardPotentialRankAsync(mockPlayerScore);
        }

        public async Task<int> GetLeaderboardPotentialRankAsync(int playerScore)
        {
            int limit = 100;
            int offset = 0;

            try
            {
                LeaderboardScoresPage response = null;

                bool hasMore = true;
                while (hasMore)
                {
                    response = await leaderboardRetriever.GetScoresAsync(k_leaderboardID, new GetScoresOptions
                    {
                        Offset = offset,
                        Limit = limit
                    });

                    int count = response.Results.Count;
                    hasMore = count == limit;

                    if (!hasMore || count == 0 || (int)response.Results[count - 1].Score <= playerScore)
                    {
                        break;
                    }

                    offset += count;
                }

                int insertIndex = -1;
                for (int i = response.Results.Count - 1; i >= 0; i--)
                {
                    if (playerScore <= (int)response.Results[i].Score)
                    {
                        insertIndex = i;
                        break;
                    }
                }

                var result = offset + insertIndex + 1;

                Debug.Log($"Player potential rank : {result}");

                return result;
            }
            catch (Exception e)
            {
                Debug.LogError("Error while getting scores : " + e);
                return -1;
            }
        }
    }
}