using System.Collections;
using UnityEngine;

public class AnimationManager : SingletonBehaviour<AnimationManager>
{
    [SerializeField] private Animator GreenLineAnimator;
    [SerializeField] private Animator PinkLineAnimator;
    [SerializeField] private Animator ScoreAnimator;
    [SerializeField] private Animator RadioAnimator;
    [SerializeField] private Animator Sunglasses;
    [SerializeField] private GameObject Dimmer;

    public void PlayGreenArrows()
    {
        PlayDimIn();
        GreenLineAnimator.Play("green_arrows");
        StartCoroutine(WaitForGreenArrowsEnd());
    }

    private IEnumerator WaitForGreenArrowsEnd()
    {
        UIManager.Instance.FinishLineTop.GetComponent<Canvas>().sortingLayerName = "foreground";
        yield return new WaitForSeconds(3.4f);
        UIManager.Instance.FinishLineTop.GetComponent<Canvas>().sortingLayerName = "background";
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
        yield return new WaitForSeconds(2.9f);
        PlayDimOut();
    }

    public void PlayDimIn(int layer = 1)
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

    public void PlaySunglasses()
    {
        Sunglasses.GetComponent<Animator>().Play("sunglasses");
    }
}
