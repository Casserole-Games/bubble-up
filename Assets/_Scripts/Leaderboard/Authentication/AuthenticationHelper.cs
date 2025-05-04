using Assets._Scripts.Leaderboard.DependenciesContainer;
using System.Threading.Tasks;

namespace Assets._Scripts.Leaderboard
{
    internal class AuthenticationHelper
    {
        public static async Task SignInAnonymouslyIfNotSignedIn()
        {
            if (!DependencyContainer.AuthenticationManager.IsSignedIn)
            {
                await DependencyContainer.AuthenticationManager.SignInAnonymouslyAsync();
            }
        }

        public static async Task<string> GetPlayerNameWithoutUGSSuffix()
        {
            await SignInAnonymouslyIfNotSignedIn();
            string playerName = DependencyContainer.AuthenticationManager.PlayerName;
            playerName = playerName.Substring(0, playerName.Length - 5);
            return playerName;
        }
    }
}
