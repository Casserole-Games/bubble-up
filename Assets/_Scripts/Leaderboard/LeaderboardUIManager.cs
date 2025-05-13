using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Leaderboards.Models;
using UnityEngine;

namespace Assets._Scripts.Leaderboard
{

    internal class LeaderboardUIManager : SingletonBehaviour<LeaderboardUIManager>
    {
        public Transform Container;
        public GameObject LeaderboardFrame;

        public GameObject PlayerScorePrefab;
        public GameObject PlayerScorePrefabDottedLine;
        public GameObject PlayAgainButtonPrefab;

        public float WindowShowAnimTime = 0.25f;
        public float WindowCloseAnimTime = 0.25f;

        private readonly List<GameObject> UiElements = new();


        protected void Start()
        {
            Container.transform.localScale = Vector3.zero;
            LeaderboardManager.Instance.OnLeaderboardUpdated += async () => await UpdateUI();
            EditNameCanvasController.Instance.OnEditNamePanelClose += () => DisplayLeaderboard();
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
                    return InstantiatePlayerScore(entry);
                    
                }).ToList());
            }
            else
            {
                List<LeaderboardEntry> leaderboardEntries = await LeaderboardManager.Instance.GetLeaderboardTop(3);
                UiElements.AddRange(leaderboardEntries.Select(entry => InstantiatePlayerScore(entry)));

                UiElements.Add(Instantiate(PlayerScorePrefabDottedLine, Container));

                List<LeaderboardEntry> playerNeighbours = await LeaderboardManager.Instance.GetPlayerNeighbours();
                UiElements.AddRange(playerNeighbours.Select(entry =>
                    InstantiatePlayerScore(entry)));
            }

            UiElements.Add(Instantiate(PlayAgainButtonPrefab, Container));
        }

        private GameObject InstantiatePlayerScore(LeaderboardEntry entry)
        {
            GameObject gameObject = Instantiate(PlayerScorePrefab, Container);
            PlayerScoreUI playerScoreUI = gameObject.GetComponent<PlayerScoreUI>();
            playerScoreUI.SetEntryAndIsCurrentPlayer(entry);
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

        public void DisplayLeaderboard()
        {
            DisplayLeaderboardFrame();
            Container.gameObject.SetActive(true);
            Container.DOScale(Vector3.one, WindowShowAnimTime).SetEase(Ease.OutBack);
        }

        public bool IsLeaderboardFrameActive()
        {
            return LeaderboardFrame.activeSelf;
        }

        public void DisplayLeaderboardFrame()
        {
            LeaderboardFrame.SetActive(true);
        }

        public void HideLeaderboardFrame()
        {
            LeaderboardFrame.SetActive(false);
        }
    }
}
