using System;
using System.Collections;
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

        private float durationBeforeShowingTextBubble => GameParameters.Instance.DurationBeforeShowingTextBubble;
        private float durationBeforeDuckTurnsAround => GameParameters.Instance.DurationBeforeDuckTurnsAround;
        private float textDisplayDuration => GameParameters.Instance.Phase2TextDisplayDuration;

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
                GameOver();
            }
        }

        private void SetupPhase2()
        {
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
            BubbleSpawner.RemainingSoap = 8;
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
            UIManager.Instance.GameOver();
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

        public void RestartGame()
        {
            SceneManager.LoadScene("SampleScene");
        }
    }
}
