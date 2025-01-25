using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Scripts
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);

        }

        public Slider soapTankSlider;
        public TMP_Text soapTankText;

        public void SetTankValue(int value)
        {
            soapTankSlider.value = value;
            soapTankText.text = value.ToString();
        }
    }
}
