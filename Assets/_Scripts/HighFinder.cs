using System;
using UnityEngine;

namespace Assets._Scripts
{
    public class HighFinder : MonoBehaviour
    {
        public static HighFinder Instance { get; private set; }

        public bool Phase1 = true;

        public GameObject BubbleContainer;
        public GameObject GlobalHighLine;
        public GameObject LocalHighGreenLine;
        public GameObject LocalHighPinkLine;

        internal float globalHighestY = 0;
        internal float localHighestY = 0;

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        void FixedUpdate()
        {
            localHighestY = 0;
            foreach (Transform child in BubbleContainer.transform)
            {
                Bubble bubble = child.GetComponent<Bubble>();
                if (bubble == null) continue;

                if (!bubble.alreadyCollided) continue;

                // add half of the scale to the position to get the top of the bubble -> to get the top of the bubble
                float screenPos = Camera.main.WorldToScreenPoint(child.position + child.localScale * 0.7f / 2).y;
                if (screenPos > globalHighestY)
                {
                    globalHighestY = Math.Min(screenPos, Screen.height);
                }
                if (screenPos > localHighestY)
                {
                    localHighestY = Math.Min(screenPos, Screen.height);
                }
            }

            Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector2(0, 0));
            Vector3 localLinePos = Camera.main.ScreenToWorldPoint(new Vector3(0, localHighestY, 0));
            localLinePos.x = 0;
            localLinePos.z = 0;
            if (Phase1)
            {
                LocalHighGreenLine.transform.position = localLinePos;
            }
            else
            {
                LocalHighPinkLine.transform.position = localLinePos;
            }

            //UIManager.Instance.SetBestScore((int)globalHighestY);
            GameManager.Instance.SetCurrentScore((int)localHighestY);
        }
    }
}
