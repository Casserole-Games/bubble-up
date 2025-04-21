using System.Threading.Tasks;
using Unity.Services.Leaderboards.Models;
using Unity.Services.Leaderboards;

namespace Assets._Scripts.Leaderboard
{
    internal class UGSLeaderboardRetriever : ILeaderboardRetriever
    {
        public Task<LeaderboardScoresPage> GetScoresAsync(string leaderboardId, GetScoresOptions options = null)
        {
            return LeaderboardsService.Instance.GetScoresAsync(leaderboardId, options);
        }
    }
}
