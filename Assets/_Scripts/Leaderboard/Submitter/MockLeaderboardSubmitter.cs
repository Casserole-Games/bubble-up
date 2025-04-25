using System;
using System.Threading.Tasks;

namespace Assets._Scripts.Leaderboard
{
    internal class MockLeaderboardSubmitter : ILeaderboardSubmitter
    {
        public Task SubmitScore(string leaderboardID, int score)
        {
            throw new NotImplementedException();
        }
    }
}
