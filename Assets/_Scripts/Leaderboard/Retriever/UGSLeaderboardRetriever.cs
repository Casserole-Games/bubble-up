using System.Threading.Tasks;
using Unity.Services.Leaderboards.Models;
using Unity.Services.Leaderboards;
using System.Collections.Generic;

namespace Assets._Scripts.Leaderboard
{
    internal class UGSLeaderboardRetriever : ILeaderboardRetriever
    {
        public Task<LeaderboardScoresPage> GetScoresAsync(string leaderboardId, GetScoresOptions options = null)
        {
            return LeaderboardsService.Instance.GetScoresAsync(leaderboardId, options);
        }

        public Task<LeaderboardEntry> GetPlayerScoreAsync(string leaderboardId)
        {
            return LeaderboardsService.Instance.GetPlayerScoreAsync(leaderboardId);
        }

        public Task<LeaderboardScores> GetPlayerRangeAsync(string leaderboardId, GetPlayerRangeOptions options = null)
        {
            return LeaderboardsService.Instance.GetPlayerRangeAsync(leaderboardId, options);
        }
    }
}
