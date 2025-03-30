using System.Collections;
using UnityEngine;

public class AnimationManager : SingletonBehaviour<AnimationManager>
{
    [SerializeField] private Animator GreenLineAnimator;
    [SerializeField] private Animator PinkLineAnimator;
    [SerializeField] private Animator ScoreAnimator;
    [SerializeField] private Animator RadioAnimator;
    [SerializeField] private GameObject Dimmer;

    public void PlayGreenArrows()
    {
        PlayDimIn();
        GreenLineAnimator.Play("green_arrows");
        StartCoroutine(WaitForGreenArrowsEnd());
    }

    private IEnumerator WaitForGreenArrowsEnd()
    {
        yield return new WaitForSeconds(2f);
        PlayDimOut();
    }

    public void PlayGreenLineFade()
    {
        GreenLineAnimator.Play("green_line_fade");
    }

    public void PlayPinkArrows()
    {
        PlayDimIn();
        PinkLineAnimator.Play("pink_arrows");
        StartCoroutine(WaitForPinkArrowsEnd());
    }

    private IEnumerator WaitForPinkArrowsEnd()
    {
        yield return new WaitForSeconds(0.5f);
        UIManager.Instance.FinishLineBottom.GetComponent<Animator>().Play("finish_line_show");
        yield return new WaitForSeconds(1.5f);
        PlayDimOut();
        yield return new WaitForSeconds(0.5f);
        UIManager.Instance.FinishLineBottom.GetComponent<Canvas>().sortingOrder = -1;
    }

    public void PlayDimIn(int layer = 0)
    {
        Dimmer.GetComponent<Canvas>().sortingOrder = layer;
        Dimmer.GetComponentInChildren<Animator>().Play("dim_in");
    }

    public void PlayDimOut()
    {
        Dimmer.GetComponentInChildren<Animator>().Play("dim_out");
    }

    public void PlayScore()
    {
        ScoreAnimator.Play("score");
    }

    public void PlayRadio()
    {
        RadioAnimator.Play("radio");
        RadioAnimator.speed = 1;
    }

    public void PauseRadio() 
    {
        RadioAnimator.speed = 0;
    }
}
