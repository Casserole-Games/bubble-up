using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets._Scripts.Leaderboard.Authentication
{
    public interface IAuthenticationManager
    {
        string PlayerId { get; }
        bool IsSignedIn { get; }
        string PlayerName { get; }

        Task SignInAnonymouslyAsync();
        Task UpdatePlayerNameAsync(string name);
    }
}
