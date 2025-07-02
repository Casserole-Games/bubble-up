using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets._Scripts.Leaderboard
{
    [RequireComponent(typeof(TMP_InputField))]
    internal class InputFieldAlphanumericalCharactersValidator : MonoBehaviour
    {
        private TMP_InputField inputField;
        private static readonly Regex allowedChars = new Regex("[^a-zA-Z0-9]", RegexOptions.Compiled);

        private void Awake()
        {
            inputField = GetComponent<TMP_InputField>();
            inputField.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnDestroy()
        {
            inputField.onValueChanged.RemoveListener(OnValueChanged);
        }

        private void OnValueChanged(string value)
        {
            string filtered = allowedChars.Replace(value, "");
            if (filtered != value)
            {
                int caret = inputField.caretPosition - (value.Length - filtered.Length);
                inputField.text = filtered;
                inputField.caretPosition = Mathf.Clamp(caret, 0, filtered.Length);
            }
        }
    }
}
