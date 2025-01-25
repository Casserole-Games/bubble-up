using System;
using UnityEngine;

namespace Assets._Scripts
{
    public class HighFinder : MonoBehaviour
    {
        public static HighFinder Instance { get; private set; }

        public GameObject BubbleContainer;
        public GameObject GlobalHighLine;
        public GameObject LocalHighLine;

        internal float globalHighestY = 0;

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        void FixedUpdate()
        {
            float localHighestY = 0;
            foreach (Transform child in BubbleContainer.transform)
            {
                Bubble bubble = child.GetComponent<Bubble>();
                if (bubble == null) continue;

                if (!bubble.alreadyCollided) continue;

                // add half of the scale to the position to get the top of the bubble -> to get the top of the bubble
                float screenPos = Camera.main.WorldToScreenPoint(child.position + child.localScale / 2).y;
                if (screenPos > globalHighestY)
                {
                    globalHighestY = screenPos;
                }
                if (screenPos > localHighestY)
                {
                    localHighestY = screenPos;
                }
            }

            Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector2(0, 0));
            Vector3 globalLinePos = Camera.main.ScreenToWorldPoint(new Vector3(0, globalHighestY, 0));
            globalLinePos.x = 0;
            globalLinePos.z = 0;
            GlobalHighLine.transform.position = globalLinePos;
            GlobalHighLine.transform.localScale = new Vector3(Math.Abs(bottomLeft.x) * 2 + 1, 0.05f, 1);
            Vector3 localLinePos = Camera.main.ScreenToWorldPoint(new Vector3(0, localHighestY, 0));
            localLinePos.x = 0;
            localLinePos.z = 0;
            LocalHighLine.transform.position = localLinePos;
            LocalHighLine.transform.localScale = new Vector3(Math.Abs(bottomLeft.x) * 2 + 1, 0.05f, 1);

            UIManager.Instance.SetBestScore((int)globalHighestY);
            UIManager.Instance.SetCurrentScore((int)localHighestY);

        }
    }
}
