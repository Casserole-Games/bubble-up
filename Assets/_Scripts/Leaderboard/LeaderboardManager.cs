using Unity.Services.Core;
using UnityEngine;
using Unity.Services.Core.Environments;
using System.Threading.Tasks;
using Unity.Services.Leaderboards.Models;
using Unity.Services.Leaderboards;
using System;
using Unity.Services.Authentication;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Scripts.Leaderboard
{
    public class LeaderboardManager : SingletonBehaviour<LeaderboardManager>
    {
        private const string k_leaderboardID = "global-leaderboard";

        private ILeaderboardRetriever leaderboardRetriever;
        private ILeaderboardSubmitter leaderboardSubmitter;

        public event Action OnLeaderboardUpdated;

        [Header("MockData")]

        [SerializeField] private bool useMock = false;
        [SerializeField] private int mockScoreMin;
        [SerializeField] private int mockScoreMax;
        [SerializeField] private int mockCount;
        [SerializeField] private int mockPlayerScoreToCheck;

        [Header("Force Player Entry with PlayerID")]

        [SerializeField] private string mockPlayerID;
        [SerializeField] private int mockPlayerScore;


        protected async override void Awake()
        {
            base.Awake();
            SetupLeaderboardDependencies();
            try
            {
                await UnityServices.InitializeAsync(new InitializationOptions()
                        .SetEnvironmentName("production"));

                await AuthenticationManager.SignInAnonymouslyIfNotSignedIn();
                Debug.Log("Player ID : " + AuthenticationService.Instance.PlayerId);

            }
            catch (Exception e)
            {
                Debug.LogError("Error while initializing Unity Services : " + e);
            }
        }

        public async void Start()
        {
        }

        private void SetupLeaderboardDependencies()
        {
            if (useMock)
            {
                int? mockPlayerScore = this.mockPlayerScore == -1 ? null : this.mockPlayerScore;

                leaderboardRetriever = new MockLeaderboardRetriever(mockScoreMin, mockScoreMax, mockCount, mockPlayerID, mockPlayerScore);
                leaderboardSubmitter = new MockLeaderboardSubmitter();
            }
            else
            {
                leaderboardRetriever = new UGSLeaderboardRetriever();
                leaderboardSubmitter = new UGSLeaderboardSubmitter();
            }
        }
        public async Task<List<LeaderboardEntry>> GetLeaderboardTop(int limit)
        {
            try
            {
                GetScoresOptions options = new()
                {
                    Limit = limit,
                };

                LeaderboardScoresPage scoresResponse = await leaderboardRetriever.GetScoresAsync(k_leaderboardID, options);

                foreach (LeaderboardEntry entry in scoresResponse.Results)
                {
                    Debug.Log($"{entry.Rank + 1}: {entry.PlayerName ?? "Anonyme"} - {entry.Score}");
                }

                return scoresResponse.Results;
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
            leaderboardRetriever = new MockLeaderboardRetriever(mockScoreMin, mockScoreMax, mockCount, mockPlayerID, mockPlayerScore);
            _ = GetLeaderboardPotentialRankAsync(mockPlayerScoreToCheck).Result;
        }

        public async Task<LeaderboardPotentialRankResponse> GetLeaderboardPotentialRankAsync(int playerScore)
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

                    //iterate through the page to check if the payers id is already registered
                    for (int i = 0; i < count; i++)
                    {
                        if (response.Results[i].PlayerId == AuthenticationService.Instance.PlayerId)
                        {
                            if (response.Results[i].Score >= playerScore)
                            {
                                Debug.Log($"Player already registered with a higher score : {response.Results[i].Score}");
                                return LeaderboardPotentialRankResponse.Failure(LeaderboardPotentialRankReponseErrorType.HigherScoreAlreadyExists);
                            }
                        }
                    }

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

                return LeaderboardPotentialRankResponse.Success(result);
            }
            catch (Exception e)
            {
                Debug.LogError("Error while getting scores : " + e);
                return LeaderboardPotentialRankResponse.Failure(LeaderboardPotentialRankReponseErrorType.NetworkError);
            }
        }

        public async Task<List<LeaderboardEntry>> GetPlayerNeighbours()
        {
            try
            {
                await AuthenticationManager.SignInAnonymouslyIfNotSignedIn();
                GetPlayerRangeOptions options = new()
                {
                    RangeLimit = 2,
                };

                LeaderboardScores playerRangeResponse = await leaderboardRetriever.GetPlayerRangeAsync(k_leaderboardID, options);

                foreach (LeaderboardEntry entry in playerRangeResponse.Results)
                {
                    Debug.Log($"{entry.Rank + 1}: {entry.PlayerName ?? "Anonyme"} - {entry.Score}");
                }

                return playerRangeResponse.Results.Take(4).ToList();
            }
            catch (Exception e)
            {
                Debug.LogError("Error while getting scores : " + e);
                return null;
            }
        }

        public async Task<LeaderboardEntry> GetPlayerEntry()
        {
            await AuthenticationManager.SignInAnonymouslyIfNotSignedIn();
            return await leaderboardRetriever.GetPlayerScoreAsync(k_leaderboardID);
        }

        public async Task SubmitScore(int score)
        {
            try
            {
                await leaderboardSubmitter.SubmitScore(k_leaderboardID, score);
                OnLeaderboardUpdated?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError("Error while submitting score : " + e);
            }
        }

        public async Task UpdatePlayerName(string name)
        {
            await AuthenticationManager.SignInAnonymouslyIfNotSignedIn();
            await AuthenticationService.Instance.UpdatePlayerNameAsync(name);

            LeaderboardEntry playerEntry = await leaderboardRetriever.GetPlayerScoreAsync(k_leaderboardID);
            await leaderboardSubmitter.SubmitScore(k_leaderboardID, (int)playerEntry.Score);
        }
    }
}