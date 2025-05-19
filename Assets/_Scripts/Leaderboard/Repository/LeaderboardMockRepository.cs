using Assets._Scripts.Leaderboard.DependenciesContainer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Services.Leaderboards.Models;
using UnityEngine;

namespace Assets._Scripts.Leaderboard
{
    public class LeaderboardMockRepository
    {
        public string MockPlayerId = "MOCKPLAYERID";

        public List<LeaderboardEntry> MockLeaderboardEntries;
        public LeaderboardMockRepository()
        {
            MockLeaderboardEntries = new List<LeaderboardEntry>();
        }

        public void AddMockEntry(string playerId, string playerName, int score)
        {
            var oldEntry = MockLeaderboardEntries.FirstOrDefault(e => e.PlayerId == playerId);

            if (oldEntry == null || oldEntry.Score <= score)
            {
                if (oldEntry != null)
                {
                    MockLeaderboardEntries.Remove(oldEntry);
                }

                var entry = new LeaderboardEntry(playerId, playerName, 0, score);
                MockLeaderboardEntries.Add(entry);
                SortEntriesAndAssignRanks();
            }
        }

        private void SortEntriesAndAssignRanks()
        {
            var sortedEntries = MockLeaderboardEntries.OrderByDescending(entry => entry.Score).ToList();
            MockLeaderboardEntries.Clear();
            for (int i = 0; i < sortedEntries.Count; i++)
            {
                var newEntry = new LeaderboardEntry(sortedEntries[i].PlayerId, sortedEntries[i].PlayerName, i, sortedEntries[i].Score);
                MockLeaderboardEntries.Add(newEntry);
            }
        }

        public void SetEntries(int scoreMin, int scoreMax, int count, int forcedPlayerScore)
        {
            MockLeaderboardEntries.Clear();

            if (forcedPlayerScore != -1)
            {
                AddMockEntry(DependencyContainer.AuthenticationManager.PlayerId, DependencyContainer.AuthenticationManager.PlayerName, forcedPlayerScore);
            }
            if (count == 0)
            {
                return;
            }
            else if (count == 1)
            {
                AddMockEntry("PlayerId0", "Player0", scoreMin);
                return;
            }
            float step = (float)(scoreMax - scoreMin) / (count - 1);

            for (int i = 0; i < count; i++)
            {
                int score = Mathf.RoundToInt(scoreMax - step * i);
                string playerId = $"PlayerId{i}";
                string playerName = $"Player{i}";

                AddMockEntry(playerId, playerName, score);
            }
        }
    }
}
