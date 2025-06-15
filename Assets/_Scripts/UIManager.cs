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

[Serializable]
public struct RewardPhraseEntry
{
    public string phrase;
    public int thresholdScore;
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
    public ParticleSystem TextBubbleParticles;

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
    public GameObject FinishLineTopShadow;
    public GameObject FinishLineBottom;
    public GameObject FinishLineBottomShadow;

    // flags
    private bool _isReadyToSkipCutscene;
    private bool _isCreditsShown;
    private bool _wasLeaderboardActive;

    // other
    public TMP_Text HoldHint;
    public Image HoldHintFrame;
    public TMP_Text HoldHintShadow;
    private Action _onCutsceneFinished;
    private CutsceneType _currentCustsceneState;
    private GameState _savedGameState;
    string _lastRewardPhrase;
    float _lastRewardPhraseSfxPitch = 0.85f;
    private CameraShake _camShake;
    private const float _semiToneRatio = 1.059463094359f;

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
        InfoButton.onClick.AddListener(() => { ToggleCredits(true); });
        ReplayButton.onClick.AddListener(GameManager.Instance.ReplayButton);

        if (MusicManager.Instance.IsMusicMuted())
        {
            ShowMuteMusicButton(true);
            AnimationManager.Instance.PauseRadio();
        } else
        {
            ShowMuteMusicButton(false);
            AnimationManager.Instance.PlayRadio();
        }

        _camShake = Camera.main.GetComponentInParent<CameraShake>();
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
        bool isNewRecord = GameManager.Instance.CurrentScore > GameManager.Instance.ServerScore;

        if (isNewRecord)
        {
            AnimationManager.Instance.PlaySunglasses();
        }

        if (finalScore > 10) 
        {
            yield return ShowAnimatedScore(finalScore);
        }

        // Final display of the account
        Confetti.Play();
        SFXManager.Instance.PlayOneShot("win", GameParameters.Instance.WinVolume);
        string header = isNewRecord ? "NEW RECORD!" : GetRewardPhrase(finalScore);
        string bubbleContent =
        $"<link=fade><size=150%>{header}</size></link>\n" +
            $"<link=scale><size=170%><color=#fc699a>{finalScore}</color></size></link>";

        TextBubbleContent.transform.DOScale(1.1f, 0.1f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutSine);
        _camShake.Shake();
        DisplayTextBubble(bubbleContent, false, SkipParameter.CanSkipAfterWait, false);
    }

    public IEnumerator ShowAnimatedScore(int score)
    {
        float delay = 0.0001f; // Fixed delay for the fast part
        float deltaDelay = 0.025f;  // Increasing the delay to slow down

        // Fast part of the animation (all values except the last 10)
        for (int current = 0; current < score - 10; current += 10)
        {
            if (current % 50 == 0)
                SFXManager.Instance.PlayOneShot("score", GameParameters.Instance.ScoreVolume);

            DisplayTextBubble(
                $"<size=150%>{GetRewardPhrase(current)}</size>\n<link=scaleBig><size=170%><color=#fc699a>{current}</color></size></link>",
                false, SkipParameter.NonSkippable, false
            );
            yield return new WaitForSecondsRealtime(delay);
        }

        // Slow part of the animation (last 10 values)
        for (int current = score - 10; current <= score; current++)
        {
            SFXManager.Instance.PlayOneShot("score", GameParameters.Instance.ScoreVolume);

            DisplayTextBubble(
                $"<size=150%>{GetRewardPhrase(current)}</size>\n<link=scaleBig><size=170%><color=#fc699a>{current}</color></size></link>",
                false, SkipParameter.NonSkippable, false
            );
            yield return new WaitForSecondsRealtime(delay);
            delay += deltaDelay; // Gradually increase the delay
        }
    }

    private string GetRewardPhrase(int currentScore)
    {
        string result = "";
        foreach (var entry in GameParameters.Instance.RewardPhrases)
        {
            if (currentScore >= entry.thresholdScore)
                result = entry.phrase;
            else
                break;
        }
        if (_lastRewardPhrase != result)
        {
            _lastRewardPhrase = result;
            _lastRewardPhraseSfxPitch *= _semiToneRatio;
            SFXManager.Instance.PlayOneShot("reward_phrase", GameParameters.Instance.RewardPhraseVolume, _lastRewardPhraseSfxPitch, _lastRewardPhraseSfxPitch);
            TextBubbleParticles.Play();
            _camShake.Shake();
            TextBubbleContent.transform.DOScale(1.1f, 0.1f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutSine);
        }
        return result;
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
            FinishLineTopShadow.GetComponent<Animator>().Play("finish_line_show");
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
        FinishLineTopShadow.GetComponent<Animator>().Play("finish_line_hide");

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
        HideHoldHint();
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
        FinishLineBottomShadow.GetComponent<Canvas>().sortingOrder = -1;
        FinishLineBottomShadow.GetComponent<Animator>().Play("finish_line_hide");
        yield return new WaitForSeconds(GameParameters.Instance.DurationBeforeShowingTextBubble);
        _onCutsceneFinished = DisplayLeaderboardWindow;
    }

    private async void DisplayLeaderboardWindow()
    {
        await LeaderboardManager.Instance.SubmitScore(GameManager.Instance.CurrentScore);
        _onCutsceneFinished = null;
    }

    public void ToggleCredits(bool triggeredByButton = false)
    {
        if (_isCreditsShown)
        {
            if (triggeredByButton) SFXManager.Instance.PlayOneShot("button", GameParameters.Instance.UIClickVolume, 0.9f, 0.9f);

            // 1) hide _only_ the credits bubble
            TextBubbleBig.gameObject.SetActive(false);

            // 2) undim the background
            if (!TextBubble.gameObject.activeSelf && !_wasLeaderboardActive)
            {
                AnimationManager.Instance.PlayDimOut();
                HighlightCharacterElements(false);
            }

            if (TextBubble.gameObject.activeSelf)
            {
                SFXManager.Instance.PlayRandom(new List<string> { "duck1", "duck2", "duck3" });
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
            if (triggeredByButton) SFXManager.Instance.PlayOneShot("button", GameParameters.Instance.UIClickVolume, 1.1f, 1.1f);
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
            SFXManager.Instance.PlayRandom(new List<string> { "duck1", "duck2", "duck3" });
            AnalyticsManager.Instance.SendCreditsButtonPressed();
        }
    }

    public void DisplayHoldHint()
    {
        StartCoroutine(DisplayHoldHintCoroutine());
        IEnumerator DisplayHoldHintCoroutine() {
            if (GameManager.Instance.GameState == GameState.Phase1)
            {
                FinishLineTop.GetComponent<Animator>().Play("finish_line_hide");
                FinishLineTopShadow.GetComponent<Animator>().Play("finish_line_hide");
            }
            yield return new WaitForSeconds(0.2f);
            HoldHint.DOFade(1f, 0.2f).SetEase(Ease.InOutSine);
            HoldHintFrame.DOFade(1f, 0.2f).SetEase(Ease.InOutSine);
            HoldHintShadow.DOFade(0.7f, 0.2f).SetEase(Ease.InOutSine);
        }
    }

    public void HideHoldHint()
    {
        StartCoroutine(HideHoldHintCoroutine());
        IEnumerator HideHoldHintCoroutine()
        {
            HoldHint.DOFade(0f, 0.2f).SetEase(Ease.InOutSine);
            HoldHintFrame.DOFade(0f, 0.2f).SetEase(Ease.InOutSine);
            HoldHintShadow.DOFade(0f, 0.2f).SetEase(Ease.InOutSine);
            yield return new WaitForSeconds(0.2f);
            if (GameManager.Instance.GameState == GameState.Phase1)
            {
                FinishLineTop.GetComponent<Animator>().Play("finish_line_show");
                FinishLineTopShadow.GetComponent<Animator>().Play("finish_line_show");
            }
        }
    }

    private void DisplayTextBubble(string text, bool bigBubble = false, SkipParameter waitBeforeSkipping = SkipParameter.CanSkipImmediately, bool duckSound = true, bool restartTextFX = true)
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
