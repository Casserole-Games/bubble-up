using UnityEngine;
using DG.Tweening;

public class CameraShake : MonoBehaviour
{
    [Header("Shake Settings")]
    [Tooltip("Duration of the shake, in seconds.")]
    public float duration = 0.5f;
    [Tooltip("Strength of the shake (in world units).")]
    public float strength = 0.3f;
    [Tooltip("Number of shakes over the duration.")]
    public int vibrato = 20;
    [Tooltip("How random the shake is (0 = straight, 180 = completely random).")]
    [Range(0, 180)]
    public float randomness = 90f;
    [Tooltip("If true, will snap to integer positions.")]
    public bool snapping = false;
    [Tooltip("If true, shake will fade out gradually.")]
    public bool fadeOut = true;

    private Tween _shakeTween;

    void Start()
    {
        _shakeTween = transform.DOShakePosition(duration, strength, vibrato, randomness, snapping, fadeOut)
                              .SetAutoKill(false)
                              .Pause();
    }

    public void Shake()
    {
        _shakeTween.Restart(includeDelay: true);
    }
}
