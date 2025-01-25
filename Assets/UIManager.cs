using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Slider soapTankSlider;
    public TMP_Text soapTankText;

    public TMP_Text CurrentScoreText;
    public TMP_Text BestScoreText;
    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SetTankValue(int value)
    {
        soapTankSlider.value = value;
        soapTankText.text = value.ToString();
    }

    public void SetCurrentScore(int value)
    {
        CurrentScoreText.text = value.ToString();
    }

    public void SetBestScore(int value)
    {
        BestScoreText.text = value.ToString();
    }
}
