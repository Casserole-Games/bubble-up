using Assets._Scripts;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EasyTextEffects;
using DG.Tweening;
using System.Security;
using System.Drawing;
using System.Collections.Generic;
using Assets._Scripts.Leaderboard;

public enum SkipParameter
{
    CanSkipImmediately,
    CanSkipAfterWait,
    NonSkippable
}

public class UIManager : SingletonBehaviour<UIManager>
{
    // UI top
    public TMP_Text GreenScoreText;
    public TMP_Text PinkScoreText;
    public Slider soapTankSlider;

    // UI bubble
    public Image TextBubble;
    public TMP_Text TextBubbleContent;
    public Image TextBubbleBig;
    public TMP_Text TextBubbleBigContent;

    // duck
    public Canvas CharacterElementsCanvas;
    public Image DuckBack;
    public Image DuckFront;

    // UI texts
    public string TutorialText = "";
    public string Phase2Text = "";
    public string GameOverText = "";
    public string CreditText = "";

    // UI Buttons
    public Button MusicButton;
    public Sprite MusicButtonUnmuteSprite;
    public Sprite MusicButtonMuteSprite;

    // UI Effects
    public ParticleSystem Confetti;
    public GameObject SoapBarParticles;

    // UI Finish Lines
    public GameObject FinishLineTop;
    public GameObject FinishLineBottom;

    // flags
    private bool _isReadyToSkipText;
    private bool _isStartCutsceneSkipped;

    // other
    private GameState nextGameState;
    private delegate void Cutscene();
    private Cutscene nextCutscene;

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += HandleGameStateChange;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChange;
    }

    private void HandleGameStateChange(GameState currentGameState)
    {
        if (currentGameState == GameState.UICutscene) _isReadyToSkipText = false;
    }

    protected override void Awake()
    {
        base.Awake();
        MusicButton.onClick.AddListener(MusicManager.Instance.Toggle);
    }

    private void Update()
    {
        if (GameManager.Instance.GameState != GameState.UICutscene) return;
        if (_isReadyToSkipText && InputManager.InputDown())
        {
            if (nextCutscene != null) 
            {
                nextCutscene();
            } else {
                SkipText();
            }
        }
    }

    public void SetTankValue(int value)
    {
        soapTankSlider.value = value / GameParameters.Instance.MaxSoapAmount;
    }

    public void SetGreenScore(int value)
    {
        GreenScoreText.text = value.ToString();
    }

    public void SetPinkScore(int value)
    {
        PinkScoreText.text = value.ToString();
    }

    internal void HighlightCharacterElements(bool shouldHighlight)
    {
        CharacterElementsCanvas.sortingLayerID = shouldHighlight ? SortingLayer.NameToID("foreground") : SortingLayer.NameToID("background");
    }

    internal void DisplayTetBubble(bool shouldDisplay, string text = "")
    {
        TextBubble.gameObject.SetActive(shouldDisplay);

        if (text == "") return;
        TextBubbleContent.text = text;
    }

    internal void DisplayTextBubbleBig(bool shouldDisplay, string text = "")
    {
        TextBubbleBig.gameObject.SetActive(shouldDisplay);

        if (text == "") return;
        TextBubbleBigContent.text = text;
    }

    internal void SwitchDuckSprite()
    {
        DuckBack.gameObject.SetActive(!DuckBack.gameObject.activeSelf);
        DuckFront.gameObject.SetActive(!DuckFront.gameObject.activeSelf);
    }

    internal void GameOver()
    {
        StartCoroutine(FinalScoreAnimation());
    }

    internal IEnumerator FinalScoreAnimation()
    {
        int finalScore = GameManager.Instance.CurrentScore;

        if (BubbleSpawner.RemainingSoap > 0)
        {
            finalScore += (int)BubbleSpawner.RemainingSoap * 10;
        }

        if (finalScore > 10) 
        {
            yield return ShowAnimatedScore(GameOverText, finalScore);
        }

        // Final display of the account
        Confetti.Play();
        SFXManager.Instance.PlayOneShot("win", GameParameters.Instance.WinVolume);
        DisplayTextBubble("FINAL SCORE:\n<link=scale><size=180%><color=#fc699a>" + finalScore + "</color></size></link>", false, SkipParameter.CanSkipAfterWait, false);
    }

    internal IEnumerator ShowAnimatedScore(string textBeforeScore, int score)
    {
        float delay = 0.0001f; // Fixed delay for the fast part
        float deltaDelay = 0.025f;  // Increasing the delay to slow down

        // Fast part of the animation (all values except the last 10)
        for (int currentScore = 0; currentScore < score - 10; currentScore += 10)
        {
            if (currentScore % 50 == 0)
            {
                SFXManager.Instance.PlayOneShot("score", GameParameters.Instance.ScoreVolume);
            }
            DisplayTextBubble(textBeforeScore + "<link=scaleBig><size=130%><color=#fc699a>" + currentScore + "</color></size></link>", false, SkipParameter.NonSkippable, false);
            yield return new WaitForSecondsRealtime(delay);
        }

        // Slow part of the animation (last 10 values)
        for (int currentScore = score - 10; currentScore <= score; currentScore++)
        {
            SFXManager.Instance.PlayOneShot("score", GameParameters.Instance.ScoreVolume);
            DisplayTextBubble(textBeforeScore + "<link=scaleBig><size=130%><color=#fc699a>" + currentScore + "</color></size></link>", false, SkipParameter.NonSkippable, false);
            yield return new WaitForSecondsRealtime(delay);
            delay += deltaDelay; // Gradually increase the delay
        }
    }

    internal void ButtonPressed()
    {
        if (BubbleSpawner.Instance.IsGameOver && _isReadyToSkipText)
        {
            SkipText();
        }
    }

    private void SkipText()
    {
        if (_isReadyToSkipText)
        {
            HideTextBubble();
            HighlightCharacterElements(false);
            GameManager.Instance.GameState = nextGameState;

            // Show top finish line
            if (!_isStartCutsceneSkipped)
            {
                FinishLineTop.GetComponent<Animator>().Play("finish_line_show");
                _isStartCutsceneSkipped = true;
            }
        }
    }

    public void PlayCutsceneStart()
    {
        GameManager.Instance.GameState = GameState.UICutscene;
        nextGameState = GameState.Phase1;
        HideTextBubble();
        HighlightCharacterElements(true);
        DisplayTextBubble(TutorialText, false, SkipParameter.CanSkipImmediately);
    }

    public void PlayCutsceneBetweenPhases()
    {
        GameManager.Instance.GameState = GameState.UICutscene;
        nextGameState = GameState.Phase2;
        AnimationManager.Instance.PlayDimIn(3);
        StartCoroutine(CutsceneBetweenPhases());
    }

    private IEnumerator CutsceneBetweenPhases()
    {
        // finish line accent
        if (HighFinder.Instance.MaxHeightAchieved)
        {
            FinishLineTop.GetComponentInParent<Canvas>().sortingLayerID = SortingLayer.NameToID("foreground");
            FinishLineTop.GetComponent<Animator>().Play("finish_line_accent");
            yield return new WaitForSeconds(1.5f);
            FinishLineTop.GetComponentInParent<Canvas>().sortingLayerID = SortingLayer.NameToID("background");
        }

        AnimationManager.Instance.PlayGreenLineFade();
        FinishLineTop.GetComponent<Animator>().Play("finish_line_hide");

        // put text bubble, duck and bathtub in front of everything
        HighlightCharacterElements(true);

        yield return new WaitForSeconds(GameParameters.Instance.DurationBeforeDuckTurnsAround);
        // switch duck sprite
        SwitchDuckSprite();

        yield return new WaitForSeconds(GameParameters.Instance.DurationBeforeShowingTextBubble);

        DisplayTextBubble(Phase2Text, false, SkipParameter.CanSkipAfterWait);
    }

    public void PlayCutsceneGameOver()
    {
        GameManager.Instance.GameState = GameState.UICutscene;
        AnimationManager.Instance.PlayDimIn(3);
        HighFinder.Instance.LocalHighGreenLine.GetComponent<Animator>().Play("green_line_hide");
        HighFinder.Instance.LocalHighPinkLine.GetComponent<Animator>().Play("pink_line_hide");
        StartCoroutine(CutsceneGameOver());
    }

    private IEnumerator CutsceneGameOver()
    {
        // finish line accent
        if (HighFinder.Instance.MinHeightAchieved)
        {
            FinishLineBottom.GetComponent<Canvas>().sortingOrder = 4;
            FinishLineBottom.GetComponent<Animator>().Play("finish_line_accent");
            yield return new WaitForSeconds(1.5f);
            FinishLineBottom.GetComponent<Canvas>().sortingOrder = -1;
        }

        if (HighFinder.Instance.MaxHeightAchieved && HighFinder.Instance.MinHeightAchieved)
        {
            AnimationManager.Instance.PlaySunglasses();
        }

        if (BubbleSpawner.RemainingSoap > 0)
        {
            BubbleSpawner.Instance.AddSoap(-BubbleSpawner.RemainingSoap, 2f);
            SoapBarParticles.GetComponent<ParticleSystem>().Play();
            SoapBarParticles.GetComponent<MoveParticlesToTarget>().StartMovement();
        }

        HighlightCharacterElements(true);
        BubbleSpawner.Instance.PauseSpawner();
        BubbleSpawner.Instance.IsGameOver = true;
        StartCoroutine(GameManager.Instance.PopAllBubbles());
        yield return new WaitForSeconds(GameParameters.Instance.DurationBeforeShowingTextBubble);
        nextCutscene = PlayCutsceneGameOverPt2;
    }

    public void PlayCutsceneGameOverPt2()
    {
        DisplayTextBubble(CreditText, true, SkipParameter.CanSkipAfterWait);

        LeaderboardUIManager.Instance.DisplayLeaderboard(GameManager.Instance.CurrentScore);

        nextCutscene = () => { };
    }

    public void DisplayTextBubble(string text, bool bigBubble = false, SkipParameter waitBeforeSkipping = SkipParameter.CanSkipImmediately, bool duckSound = true)
    {
        if (text == "") return;
        if (bigBubble)
        {
            TextBubbleBig.gameObject.SetActive(true);
            TextBubbleBigContent.text = text;
            TextBubbleBigContent.GetComponent<TextEffect>().Refresh();
            TextBubbleBigContent.GetComponent<TextEffect>().StartManualTagEffects();

        } else
        {
            TextBubble.gameObject.SetActive(true);
            TextBubbleContent.text = text;
            TextBubbleContent.GetComponent<TextEffect>().Refresh();
            TextBubbleContent.GetComponent<TextEffect>().StartManualTagEffects();
        }
        switch (waitBeforeSkipping)
        {
            case SkipParameter.CanSkipAfterWait:
                StartCoroutine(WaitBeforeSkipping());
                break;
            case SkipParameter.CanSkipImmediately:
                _isReadyToSkipText = true;
                break;
            case SkipParameter.NonSkippable:
                break;
        }

        if (duckSound)
        {
            SFXManager.Instance.PlayRandom(new List<string> { "duck1", "duck2", "duck3" });
        }
    }

    public void HideTextBubble()
    {
        TextBubbleBig.gameObject.SetActive(false);
        TextBubble.gameObject.SetActive(false);
    }

    public void ShowMuteMusicButton(bool isMute)
    {
        if (isMute)
        {
            MusicButton.GetComponent<Image>().sprite = MusicButtonMuteSprite;
        } else
        {
            MusicButton.GetComponent<Image>().sprite = MusicButtonUnmuteSprite;
        }
    }

    private IEnumerator WaitBeforeSkipping()
    {
        yield return new WaitForSeconds(GameParameters.Instance.DurationBeforeSkipping);
        _isReadyToSkipText = true;
    }
}
