using System.Collections.Generic;
using UnityEngine;

namespace Assets._Scripts
{
    internal class GameParameters : SingletonBehaviour<GameParameters>
    {
        [Header("Bubbles")]
        public float GunLatSpeed = 1.0f;
        public float InitialBubbleSize = 1.0f;
        public float MaximalBubbleSize = 8.0f;
        public int BubbleColorsCount = 5;
        public float BubbleInflationRate = 0.1f;
        public float BubbleFlowSpeed = 5.0f;
        public float BubbleMinimumSoapConsumption = 0.25f;

        [Header("Soap")]
        public float SoapFlowRate = 0.1f;
        public float MaxSoapAmount = 100;
        public float StartSoapAmount = 100;
        public float Phase2AdditionalSoap = 10;
        public float BonusSoap = 15;

        [Header("Segues")]
        public float DurationBeforeShowingTextBubble = 1.0f;
        public float DurationBeforeDuckTurnsAround = 1.0f;
        public float DurationBeforeSkipping = 1.0f;
        public float TutorialTime = 3f;
        public float DurationOfSoapRefill = 1f;

        [Header("Score")]
        public int MaxScoreInScreen = 1000;
        public int MinScoreBeforeRefill = 500;

        [Header("Volume")]
        public float InflatingVolume = 1f;
        public float MergeVolume = 1f;
        public float PopVolume = 1f;
        public float ScoreVolume = 1f;
        public float WinVolume = 1f;
        public float PickupVolume = 1f;
        public float DuckVolume = 1f;
        public float FuseVolume = 1f;
        public float BombVolume = 1f;
        public float UIClickVolume = 1f;

        [Header("Heights")]
        public float Phase1MaxHeight = 1000f;
        public float Phase2MinHeight = -4.3f;
        public float MinHeightBeforeBomb = -3.5f;

        [Header("Hold Hint")]
        public float ShortHoldThresholdSeconds = 0.25f;
        public float LongHoldThresholdSeconds = 0.5f;
        public float ShortHoldsToHint = 3f;
        public float ShortHoldsToRepeatHint = 5f;

        [Header("Reward Phrases")]
        public List<RewardPhraseEntry> RewardPhrases = new List<RewardPhraseEntry>();
        public ParticleSystem rewardPhraseParticles;
    }
}
