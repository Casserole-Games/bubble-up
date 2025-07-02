using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Scripts.Leaderboard
{
    internal static class BadWordsFilter
    {
        private static HashSet<string> _badWords;

        public static bool ContainsBadWords(string input)
        {
            if (_badWords == null || _badWords.Count == 0)
            {
                LoadBadWords();
            }

            return _badWords.Contains(input.ToLowerInvariant());
        }

        private static void LoadBadWords()
        {
            TextAsset textAsset = Resources.Load<TextAsset>("Texts/badwords");
            if (textAsset == null)
            {
                Debug.LogError("File badwords.txt was not found in Resources.");
                _badWords = new HashSet<string>();
                return;
            }
            _badWords = new HashSet<string>();
            string[] lines = textAsset.text.Split('\n');
            foreach (string line in lines)
            {
                string word = line.Trim().ToLower();
                if (word.Length > 0)
                    _badWords.Add(word);
            }
        }
    }
}
