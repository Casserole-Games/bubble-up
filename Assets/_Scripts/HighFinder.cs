using System;
using UnityEngine;

namespace Assets._Scripts
{
    public class HighFinder : SingletonBehaviour<HighFinder>
    {
        public static event Action OnMaxHeightAchieved;
        public static event Action OnMinHeightAchieved;

        public bool MaxHeightAchieved = false;
        public bool MinHeightAchieved = false;

        private bool _wereGreenArrowsShown = false;

        public GameObject BubbleContainer;
        public GameObject LocalHighGreenLine;
        public GameObject LocalHighPinkLine;

        internal float localHighestY = 0;
        private float _startGreenLinePosition;
        private float _maxHeight;
        private int _maxScore; // Maximum score when reaching 89% of the screen height

        void Start()
        {
            _startGreenLinePosition = LocalHighGreenLine.transform.position.y;
            _maxScore = GameParameters.Instance.MaxScoreInScreen;
        }

        void FixedUpdate()
        {
            if ((GameManager.Instance.GameState != GameState.Phase1) && (GameManager.Instance.GameState != GameState.Phase2)) return;

            Calculate();
        }

        public void Calculate()
        {
            var lastLocalHighestY = localHighestY;
            localHighestY = 0;
            _maxHeight = Screen.height * 0.89f;

            foreach (Transform child in BubbleContainer.transform)
            {
                Bubble bubble = child.GetComponent<Bubble>();
                if (bubble == null) continue;
                if (!bubble.alreadyCollided) continue;

                // Get the top position of the bubble in screen coordinates
                float screenPos = Camera.main.WorldToScreenPoint(child.position + child.localScale * 0.7f / 2).y;

                if (screenPos > localHighestY)
                {
                    localHighestY = Mathf.Min(screenPos, _maxHeight); // Limit to max height
                }
            }

            // Log warning if height remains zero
            if (localHighestY == 0)
            {
                Debug.LogWarning("localHighestY = 0, check if bubbles exist and are measured correctly!");
            }

            // Compute score based on reaching 90% of the screen height
            GameManager.Instance.CurrentScore = Mathf.RoundToInt((localHighestY / _maxHeight) * _maxScore);
            Debug.Log($"Score: {GameManager.Instance.CurrentScore}, localHighestY: {localHighestY}, maxHeight: {_maxHeight}");

            Vector3 localLinePos = Camera.main.ScreenToWorldPoint(new Vector3(0, localHighestY, 0));
            localLinePos.x = 0;
            localLinePos.z = 0;

            if (GameManager.Instance.GameState == GameState.Phase1)
            {
                if (!_wereGreenArrowsShown && localLinePos.y > _startGreenLinePosition)
                {
                    AnimationManager.Instance.PlayGreenArrows();
                    _wereGreenArrowsShown = true;
                }
                LocalHighGreenLine.transform.position = localLinePos;

                if (!MaxHeightAchieved && GameManager.Instance.CurrentScore >= GameParameters.Instance.Phase1MaxHeight && AreBubblesSettled())
                {
                    MaxHeightAchieved = true;
                    OnMaxHeightAchieved?.Invoke();
                }
            }
            else if (GameManager.Instance.GameState == GameState.Phase2)
            {
                LocalHighPinkLine.transform.position = localLinePos;

                if (LocalHighPinkLine.transform.position.y < GameParameters.Instance.MinHeightBeforeBomb && AreBubblesSettled() && !BubbleSpawner.Instance.SpawnBomb)
                {
                    BubbleSpawner.Instance.SpawnBomb = true;
                }

                if (!MinHeightAchieved && LocalHighPinkLine.transform.position.y <= GameParameters.Instance.Phase2MinHeight && AreBubblesSettled())
                {
                    MinHeightAchieved = true;
                    OnMinHeightAchieved?.Invoke();
                }
            }
        }

        public void ActivatePinkLine()
        {
            LocalHighPinkLine.SetActive(true);
        }

        public bool AreBubblesSettled()
        {
            foreach (Transform child in BubbleContainer.transform)
            {
                Bubble bubble = child.GetComponent<Bubble>();
                if (bubble != null && !bubble.IsSettled())
                {
                    return false;
                }
            }
            return true;
        }
    }
}
