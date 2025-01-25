using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Scripts
{
    internal class GameParameters : MonoBehaviour
    {
        // Bubble Canon
        public float BubbleSpawnerSpeed = 1.0f;
        public float InitialBubbleSize = 1.0f;

        //Spikes
        public int SpikeCount = 6;
        public float MinSpikeLength = 0.3f;
        public float MaxSpikeLength = 0.6f;
        public int MinSpikeCountOnEachSide = 2;
        public float SoapFlowRate = 0.1f;
        public int BubbleColorsCount = 5;
        public float BubbleInflationRate = 0.1f;

        public static GameParameters Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

    }
}
