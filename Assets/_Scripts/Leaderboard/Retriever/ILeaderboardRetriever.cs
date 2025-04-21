using System.Threading.Tasks;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;

namespace Assets._Scripts.Leaderboard
{
    internal interface ILeaderboardRetriever
    {
        Task<LeaderboardScoresPage> GetScoresAsync(string leaderboardId, GetScoresOptions options = null);

    }
}
