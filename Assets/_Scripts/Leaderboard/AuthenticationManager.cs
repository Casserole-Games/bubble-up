using System.Threading.Tasks;
using Unity.Services.Authentication;

namespace Assets._Scripts.Leaderboard
{
    internal class AuthenticationManager
    {
        public static async Task SignInAnonymouslyIfNotSignedIn()
        {
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
        }
    }
}
