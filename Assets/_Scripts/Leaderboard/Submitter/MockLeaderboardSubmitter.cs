using Assets._Scripts.Leaderboard.DependenciesContainer;
using System;
using System.Threading.Tasks;

namespace Assets._Scripts.Leaderboard
{
    internal class MockLeaderboardSubmitter : ILeaderboardSubmitter
    {
        private readonly LeaderboardMockRepository leaderboardMockRepository;

        public MockLeaderboardSubmitter(LeaderboardMockRepository leaderboardMockRepository)
        {
            this.leaderboardMockRepository = leaderboardMockRepository;
        }

        public Task SubmitScore(string leaderboardID, int score)
        {
            leaderboardMockRepository.AddMockEntry(DependencyContainer.AuthenticationManager.PlayerId, DependencyContainer.AuthenticationManager.PlayerName, score);
            return Task.CompletedTask;
        }
    }
}
