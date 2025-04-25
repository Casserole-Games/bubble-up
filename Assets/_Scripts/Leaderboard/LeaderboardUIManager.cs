using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Leaderboards.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Scripts.Leaderboard
{

    internal class LeaderboardUIManager : SingletonBehaviour<LeaderboardUIManager>
    {
        public Transform Container;
        public GameObject LeaderboardFrame;

        public GameObject PlayerScorePrefab;
        public GameObject PlayerScorePrefabInput;
        public GameObject PlayerScorePrefabTop3;
        public GameObject PlayerScorePrefabDottedLine;


        private readonly List<GameObject> uiElements = new();

        private async void Start()
        {
           // await UpdateUI(true);
        }

        public async Task UpdateUI(bool isInputScren, int score = 0)
        {
            ClearUI();
            List<LeaderboardEntry> leaderboardEntries = (await LeaderboardManager.Instance.GetLeaderboardTop7()).Results;

            if (isInputScren) { 
            
                var inputFielUIElement = InstantiateInputField(score);
                uiElements.Add(inputFielUIElement);
            }
            for (int i = 0; i < 7; i++)
            {
                GameObject UIElement;
                if (i < 3)
                {
                    UIElement = InstantiateTop3PlayerScore(leaderboardEntries[i]);
                }
                else
                {
                    UIElement = InstantiatePlayerScore(leaderboardEntries[i]);
                }
                if (!isInputScren && i == 2)
                {
                    uiElements.Add(Instantiate(PlayerScorePrefabDottedLine, Container));
                }
                uiElements.Add(UIElement);

            }
        }

        private async void DisplayInputFieldScreen()
        {

        }

        private async void DisplayGlobalLeaderboard()
        {

        }

        private GameObject InstantiateInputField(int score)
        {
            GameObject gameObject = Instantiate(PlayerScorePrefabInput, Container);
            InputFieldLeaderBoard script = gameObject.GetComponent<InputFieldLeaderBoard>();
            script.UpdateScore(score);
            return gameObject;
        }
        private GameObject InstantiateTop3PlayerScore(LeaderboardEntry entry)
        {
            GameObject gameObject = Instantiate(PlayerScorePrefabTop3, Container);
            PlayerScoreUI playerScoreUI = gameObject.GetComponent<PlayerScoreUITop3>();
            playerScoreUI.SetEntry(entry);
            playerScoreUI.UpdateUI();
            return gameObject;
        }

        private GameObject InstantiatePlayerScore(LeaderboardEntry entry)
        {
            GameObject gameObject = Instantiate(PlayerScorePrefab, Container);
            PlayerScoreUI playerScoreUI = gameObject.GetComponent<PlayerScoreUI>();
            playerScoreUI.SetEntry(entry);
            playerScoreUI.UpdateUI();
            return gameObject;
        }

        private void ClearUI()
        {
            foreach (GameObject uiElement in uiElements)
            {
                Destroy(uiElement);
            }
            uiElements.Clear();
        }

        private void AnimateUI()
        {
            var layoutGroup = Container.GetComponent<VerticalLayoutGroup>();
        }

        internal async void DisplayLeaderboard(int currentScore)
        {
            LeaderboardFrame.SetActive(true);
            await UpdateUI(true, currentScore);
        }
    }
}
