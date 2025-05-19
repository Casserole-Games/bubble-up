using Assets._Scripts.Leaderboard.DependenciesContainer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;

namespace Assets._Scripts.Leaderboard
{
    internal class MockLeaderboardRetriever : ILeaderboardRetriever
    {
        private readonly LeaderboardMockRepository leaderboardMockRepository;
        private readonly RuntimeConfig config;

        public MockLeaderboardRetriever(RuntimeConfig config, LeaderboardMockRepository leaderboardMockRepository)
        {
            this.config = config;
            this.leaderboardMockRepository = leaderboardMockRepository;
            leaderboardMockRepository.SetEntries(config.MockScoreMin, config.MockScoreMax, config.MockCount, config.PlayerScore);
        }

        public Task<LeaderboardScoresPage> GetScoresAsync(string leaderboardId, GetScoresOptions options = null)
        {
            int firstElementIndex = options?.Offset ?? 0;
            int count = Math.Min((options?.Limit ?? 0), leaderboardMockRepository.MockLeaderboardEntries.Count);

            var entriesToReturn = leaderboardMockRepository.MockLeaderboardEntries.Skip(firstElementIndex).Take(count).ToList();

            return Task.FromResult(new LeaderboardScoresPage(options?.Offset ?? 0, options?.Limit ?? 0, leaderboardMockRepository.MockLeaderboardEntries.Count, entriesToReturn));
        }

        public Task<LeaderboardEntry> GetPlayerScoreAsync(string leaderboardId)
        {
            var playerScore = leaderboardMockRepository.MockLeaderboardEntries.FirstOrDefault(t => t.PlayerId == config.PlayerId);
            return Task.FromResult(playerScore);
        }

        public Task<LeaderboardScores> GetPlayerRangeAsync(string leaderboardId, GetPlayerRangeOptions options = null)
        {
            int playerRank = GetPlayerScoreAsync(leaderboardId).Result.Rank;

            int startIndexRaw = playerRank - options?.RangeLimit ?? 5;
            int startIndex = Math.Max(0, startIndexRaw);

            int endIndexRaw = playerRank + options?.RangeLimit ?? 5;
            int endIndex = Math.Min(leaderboardMockRepository.MockLeaderboardEntries.Count - 1, endIndexRaw);

            List<LeaderboardEntry> results = new();

            for (int i = startIndex; i <= endIndex; i++)
            {
                results.Add(leaderboardMockRepository.MockLeaderboardEntries[i]);
            }

            return Task.FromResult(new LeaderboardScores(results));
        }
    }
}
