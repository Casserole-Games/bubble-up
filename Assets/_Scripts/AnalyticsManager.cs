using GameAnalyticsSDK;
using System.Diagnostics;

public class AnalyticsManager : SingletonPersistent<AnalyticsManager>
{
    protected override void Awake()
    {
        base.Awake();
        GameAnalytics.Initialize();
    }

    public void SendCutsceneStart(string cutscene)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, cutscene);
    }

    public void SendCutsceneComplete(string cutscene)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, cutscene);
    }

    public void SendPhaseStart(string phase)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, phase);
    }

    public void SendPhaseComplete(string phase, int score, float soapRemaining)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, phase);
        GameAnalytics.NewDesignEvent($"phase:{phase}:score", score);
        GameAnalytics.NewDesignEvent($"phase:{phase}:soap_remaining", soapRemaining);
    }

    public void SendFinalScore(int score)
    {
        GameAnalytics.NewDesignEvent("game:final_score", score);
    }

    public void SendAdditionalSoapCollected()
    {
        GameAnalytics.NewDesignEvent("game:additional_soap");
    }

    public void SendReplayButtonPressed()
    {
        GameAnalytics.NewDesignEvent("ui:replay");
    }

    public void SendMuteMusicButtonPressed(bool isMuted)
    {
        GameAnalytics.NewDesignEvent("ui:mute_music", isMuted ? 1 : 0);
    }
}
