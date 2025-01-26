using Assets._Scripts;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Canvas CharacterElementsCanvas;

    public Slider soapTankSlider;
    //public TMP_Text soapTankText;

    public TMP_Text CurrentScoreText;
    //public TMP_Text BestScoreText;

    public Image TextBubble;
    public TMP_Text TextBubbleContent;
    public Image DuckBack;
    public Image DuckFront;

    public Sprite DuckFirstSprite;
    public Sprite DuckSecondSprite;

    private bool readyToSkipText = false;
    private bool isGameOverText = false;
    private bool isCreditText = false;
    public string GameOverText = "";
    public string CreditText = "";

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
        soapTankSlider.value = value / GameParameters.Instance.MaxSoapAmount;
    }

    public void SetCurrentScore(int value)
    {
        CurrentScoreText.text = value.ToString();
    }

    internal void HighlightCharacterElements(bool shouldHighlight)
    {
        CharacterElementsCanvas.sortingLayerID = shouldHighlight ? SortingLayer.NameToID("foreground") : SortingLayer.NameToID("background");
    }
    internal void DisplayTextBubble(bool shouldDisplay)
    {
        TextBubble.gameObject.SetActive(shouldDisplay);
    }

    internal void SwitchDuckSprite()
    {
        DuckBack.gameObject.SetActive(!DuckBack.gameObject.activeSelf);
        DuckFront.gameObject.SetActive(!DuckFront.gameObject.activeSelf);
    }

    internal void GameOver()
    {
        DisplayGameOverText();
    }


    internal void ButtonPressed()
    {
        if (BubbleSpawner.Instance.IsGameOver && readyToSkipText)
        {
            SkipText();
        }
    }

    private void SkipText()
    {
        if (isGameOverText)
        {
            isGameOverText = false;
            DisplayCredits();
        }
        else if (isCreditText)
        {
            GameManager.Instance.RestartGame();
        }
    }

    private void DisplayGameOverText()
    {
        TextBubble.gameObject.SetActive(true);
        TextBubbleContent.text = GameOverText + " " + GameManager.Instance.CurrentScore;
        isGameOverText = true;
        StartCoroutine(WaitFor1SecondBeforeSkipping());
    }

    private void DisplayCredits()
    {
        TextBubble.gameObject.SetActive(true);
        TextBubbleContent.text = CreditText;
        isCreditText = true;
        StartCoroutine(WaitFor1SecondBeforeSkipping());
    }

    private IEnumerator WaitFor1SecondBeforeSkipping()
    {
        readyToSkipText = false;
        yield return new WaitForSeconds(1.0f);
        readyToSkipText = true;
    }
}
