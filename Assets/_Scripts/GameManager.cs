using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets._Scripts
{
    public class GameManager : SingletonBehaviour<GameManager>
    {
        public static event Action<GameState> OnGameStateChanged;

        public GameState GameState 
        { 
            get { return _gameState; } 
            set { UpdateGameState(value); }
        }

        public int CurrentScore 
        {
            get { return _currentScore;  }
            set { SetCurrentScore(value);  }
        }

        private GameState _gameState;
        private int _currentScore;
        private int _phase1Score;

        private void OnEnable()
        {
            BubbleSpawner.OnEmptyTank += HandlePhaseEnd;
            HighFinder.OnMaxHeightAchieved += HandleOnMaxHeightAchieved;
        }

        private void OnDisable()
        {
            BubbleSpawner.OnEmptyTank -= HandlePhaseEnd;
            HighFinder.OnMaxHeightAchieved -= HandleOnMaxHeightAchieved;
        }

        // game entrance point
        private void Start()
        {
            UIManager.Instance.PlayCutsceneStart();
        }

        public IEnumerator PopAllBubbles()
        {
            List<Bubble> bubbles = GameObject.FindGameObjectsWithTag("Bubble").Select(t => t.GetComponent<Bubble>()).ToList();
            bubbles.ForEach(b => b.GetComponent<Rigidbody2D>().simulated = false);
            bubbles.Reverse();
            foreach (var bubble in bubbles)
            {
                float waitTime = UnityEngine.Random.Range(0.05f, 0.2f);
                bubble.Pop();
                yield return new WaitForSeconds(waitTime);
            };

            UIManager.Instance.GameOver();
        }

        public void RestartGame()
        {
            SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
        }

        private void UpdateGameState(GameState newGameState)
        {
            _gameState = newGameState;

            switch (newGameState)
            {
                case GameState.Phase1:
                    AnimationManager.Instance.PlayDimOut();
                    break;
                case GameState.Phase2:
                    PlayPhase2();
                    break;
                case GameState.UICutscene:
                    break;
            }

            OnGameStateChanged?.Invoke(newGameState);
        }

        private void SetCurrentScore(int localHighestY)
        {
            if (GameState == GameState.Phase1)
            {
                _currentScore = localHighestY;
                UIManager.Instance.SetGreenScore(_currentScore);
            }
            else if (GameState == GameState.Phase2)
            {
                var phase2Score = Math.Max(_phase1Score - localHighestY, 0);
                _currentScore = _phase1Score  + phase2Score;
                UIManager.Instance.SetPinkScore(phase2Score);
            }
        }

        private void HandlePhaseEnd()
        {
            if (GameState == GameState.Phase1)
            {
                SetupPhase2();
            }
            else if (GameState == GameState.Phase2)
            {
                UIManager.Instance.PlayCutsceneGameOver();
            }
        }

        private void HandleOnMaxHeightAchieved()
        {
            SetupPhase2();
        }

        private void SetupPhase2()
        {
            HighFinder.Instance.ActivatePinkLine();
            _phase1Score = CurrentScore;
            Debug.Log("SetupPhase2");
            BubbleSpawner.Instance.PauseSpawner();
            UIManager.Instance.PlayCutsceneBetweenPhases();
        }

        private void PlayPhase2()
        {
            Debug.Log("Resume Game");
            BubbleSpawner.Instance.ResumeSpawner();
            AnimationManager.Instance.PlayScore();
            AnimationManager.Instance.PlayPinkArrows();
            BubbleSpawner.Instance.AddSoap(GameParameters.Instance.Phase2AdditionalSoap);
        }
    }
}

public enum GameState
    {
        Phase1,
        Phase2,
        UICutscene,
    }
