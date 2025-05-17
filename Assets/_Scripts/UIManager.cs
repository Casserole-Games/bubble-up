using Assets._Scripts;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EasyTextEffects;
using System.Collections.Generic;
using Assets._Scripts.Leaderboard;
using System;
using DG.Tweening;

public enum SkipParameter
{
    CanSkipImmediately,
    CanSkipAfterWait,
    NonSkippable
}

public enum CutsceneType
{
    Start,
    Between,
    End
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
    public Button InfoButton;
    public Button ReplayButton;

    // UI Effects
    public ParticleSystem Confetti;
    public GameObject SoapBarParticles;

    // UI Finish Lines
    public GameObject FinishLineTop;
    public GameObject FinishLineBottom;

    // flags
    private bool _isReadyToSkipCutscene;
    private bool _isCreditsShown;
    private bool _wasLeaderboardActive;

    // other
    public TMP_Text HoldHint;
    private Action _onCutsceneFinished;
    private CutsceneType _currentCustsceneState;
    private GameState _savedGameState;

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += HandleGameStateChange;
        BubbleSpawner.OnTooManyShortHolds += DisplayHoldHint;
        BubbleSpawner.OnLongHold += HideHoldHint;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChange;
        BubbleSpawner.OnTooManyShortHolds -= DisplayHoldHint;
        BubbleSpawner.OnLongHold -= HideHoldHint;
    }

    private void HandleGameStateChange(GameState currentGameState)
    {
        if (currentGameState == GameState.UICutscene) _isReadyToSkipCutscene = false;
    }

    private void Start()
    {
        MusicButton.onClick.AddListener(MusicManager.Instance.Toggle);
        InfoButton.onClick.AddListener(ToggleCredits);
        ReplayButton.onClick.AddListener(GameManager.Instance.ReplayButton);
    }

    private void Update()
    {
        if (GameManager.Instance.GameState != GameState.UICutscene) return;

        if (_isCreditsShown && InputManager.InputDown())
        {
            ToggleCredits();
            return;
        }

        if (_isReadyToSkipCutscene && InputManager.InputDown())
        {
            SkipCutscene();
        }
    }

    public void ButtonPressed()
    {
        if (BubbleSpawner.Instance.IsGameOver && _isReadyToSkipCutscene)
        {
            SkipCutscene();
        }
    }

    private void SkipCutscene()
    {
        if (_isReadyToSkipCutscene)
        {
            AnalyticsManager.Instance.SendCutsceneComplete(GetCutsceneString(_currentCustsceneState));
            HideTextBubble();
            HighlightCharacterElements(false);
            _onCutsceneFinished?.Invoke();
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

    public void HighlightCharacterElements(bool shouldHighlight)
    {
        CharacterElementsCanvas.sortingLayerID = shouldHighlight ? SortingLayer.NameToID("foreground") : SortingLayer.NameToID("background");
    }

    public void SwitchDuckSprite()
    {
        DuckBack.gameObject.SetActive(!DuckBack.gameObject.activeSelf);
        DuckFront.gameObject.SetActive(!DuckFront.gameObject.activeSelf);
    }

    public void GameOver()
    {
        StartCoroutine(FinalScoreAnimation());
    }

    public IEnumerator FinalScoreAnimation()
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
        AnalyticsManager.Instance.SendFinalScore(finalScore);
    }

    public IEnumerator ShowAnimatedScore(string textBeforeScore, int score)
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

    public void PlayCutsceneStart()
    {
        _currentCustsceneState = CutsceneType.Start;
        AnalyticsManager.Instance.SendCutsceneStart(GetCutsceneString(_currentCustsceneState));
        GameManager.Instance.GameState = GameState.UICutscene;
        HideTextBubble();
        HighlightCharacterElements(true);
        DisplayTextBubble(TutorialText, false, SkipParameter.CanSkipImmediately);
        _onCutsceneFinished = () => {
            FinishLineTop.GetComponent<Animator>().Play("finish_line_show");
            GameManager.Instance.PlayPhase1();
        };
    }

    public void PlayCutsceneBetweenPhases()
    {
        _currentCustsceneState = CutsceneType.Between;
        AnalyticsManager.Instance.SendCutsceneStart(GetCutsceneString(_currentCustsceneState));
        GameManager.Instance.GameState = GameState.UICutscene;
        AnimationManager.Instance.PlayDimIn(3);
        StartCoroutine(CutsceneBetweenPhases());
        _onCutsceneFinished = () => {
            GameManager.Instance.PlayPhase2();
        };
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
        _currentCustsceneState = CutsceneType.End;
        AnalyticsManager.Instance.SendCutsceneStart(GetCutsceneString(_currentCustsceneState));
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
        FinishLineBottom.GetComponent<Canvas>().sortingOrder = -1;
        FinishLineBottom.GetComponent<Animator>().Play("finish_line_hide");
        yield return new WaitForSeconds(GameParameters.Instance.DurationBeforeShowingTextBubble);
        _onCutsceneFinished = DisplayLeaderboardWindow;
    }

    private async void DisplayLeaderboardWindow()
    {
        await LeaderboardManager.Instance.SubmitScore(GameManager.Instance.CurrentScore);
        EditNameCanvasController.Instance.DisplayEditNamePanel(true);
        _onCutsceneFinished = null;
    }

    public void ToggleCredits()
    {
        if (_isCreditsShown)
        {
            // 1) hide _only_ the credits bubble
            TextBubbleBig.gameObject.SetActive(false);

            // 2) undim the background
            if (!TextBubble.gameObject.activeSelf && !_wasLeaderboardActive)
            {
                AnimationManager.Instance.PlayDimOut();
                HighlightCharacterElements(false);
            }

            if (_wasLeaderboardActive)
            {
                LeaderboardUIManager.Instance.DisplayLeaderboardFrame();
                HighlightCharacterElements(false);
            }

            if (_savedGameState != GameManager.Instance.GameState) GameManager.Instance.GameState = _savedGameState;
            _isCreditsShown = false;
        }
        else
        {
            _savedGameState = GameManager.Instance.GameState;

            // ensure we're in cutscene mode
            if (GameManager.Instance.GameState != GameState.UICutscene)
            {
                GameManager.Instance.GameState = GameState.UICutscene;
                AnimationManager.Instance.PlayDimIn(3);
                HighlightCharacterElements(true);
            }
            
            if (LeaderboardUIManager.Instance.IsLeaderboardFrameActive())
            {
                _wasLeaderboardActive = true;
                LeaderboardUIManager.Instance.HideLeaderboardFrame();
                HighlightCharacterElements(true);
            }

            // show the big bubble containing your credits
            TextBubbleBig.gameObject.SetActive(true);
            TextBubbleBigContent.text = CreditText;
            TextBubbleBigContent.GetComponent<TextEffect>().Refresh();
            TextBubbleBigContent.GetComponent<TextEffect>().StartManualTagEffects();

            // allow the user to skip away from credits if they tap again
            _isReadyToSkipCutscene = true;

            _isCreditsShown = true;
            AnalyticsManager.Instance.SendCreditsButtonPressed();
        }
    }

    public void DisplayHoldHint()
    {
        StartCoroutine(DisplayHoldHintCoroutine());
        IEnumerator DisplayHoldHintCoroutine() {
            FinishLineTop.GetComponent<Animator>().Play("finish_line_hide");
            yield return new WaitForSeconds(0.2f);
            HoldHint.DOFade(1f, 0.2f).SetEase(Ease.InOutSine);
        }
    }

    public void HideHoldHint()
    {
        StartCoroutine(HideHoldHintCoroutine());
        IEnumerator HideHoldHintCoroutine()
        {
            HoldHint.DOFade(0f, 0.2f).SetEase(Ease.InOutSine);
            yield return new WaitForSeconds(0.2f);
            FinishLineTop.GetComponent<Animator>().Play("finish_line_show");
        }
    }

    private void DisplayTextBubble(string text, bool bigBubble = false, SkipParameter waitBeforeSkipping = SkipParameter.CanSkipImmediately, bool duckSound = true)
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
                _isReadyToSkipCutscene = true;
                break;
            case SkipParameter.NonSkippable:
                break;
        }

        if (duckSound)
        {
            SFXManager.Instance.PlayRandom(new List<string> { "duck1", "duck2", "duck3" });
        }
    }

    private void HideTextBubble()
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
        _isReadyToSkipCutscene = true;
    }

    private string GetCutsceneString(CutsceneType cutscene)
    {
        return cutscene switch
        {
            CutsceneType.Start => "cutscene:1",
            CutsceneType.Between => "cutscene:2",
            CutsceneType.End => "cutscene:3",
            _ => "",
        };
    }
}
