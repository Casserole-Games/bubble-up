using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets._Scripts
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        private bool isPhase1 = true;
        private bool isPhase2 = false;
        public bool IsGameOver = false;

        private float durationBeforeShowingTextBubble => GameParameters.Instance.DurationBeforeShowingTextBubble;
        private float durationBeforeDuckTurnsAround => GameParameters.Instance.DurationBeforeDuckTurnsAround;
        private float textDisplayDuration => GameParameters.Instance.Phase2TextDisplayDuration;

        public int CurrentScore { get; private set; }
        private int phase1Score;

        internal void TriggerEmptyTank()
        {
            if (isPhase1)
            {
                isPhase1 = false;
                isPhase2 = true;
                SetupPhase2();
            }
            else if (isPhase2)
            {
                IsGameOver = true;
                GameOver();
            }
        }

        private void SetupPhase2()
        {
            HighFinder.Instance.SetToPhase2();
            phase1Score = CurrentScore;
            Debug.Log("SetupPhase2");
            BubbleSpawner.Instance.PauseGame();
            StartCoroutine(DisplayTextBubbleCoroutine());
        }

        private IEnumerator DisplayTextBubbleCoroutine()
        {
            //put text bubble, duck and bathtub in front of everything
            HighlightCharacterElements(true);

            yield return new WaitForSeconds(durationBeforeDuckTurnsAround);
            //switch duck sprite
            UIManager.Instance.SwitchDuckSprite();

            yield return new WaitForSeconds(durationBeforeShowingTextBubble);

            //display text bubble
            DisplayTextBubble(true);

            //wait for textDisplayDuration
            yield return new WaitForSeconds(textDisplayDuration);

            //put text bubble, duck and bathtub back to their original layers
            HighlightCharacterElements(false);

            //hide text bubble
            DisplayTextBubble(false);
            
            //go on with phase 2
            PlayPhase2();
        }

        private void PlayPhase2()
        {
            Debug.Log("Resume Game");
            BubbleSpawner.Instance.ResumeGame();
            BubbleSpawner.RemainingSoap = GameParameters.Instance.Phase2StartSoap;
            //register actual green line ??
        }

        private void DisplayTextBubble(bool shouldDisplay)
        {
            UIManager.Instance.DisplayTextBubble(shouldDisplay);
        }

        private void HighlightCharacterElements(bool shouldHighlight)
        {
            UIManager.Instance.HighlightCharacterElements(shouldHighlight);
        }

        private void GameOver()
        {
            BubbleSpawner.Instance.IsPaused = true;
            BubbleSpawner.Instance.IsGameOver = true;
            StartCoroutine(PopAllBubbles());
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
                Destroy(gameObject);
        }

        public IEnumerator PopAllBubbles()
        {
            yield return new WaitForSeconds(2.0f);

            List<Bubble> bubbles = GameObject.FindGameObjectsWithTag("Bubble").Select(t => t.GetComponent<Bubble>()).ToList();
            bubbles.ForEach(b => b.GetComponent<Rigidbody2D>().simulated = false);
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
            SceneManager.LoadScene("SampleScene");
        }

        internal void SetCurrentScore(int localHighestY)
        {
            if (isPhase1)
            {
                CurrentScore = localHighestY;
            }
            else  if (isPhase2)
            {
                var phase2Score = Math.Max(phase1Score - localHighestY, 0);
                CurrentScore = phase1Score  + phase2Score;
            }
            UIManager.Instance.SetCurrentScore(CurrentScore);
        }
    }
}
