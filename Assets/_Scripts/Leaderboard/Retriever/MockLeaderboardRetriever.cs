using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace Assets._Scripts.Leaderboard
{
    internal class MockLeaderboardRetriever : ILeaderboardRetriever
    {
        private int mockScoreMin;
        private int mockScoreMax;
        private int mockCount;

        public MockLeaderboardRetriever(int mockScoreMin, int mockScoreMax, int mockCount)
        {
            this.mockScoreMin = mockScoreMin;
            this.mockScoreMax = mockScoreMax;
            this.mockCount = mockCount;
        }

        public Task<LeaderboardScoresPage> GetScoresAsync(string leaderboardId, GetScoresOptions options = null)
        {
            List<LeaderboardEntry> entries = GetEntries(mockScoreMin, mockScoreMax, mockCount);

            int firstElementIndex = options?.Offset ?? 0;
            int count = Math.Min((options?.Limit ?? 0), entries.Count);

            var entriesToReturn = entries.Skip(firstElementIndex).Take(count).ToList();

            return Task.FromResult(new LeaderboardScoresPage(options?.Offset ?? 0, options?.Limit ?? 0, entries.Count, entriesToReturn));
        }

        private List<LeaderboardEntry> GetEntries(int scoreMin, int scoreMax, int count)
        {
            List<LeaderboardEntry> result = new();

            if (count == 0)
            {
                return result;
            }
            else if (count == 1)
            {
                result.Add(new LeaderboardEntry("Player0", "Player Name 0", 0, scoreMin));
                return result;
            }

            float step = (float)(scoreMax - scoreMin) / (count - 1);

            for (int i = 0; i < count; i++)
            {
                string playerId = $"PlayerId{i}";
                string playerName = $"PlayerName{i}";
                int rank = i;
                int score = Mathf.RoundToInt(scoreMax - step * i);

                var entry = new LeaderboardEntry
                (
                    playerId,
                    playerName,
                    rank,
                    score
                );

                result.Add(entry);
            }
            return result.OrderByDescending(x => x.Score).ToList();
        }
    }
}
