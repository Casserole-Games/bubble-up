using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
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
        public GameObject PlayerScorePrefabDottedLine;

        private readonly List<GameObject> UiElements = new();

        private async void Start()
        {
           // await UpdateUI(true);
        }

        protected override void Awake()
        {
            base.Awake();
            LeaderboardManager.Instance.OnLeaderboardUpdated += async () => await UpdateUI();
        }

        public async Task UpdateUI()
        {
            ClearUI();
            LeaderboardEntry playerEntry = await LeaderboardManager.Instance.GetPlayerEntry();

            if (playerEntry.Rank < 7)
            {
                List<LeaderboardEntry> leaderboardEntries = await LeaderboardManager.Instance.GetLeaderboardTop(8);
                UiElements.AddRange(leaderboardEntries.Select(entry =>
                {
                    if (entry.PlayerId == AuthenticationService.Instance.PlayerId)
                    {
                        return InstantiateInputField(entry);
                    }
                    else
                    {
                        return InstantiatePlayerScore(entry);
                    }
                }).ToList());
            }
            else
            {
                List<LeaderboardEntry> leaderboardEntries = await LeaderboardManager.Instance.GetLeaderboardTop(3);
                UiElements.AddRange(leaderboardEntries.Select(entry => InstantiatePlayerScore(entry)));

                UiElements.Add(Instantiate(PlayerScorePrefabDottedLine, Container));

                List<LeaderboardEntry> playerNeighbours = await LeaderboardManager.Instance.GetPlayerNeighbours();
                UiElements.AddRange(playerNeighbours.Select(entry =>
                {
                    if (entry.PlayerId == AuthenticationService.Instance.PlayerId)
                    {
                        return InstantiateInputField(entry);
                    }
                    else
                    {
                        return InstantiatePlayerScore(entry);
                    }
                }));
            }
        }

        private GameObject InstantiateInputField(LeaderboardEntry entry)
        {
            GameObject gameObject = Instantiate(PlayerScorePrefabInput, Container);
            CurrentPlayerScoreUI currentpLayerScoreUI = gameObject.GetComponent<CurrentPlayerScoreUI>();
            currentpLayerScoreUI.SetEntry(entry);
            currentpLayerScoreUI.UpdateUI();
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
            foreach (GameObject uiElement in UiElements)
            {
                Destroy(uiElement);
            }
            UiElements.Clear();
        }

        private async void DisplayLeaderboard(int currentScore)
        {
            await LeaderboardManager.Instance.SubmitScore(currentScore);
            LeaderboardFrame.SetActive(true);
            await UpdateUI();
        }
    }
}
