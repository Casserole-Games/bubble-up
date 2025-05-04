using System.Threading.Tasks;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;

namespace Assets._Scripts.Leaderboard
{
    public interface ILeaderboardRetriever
    {
        Task<LeaderboardScoresPage> GetScoresAsync(string leaderboardId, GetScoresOptions options = null);
        Task<LeaderboardEntry> GetPlayerScoreAsync(string leaderboardId);
        Task<LeaderboardScores> GetPlayerRangeAsync(string leaderboardId, GetPlayerRangeOptions options = null);
    }
}
