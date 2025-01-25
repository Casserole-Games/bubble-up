using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Scripts
{
    internal class SpikeManager : MonoBehaviour
    {
        public GameObject SpikePrefab;
        [SerializeField] private float minSpikeLength = 0.3f;
        [SerializeField] private float maxSpikeLength = 0.6f;
        [SerializeField] private int spikeCount = 6;
        private float minGap;
        private float maxGap;

        private Vector3 bottomLeft;
        private Vector3 topRight;
        private float lastHeight = -5;

        private void Start()
        {
            bottomLeft = Camera.main.ScreenToWorldPoint(new Vector2(0, 0));
            topRight = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));

            var screenHeight = Math.Abs(topRight.y - bottomLeft.y);
            var avgGap = screenHeight / (spikeCount + 1);
            minGap = avgGap * 0.9f;
            maxGap = avgGap * 1.1f;

            for (int i = 0; i < spikeCount; i++)
            {
                var flip = UnityEngine.Random.Range(0, 2);
                if (flip == 0)
                {
                    CreateSpikeLeftWall();
                }
                else
                {
                    CreateSpikeRightWall();
                }
            }
        }

        public void CreateSpikeLeftWall()
        {
            GameObject spike = Instantiate(SpikePrefab);
            float newHeight = UnityEngine.Random.Range(minGap, maxGap) + lastHeight;
            spike.transform.position = new Vector3(bottomLeft.x, UnityEngine.Random.Range(minGap, maxGap) + lastHeight, 0);
            var currentRotation = spike.transform.rotation;
            currentRotation = Quaternion.Euler(0, 0, -90);
            spike.transform.rotation = currentRotation;

            var currentScale = spike.transform.localScale;
            currentScale.y = UnityEngine.Random.Range(minSpikeLength, maxSpikeLength);
            spike.transform.localScale = currentScale;
            lastHeight = newHeight;
        }

        public void CreateSpikeRightWall() {
            GameObject spike = Instantiate(SpikePrefab);
            float newHeight = UnityEngine.Random.Range(minGap, maxGap) + lastHeight;
            spike.transform.position = new Vector3(topRight.x, UnityEngine.Random.Range(minGap, maxGap) + lastHeight, 0);
            lastHeight = newHeight;

            var currentScale = spike.transform.localScale;
            currentScale.y = UnityEngine.Random.Range(minSpikeLength, maxSpikeLength);
            spike.transform.localScale = currentScale;
        }


    }
}
