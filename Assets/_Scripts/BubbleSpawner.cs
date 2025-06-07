using DG.Tweening;
using GameAnalyticsSDK;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Scripts
{
    internal class BubbleSpawner : SingletonBehaviour<BubbleSpawner>
    {
        static public event Action OnEmptyTank;
        static public event Action OnTooManyShortHolds;
        static public event Action OnLongHold;
        static public event Action<GameObject> OnStartInflating;

        public GameObject BubblePrefab;
        public GameObject BombPrefab;
        public GameObject BubbleContainer;

        private const float localScaleBase = 0.25f;

        private float minimumSoapConsumption => GameParameters.Instance.BubbleMinimumSoapConsumption;

        private GameObject bubble;
        private float lastRemainingSoap;
        private bool isInflating = false;
        private bool canInflate = true;
        private bool isUnpausingOnKeyRelease = false;

        private static float _RemainingSoap = 1;
        public static float RemainingSoap
        {
            get { return _RemainingSoap; }
            set { 
                UIManager.Instance.SetTankValue((int)value);
                _RemainingSoap = value;
             }
        }
        public bool IsSpawnerPaused = false;
        public bool IsGameOver = false;
        public bool CanFinishPhase = true;
        public bool SpawnBomb = false;

        public Vector3 SpawnPointOffset = Vector3.zero;

        private static List<Color> colors = new(){
            new Color32(0x4C, 0xBF, 0xFF, 0xFF), // Cyan
            //new Color32(0xFD, 0xFE, 0xFF, 0xFF), // White
            new Color32(0xB0, 0xFF, 0xDC, 0xFF), // Green
            new Color32(0xFF, 0x42, 0x45, 0xFF), // Red
            //new Color32(0xFF, 0x97, 0x8B, 0xFF), // Orange
            new Color32(0xC8, 0xA2, 0xFF, 0xFF),  // Soft Lavender

            // new Color32(0x00, 0x6D, 0xD2, 0xFF), // Blue
            // new Color32(0xFF, 0xC4, 0x82, 0xFF), // Yellow
            // new Color32(0xFF, 0xB0, 0xD3, 0xFF), // Pink
        };
        private bool emptyTankTriggered;

        private float _holdStartTime;
        private int _shortHoldsCounter;
        private bool _holdHintShowed = false;

        void Start()
        {
            RemainingSoap = GameParameters.Instance.StartSoapAmount;
        }

        void Update()
        {
            if (GameManager.Instance.GameState != GameState.Phase1 && GameManager.Instance.GameState != GameState.Phase2) return;

            if (IsSpawnerPaused && isUnpausingOnKeyRelease && InputManager.InputUp())
            {
                IsSpawnerPaused = false;
                isUnpausingOnKeyRelease = false;
            }

            if (IsSpawnerPaused || IsGameOver)
            {
                if (InputManager.InputUp())
                {
                    UIManager.Instance.ButtonPressed();
                }
                return;
            }

            if (!IsSpawnerPaused && (bubble == null && transform.childCount < 2 && canInflate) && RemainingSoap > 0)
            {
                if (!SpawnBomb)
                {
                    bubble = CreateBubble(GetNextColor());
                } else
                {
                    bubble = CreateBomb();
                }
                bubble.layer = LayerMask.NameToLayer("ShooterBubble");
            }

            if (canInflate && InputManager.InputHeld() && RemainingSoap > 0)
            {
                if (!isInflating)
                {
                    OnStartInflating?.Invoke(bubble);
                }

                isInflating = true;
                lastRemainingSoap = RemainingSoap;
                InflateBubble();

                if (_holdStartTime <= 0f)
                    _holdStartTime = Time.time;
            }

            bool hasBubbleReachedMaxSize = bubble != null && bubble.transform.localScale.x >= GameParameters.Instance.MaximalBubbleSize;
            bool isKeyReleased = InputManager.InputUp();
            if (isInflating && bubble != null && (isKeyReleased || hasBubbleReachedMaxSize))
            {
                float holdDuration = Time.time - _holdStartTime;
                _holdStartTime = 0f;

                if ((GameManager.Instance.GameState != GameState.Phase2) && (holdDuration < GameParameters.Instance.ShortHoldThresholdSeconds))
                {
                    _shortHoldsCounter++;
                    if ((!_holdHintShowed && (_shortHoldsCounter >= GameParameters.Instance.ShortHoldsToHint)) ||
                        (_holdHintShowed && (_shortHoldsCounter >= GameParameters.Instance.ShortHoldsToRepeatHint)))
                    {
                        OnTooManyShortHolds?.Invoke();
                        _shortHoldsCounter = 0;
                    }
                }
                else
                {
                    if (holdDuration >= GameParameters.Instance.LongHoldThresholdSeconds)
                    {
                        OnLongHold?.Invoke();
                        _holdHintShowed = true;
                    }
                    _shortHoldsCounter = 0;
                }

                if (hasBubbleReachedMaxSize) canInflate = false;
                if (!bubble.CompareTag("Bomb")) DropBubble(); else DropBomb();
                if (lastRemainingSoap - RemainingSoap < minimumSoapConsumption)
                {
                    RemainingSoap = lastRemainingSoap - minimumSoapConsumption;
                }
            }
            if (RemainingSoap <= 0 && !emptyTankTriggered)
            {
                Debug.Log("Empty tank !");
                emptyTankTriggered = true;
                if (bubble != null && !bubble.CompareTag("Bomb")) DropBubble(); else DropBomb();
            }
            if (CanFinishPhase && emptyTankTriggered && HighFinder.Instance.AreBubblesSettled()) 
            {
                if (RemainingSoap <= 0) // case with getting additional soap with last bubble
                {
                    StartCoroutine("InvokeEmptyTank"); 
                } else
                {
                    emptyTankTriggered = false;
                }
            }
            if (isKeyReleased) canInflate = true;
        }

        private IEnumerator InvokeEmptyTank()
        {
            yield return new WaitForSeconds(0.5f);
            HighFinder.Instance.Calculate();
            OnEmptyTank?.Invoke();
        }

        private void InflateBubble()
        {
            SFXManager.Instance.PlayMain("inflating", GameParameters.Instance.InflatingVolume, 0.5f, 0.5f);
            RemainingSoap -= GameParameters.Instance.SoapFlowRate * Time.deltaTime;
            var localScale = bubble.transform.localScale.x;
            if (localScale < GameParameters.Instance.MaximalBubbleSize)
            {
                var increasedScaleRation = localScale / localScaleBase;
                var multiplier = 0.75f / Mathf.Sqrt(increasedScaleRation);
                var diff = GameParameters.Instance.BubbleInflationRate * Time.deltaTime * multiplier;

                bubble.transform.localScale += diff * Vector3.one;
                var divider = localScale + diff > 1.5 && localScale + diff < 2.5 ? 2 : 3.5f;
                bubble.transform.position += (diff * Vector3.down) / divider;
                bubble.GetComponent<Rigidbody2D>().mass = bubble.transform.localScale.x;
            }
        }

        private GameObject CreateBubble(Color color)
        {
            Debug.Log("Creating bubble with color " + color.ToString());
            GameObject newBubble = Instantiate(BubblePrefab, transform.position + SpawnPointOffset, Quaternion.identity);
            newBubble.transform.parent = transform;
            newBubble.GetComponent<Rigidbody2D>().simulated = true;
            newBubble.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            newBubble.transform.localScale = new Vector2(GameParameters.Instance.InitialBubbleSize, GameParameters.Instance.InitialBubbleSize);

            Bubble bubbleComponent = newBubble.GetComponent<Bubble>();
            bubbleComponent.SetColor(color);
            bubbleComponent.OnBubblePopped += HandleBubblePop;

            return newBubble;
        }

        private void HandleBubblePop()
        {
            canInflate = false;
            SFXManager.Instance.StopMain();
            bubble = null;
            isInflating = false;
        }

        private void DropBubble()
        {
            isInflating = false;
            SFXManager.Instance.StopMain();

            // simulate gravity
            Debug.Log("Drop !");
            if (!bubble) return; // bugfix
            bubble.transform.parent = BubbleContainer.transform;
            bubble.layer = LayerMask.NameToLayer("Bubble");

            Rigidbody2D rb = bubble.GetComponent<Rigidbody2D>();
            rb.simulated = true;
            rb.constraints = RigidbodyConstraints2D.None;

            float baseSpeed = GameParameters.Instance.BubbleFlowSpeed;
            baseSpeed *= (float)Math.Sqrt(rb.mass);
            float appliedSpeed = Mathf.Min(baseSpeed, GameParameters.Instance.GunLatSpeed);
            rb.AddForce(new Vector2(appliedSpeed * BubbleSpawnerMouvements.direction, -0.5f), ForceMode2D.Impulse);
            bubble.GetComponent<Bubble>().alreadyDropped = true;
            bubble.GetComponent<Bubble>().OnBubblePopped -= HandleBubblePop;
            bubble = null;
        }

        private Color GetNextColor()
        {
            int colorIndex = 0;

            if (GameManager.Instance.GameState == GameState.Phase2)
            {
                // collect used colors
                HashSet<Color> usedColorsHash = new HashSet<Color>();
                foreach (Transform bubbleTransform in BubbleContainer.transform)
                {
                    Bubble bubble = bubbleTransform.GetComponent<Bubble>();
                    if (bubble == null) continue;
                    usedColorsHash.Add(bubble.Color);
                }
                
                if (usedColorsHash.Count > 0)
                {
                    List<Color> usedColors = new List<Color>(usedColorsHash);
                    return usedColors[UnityEngine.Random.Range(0, usedColors.Count)];
                } else
                {
                    return Color.white;
                }

            } else
            {
                colorIndex = UnityEngine.Random.Range(0, Math.Min(GameParameters.Instance.BubbleColorsCount, colors.Count));
                return colors[colorIndex];
            }
        }

        internal void PauseSpawner()
        {
            IsSpawnerPaused = true;
        }
        public void ResumeSpawner()
        {
            emptyTankTriggered = false;
            isUnpausingOnKeyRelease = true;
        }

        public void AddSoap(float amountToAdd, float duration, Action onComplete = null)
        {
            float oldValue = 0f;

            float maxSoapAmount = GameParameters.Instance.MaxSoapAmount;
            float targetAmount = Mathf.Min(RemainingSoap + amountToAdd, maxSoapAmount);
            float amountToAddLimited = targetAmount - RemainingSoap;

            DOTween.To(
                () => 0f,
                currentValue =>
                {
                    float delta = currentValue - oldValue;
                    oldValue = currentValue;
                    RemainingSoap += delta;
                },
                amountToAddLimited,
                duration
            )
            .OnComplete(() => onComplete?.Invoke());
        }

        private GameObject CreateBomb()
        {
            GameObject bomb = Instantiate(BombPrefab, transform.position + new Vector3(-0.23f, -0.09f, 0f), Quaternion.Euler(0f, 0f, -90f));
            bomb.transform.parent = transform;
            bomb.GetComponent<Rigidbody2D>().simulated = true;
            bomb.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;

            return bomb;
        }

        private void DropBomb()
        {
            isInflating = false;
            SFXManager.Instance.StopMain();

            // simulate gravity
            Debug.Log("Drop !");
            if (!bubble) return; // bugfix
            bubble.transform.parent = BubbleContainer.transform;
            bubble.layer = LayerMask.NameToLayer("Bubble");

            Rigidbody2D rb = bubble.GetComponent<Rigidbody2D>();
            rb.simulated = true;
            rb.constraints = RigidbodyConstraints2D.None;

            float baseSpeed = GameParameters.Instance.BubbleFlowSpeed;
            baseSpeed *= (float)Math.Sqrt(rb.mass);
            float appliedSpeed = Mathf.Min(baseSpeed, GameParameters.Instance.GunLatSpeed);
            rb.AddForce(new Vector2(appliedSpeed * BubbleSpawnerMouvements.direction, -0.5f), ForceMode2D.Impulse);
            StartCoroutine(ExplodeBomb(bubble));
            bubble = null;
            PauseSpawner();
        }

        private IEnumerator ExplodeBomb(GameObject bomb)
        {
            bomb.GetComponent<Rigidbody2D>().AddTorque(-100f);
            SFXManager.Instance.PlayMain("fuse", GameParameters.Instance.FuseVolume);
            yield return new WaitForSeconds(0.65f);
            SFXManager.Instance.StopMain();
            SFXManager.Instance.PlayOneShot("bomb", GameParameters.Instance.BombVolume);
            bomb.GetComponent<Animator>().Play("bomb_exploding");
            yield return new WaitForSeconds(0.15f);
            bomb.GetComponent<SpriteRenderer>().enabled = false;

            ParticleSystem ps = bomb.GetComponentInChildren<ParticleSystem>();
            ps.Play();
            Camera.main.GetComponentInParent<CameraShake>().Shake();
            yield return new WaitWhile(() => ps.isPlaying);

            Destroy(bomb);
        }
    }
}
