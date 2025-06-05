using Assets._Scripts;
using UnityEngine;
using DG.Tweening;

public class EyesBehaviour : MonoBehaviour
{
    [SerializeField] private RectTransform _eyesRect;
    [SerializeField] private RectTransform _eyelidsRect;
    [SerializeField] private RectTransform _eyelidsUpRect;
    [SerializeField] private Canvas _parentCanvas;
    [SerializeField] private float _maxOffset = 2f;
    [SerializeField] private float _followSpeed = 5f;
    [SerializeField] private float _eyelidsUpThreshold = 1f;
    [SerializeField] private float _timeToCenterEyes = 0.5f;

    private Vector2 _eyesCenterLocal;
    private GameObject _bubble;
    private bool _isWatching = false;

    private void Awake()
    {
        _eyesCenterLocal = _eyesRect.anchoredPosition;
        _eyelidsUpRect.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        BubbleSpawner.OnStartInflating += HandleStartInflating;
    }

    private void OnDisable()
    {
        BubbleSpawner.OnStartInflating -= HandleStartInflating;
    }

    private void HandleStartInflating(GameObject bubble)
    {
        _isWatching = true;
        _eyesRect.DOKill();
        _bubble = bubble;
    }

    private void Update()
    {
        if (!_isWatching) return;

        if (_bubble == null || (_bubble.GetComponent<Bubble>() != null && _bubble.GetComponent<Bubble>().IsSettled()))
        {
            _eyesRect.DOAnchorPos(_eyesCenterLocal, _timeToCenterEyes).SetEase(Ease.OutQuad);
            _isWatching = false;
            _eyelidsRect.gameObject.SetActive(true);
            _eyelidsUpRect.gameObject.SetActive(false);
            return;
        }

        Vector2 bubbleScreenPos = Camera.main.WorldToScreenPoint(_bubble.transform.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _parentCanvas.transform as RectTransform,
            bubbleScreenPos,
            _parentCanvas.worldCamera,
            out Vector2 bubbleLocalPos
        );

        Vector2 direction = (bubbleLocalPos - _eyesCenterLocal).normalized;
        Vector2 targetOffset = direction * _maxOffset;

        if (targetOffset.y > _eyelidsUpThreshold)
        {
            _eyelidsRect.gameObject.SetActive(false);
            _eyelidsUpRect.gameObject.SetActive(true);
        }
        else
        {
            _eyelidsRect.gameObject.SetActive(true);
            _eyelidsUpRect.gameObject.SetActive(false);
        }

        Vector2 newAnchoredPos = Vector2.Lerp(
            _eyesRect.anchoredPosition,
            _eyesCenterLocal + targetOffset,
            Time.deltaTime * _followSpeed
        );
        _eyesRect.anchoredPosition = newAnchoredPos;
    }
}
