using Assets._Scripts.Leaderboard.Authentication;
using System;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;

namespace Assets._Scripts.Leaderboard.DependenciesContainer
{
    public class DependencyContainer
    {
        public static IAuthenticationManager AuthenticationManager { get; private set; }
        public static ILeaderboardSubmitter LeaderboardSubmitter { get; private set; }
        public static ILeaderboardRetriever LeaderboardRetriever { get; private set; }

        private static bool isInitialized;
        private static RuntimeConfig config;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            if (isInitialized) return;
            isInitialized = true;

            config = Resources.Load<RuntimeConfig>("RuntimeConfig");

            if (config && config.UseMocks)
            {
                var leaderboardMockRepository = new LeaderboardMockRepository();
                AuthenticationManager = new MockAuthenticationManager(config);
                LeaderboardSubmitter = new MockLeaderboardSubmitter(leaderboardMockRepository);
                LeaderboardRetriever = new MockLeaderboardRetriever(config, leaderboardMockRepository);
            }
            else
            {
                AuthenticationManager = new UGSAuthenticationManager();
                LeaderboardSubmitter = new UGSLeaderboardSubmitter();
                LeaderboardRetriever = new UGSLeaderboardRetriever();
            }
        }

        public static async Task InitializeGameServices()
        {
            if (config.UseMocks) return;

            try
            {
                await UnityServices.InitializeAsync(new InitializationOptions()
                    .SetEnvironmentName("production"));
            }
            catch (Exception e)
            {
                Debug.LogError("Error while initializing Unity Services : " + e);
            }
        }
    }
}
