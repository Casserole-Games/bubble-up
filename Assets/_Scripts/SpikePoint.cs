using Assets._Scripts;
using Unity.VisualScripting;
using UnityEngine;

public class SpikePoint : MonoBehaviour
{
    public static float phase1Radius = 0.036f;
    public static float phase2Radius = 0.06f;

    private Animator _highlightAnimator;
    private CameraShake _camShake;
    

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += IncreaseColliders;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= IncreaseColliders;
    }

    private void Start()
    {
        _highlightAnimator = GetComponentInChildren<Animator>();
        _camShake = Camera.main.GetComponentInParent<CameraShake>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Spike pointy end: Collision");
        if (collision.gameObject.CompareTag("Bubble"))
        {
            Debug.Log("Spike pointy end: Collision with bubble");
            collision.gameObject.GetComponent<Bubble>().Pop();
            _highlightAnimator.Play("spike_highlight", -1, 0f);
            _camShake.Shake();
        }
    }

    private void IncreaseColliders(GameState gameState)
    {
        if (gameObject.GetComponent<CircleCollider2D>() && (gameState == GameState.Phase2))
        {
            gameObject.GetComponent<CircleCollider2D>().radius = phase2Radius;
        }
    }
}
