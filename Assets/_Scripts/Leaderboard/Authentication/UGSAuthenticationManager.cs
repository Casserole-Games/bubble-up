using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Leaderboards;

namespace Assets._Scripts.Leaderboard.Authentication
{
    internal class UGSAuthenticationManager : IAuthenticationManager
    {
        public string PlayerId => GetPlayerID();
        public string PlayerName => GetPlayerName();
        public bool IsSignedIn => AuthenticationService.Instance.IsSignedIn;


        private string GetPlayerID()
        {
            if (AuthenticationService.Instance.IsSignedIn)
            {
                return AuthenticationService.Instance.PlayerId;
            }
            else
            {
                return string.Empty;
            }
        }

       private string GetPlayerName()
        {
            if (AuthenticationService.Instance.IsSignedIn)
            {
                return AuthenticationService.Instance.PlayerName;
            }
            else
            {
                return string.Empty;
            }
        }

        public Task SignInAnonymouslyAsync()
        {
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                return AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        public Task UpdatePlayerNameAsync(string name)
        {
            return AuthenticationService.Instance.UpdatePlayerNameAsync(name);
        }
    }
}
