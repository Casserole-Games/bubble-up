using UnityEngine;

namespace Assets._Scripts.Leaderboard.DependenciesContainer
{
    [CreateAssetMenu(fileName = "RuntimeConfig", menuName = "Config/Runtime")]
    public class RuntimeConfig : ScriptableObject
    {
        public bool UseMocks = true;
        public string PlayerId = "MockPlayerId";
        public string PlayerName = "MockPlayer";
        public int MockScoreMin;
        public int MockScoreMax;
        public int MockCount;
        public int PlayerScore;
    }
}