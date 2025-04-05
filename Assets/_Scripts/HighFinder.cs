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
        public GameObject MaxHeight;

        internal float localHighestY = 0;
        private float _startGreenLinePosition;
        private float _maxHeight;
        private int _maxScore;

        void Start()
        {
            _startGreenLinePosition = LocalHighGreenLine.transform.position.y;
            _maxScore = GameParameters.Instance.MaxScoreInScreen;
            _maxHeight = MaxHeight.transform.position.y;
        }

        void FixedUpdate()
        {
            if ((GameManager.Instance.GameState != GameState.Phase1) && (GameManager.Instance.GameState != GameState.Phase2))
                return;

            Calculate();
        }

        public void Calculate()
        {
            localHighestY = _startGreenLinePosition;

            foreach (Transform child in BubbleContainer.transform)
            {
                Bubble bubble = child.GetComponent<Bubble>();
                if (bubble == null || !bubble.alreadyCollided)
                    continue;

                // Get the top position of the bubble in world coordinates
                float bubbleTopY = child.position.y + (child.localScale.y * 0.7f / 2);

                if (bubbleTopY > localHighestY)
                {
                    localHighestY = bubbleTopY;
                }
            }

            localHighestY = Mathf.Clamp(localHighestY, _startGreenLinePosition, _maxHeight);

            float progress = (localHighestY - _startGreenLinePosition) / (_maxHeight - _startGreenLinePosition);
            GameManager.Instance.CurrentScore = Mathf.RoundToInt(progress * _maxScore);

            Vector3 localLinePos = new Vector3(0, localHighestY, 0);

            if (GameManager.Instance.GameState == GameState.Phase1)
            {
                if (!_wereGreenArrowsShown && localHighestY > _startGreenLinePosition)
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

                if (localHighestY < GameParameters.Instance.MinHeightBeforeBomb && AreBubblesSettled() && !BubbleSpawner.Instance.SpawnBomb)
                {
                    BubbleSpawner.Instance.SpawnBomb = true;
                }

                if (!MinHeightAchieved && localHighestY <= GameParameters.Instance.Phase2MinHeight && AreBubblesSettled())
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
