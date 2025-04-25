using UnityEngine;

namespace Assets._Scripts.Leaderboard
{
    internal class MedalHolder : SingletonBehaviour<MedalHolder>
    {
        public Sprite goldMedal;
        public Sprite silverMedal;
        public Sprite bronzeMedal;

        public Sprite GetMedal(int rank)
        {
            return rank switch
            {
                0 => goldMedal,
                1 => silverMedal,
                2 => bronzeMedal,
                _ => null
            };
        }
    }
}
