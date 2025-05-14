using UnityEngine;
using UnityEngine.EventSystems;

public class MusicManager : SingletonPersistent<MusicManager>
{
    private AudioSource audioSource;

    override protected void Awake()
    {
        base.Awake();
        audioSource = GetComponent<AudioSource>();
        if (!audioSource.isPlaying) audioSource.Play();
    }

    public void Toggle()
    {
        AnalyticsManager.Instance.SendMuteMusicButtonPressed(!audioSource.mute);
        if (!audioSource.mute)
        {
            Mute();
        }
        else
        {
            Unmute();
        }
    }

    public void Mute()
    {
        audioSource.mute = true;
        AnimationManager.Instance.PauseRadio();
        UIManager.Instance.ShowMuteMusicButton(true);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void Unmute()
    {
        audioSource.mute = false;
        AnimationManager.Instance.PlayRadio();
        UIManager.Instance.ShowMuteMusicButton(false);
        EventSystem.current.SetSelectedGameObject(null);
    }
}
