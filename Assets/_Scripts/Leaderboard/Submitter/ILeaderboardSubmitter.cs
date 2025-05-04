using System.Threading.Tasks;

namespace Assets._Scripts.Leaderboard
{
    public interface ILeaderboardSubmitter
    {
        public Task SubmitScore(string leaderboardID, int score);
    }
}
