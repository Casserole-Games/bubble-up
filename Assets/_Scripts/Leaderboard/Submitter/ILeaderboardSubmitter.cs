using System.Threading.Tasks;

namespace Assets._Scripts.Leaderboard
{
    internal interface ILeaderboardSubmitter
    {
        public Task SubmitScore(string leaderboardID, int score);
    }
}
