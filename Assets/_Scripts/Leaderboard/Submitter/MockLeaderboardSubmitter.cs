using System;
using System.Threading.Tasks;

namespace Assets._Scripts.Leaderboard
{
    internal class MockLeaderboardSubmitter : ILeaderboardSubmitter
    {
        public Task SubmitScore(string leaderboardID, string playerName, int score)
        {
            throw new NotImplementedException();
        }
    }
}
