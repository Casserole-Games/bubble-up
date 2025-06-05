using Assets._Scripts.Leaderboard.DependenciesContainer;
using System.Threading.Tasks;

namespace Assets._Scripts.Leaderboard.Authentication
{
    internal class MockAuthenticationManager : IAuthenticationManager
    {
        public string PlayerId => _playerId;
        private readonly string _playerId;

        public bool IsSignedIn => _isSignedIn;
        private bool _isSignedIn;

        public string PlayerName => _playerName;
        private string _playerName;

        public MockAuthenticationManager(RuntimeConfig config)
        {
            _playerId = config.PlayerId;
            _playerName = config.PlayerName;
            _isSignedIn = false;
        }

        public Task SignInAnonymouslyAsync()
        {
            _isSignedIn = true;
            return Task.CompletedTask;
        }

        public Task UpdatePlayerNameAsync(string name)
        {
            _playerName = name;
            return Task.CompletedTask;
        }
    }
}
