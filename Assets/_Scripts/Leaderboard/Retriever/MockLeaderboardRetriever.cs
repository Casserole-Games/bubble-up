using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets._Scripts.Leaderboard
{
    internal class MockLeaderboardRetriever : ILeaderboardRetriever
    {
        private readonly int mockScoreMin;
        private readonly int mockScoreMax;
        private readonly int mockCount;
        private readonly string forcedPlayerID;
        private readonly int? forcedPlayerScore;

        private readonly List<LeaderboardEntry> entries = new();

        public MockLeaderboardRetriever(int mockScoreMin, int mockScoreMax, int mockCount, string forcedPlayerID, int? forcedPlayerScore)
        {
            this.mockScoreMin = mockScoreMin;
            this.mockScoreMax = mockScoreMax;
            this.mockCount = mockCount;
            this.forcedPlayerID = forcedPlayerID;
            this.forcedPlayerScore = forcedPlayerScore;
        }

        public Task<LeaderboardScoresPage> GetScoresAsync(string leaderboardId, GetScoresOptions options = null)
        {
            SetEntries(mockScoreMin, mockScoreMax, mockCount, forcedPlayerID, forcedPlayerScore);

            int firstElementIndex = options?.Offset ?? 0;
            int count = Math.Min((options?.Limit ?? 0), entries.Count);

            var entriesToReturn = entries.Skip(firstElementIndex).Take(count).ToList();

            return Task.FromResult(new LeaderboardScoresPage(options?.Offset ?? 0, options?.Limit ?? 0, entries.Count, entriesToReturn));
        }

        public Task<LeaderboardEntry> GetPlayerScoreAsync(string leaderboardId)
        {
            SetEntries(mockScoreMin, mockScoreMax, mockCount, forcedPlayerID, forcedPlayerScore);
            var playerScore = entries.FirstOrDefault(t => t.PlayerId == forcedPlayerID);
            return Task.FromResult(playerScore);
        }

        public Task<LeaderboardScores> GetPlayerRangeAsync(string leaderboardId, GetPlayerRangeOptions options = null)
        {
            SetEntries(mockScoreMin, mockScoreMax, mockCount, forcedPlayerID, forcedPlayerScore);

            int playerRank = GetPlayerScoreAsync(leaderboardId).Result.Rank;

            int startIndexRaw = playerRank - options?.RangeLimit ?? 5;
            int startIndex = Math.Max(0, startIndexRaw);

            int endIndexRaw = playerRank + options?.RangeLimit ?? 5;
            int endIndex = Math.Min(entries.Count - 1, endIndexRaw);

            List<LeaderboardEntry> results = new();

            for (int i = startIndex; i <= endIndex; i++)
            {
                results.Add(entries[i]);
            }

            return Task.FromResult(new LeaderboardScores(results));
        }

        private void SetEntries(int scoreMin, int scoreMax, int count, string forcedPlayerID = null, int? forcedPlayerScore = null)
        {
            entries.Clear();
            bool mustHardInsertPlayer = false;

            if (!string.IsNullOrEmpty(forcedPlayerID) && forcedPlayerScore != null)
            {
                mustHardInsertPlayer = true;
            }

            if (count == 0)
            {
                return;
            }
            else if (count == 1)
            {
                entries.Add(new LeaderboardEntry("Player0", "Player0", 0, scoreMin));
                return;
            }

            float step = (float)(scoreMax - scoreMin) / (count - 1);

            for (int i = 0; i < count; i++)
            {
                int score = Mathf.RoundToInt(scoreMax - step * i);
                if (mustHardInsertPlayer && score < forcedPlayerScore.Value)
                {
                    entries.Add(new LeaderboardEntry(forcedPlayerID, "Player0", i++, forcedPlayerScore.Value));
                    mustHardInsertPlayer = false;
                }

                string playerId = $"PlayerId{i}";
                string playerName = $"Player{i}";
                int rank = i;

                var entry = new LeaderboardEntry
                (
                    playerId,
                    playerName,
                    rank,
                    score
                );

                entries.Add(entry);
            }
            entries.OrderByDescending(x => x.Score).ToList();
        }
    }
}
