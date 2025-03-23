using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Scripts
{
    internal class BubbleSpawner : SingletonBehaviour<BubbleSpawner>
    {
        static public event Action OnEmptyTank;

        public GameObject BubblePrefab;
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

            if ((bubble == null && transform.childCount < 2) && RemainingSoap > 0)
            {
                bubble = CreateBubble(GetNextColor());
                bubble.layer = LayerMask.NameToLayer("ShooterBubble");
            }

            if (canInflate && InputManager.InputHeld() && RemainingSoap > 0)
            {
                isInflating = true;
                lastRemainingSoap = RemainingSoap;
                InflateBubble();
            }

            bool hasBubbleReachedMaxSize = bubble != null && bubble.transform.localScale.x >= GameParameters.Instance.MaximalBubbleSize;
            bool isKeyReleased = InputManager.InputUp();
            if (isInflating && (isKeyReleased || hasBubbleReachedMaxSize))
            {
                if (hasBubbleReachedMaxSize) canInflate = false;
                DropBubble();
                if (lastRemainingSoap - RemainingSoap < minimumSoapConsumption)
                {
                    RemainingSoap = lastRemainingSoap - minimumSoapConsumption;
                }
            }
            if (RemainingSoap <= 0 && !emptyTankTriggered)
            {
                Debug.Log("Empty tank !");
                emptyTankTriggered = true;
                DropBubble();
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
            SFXManager.Instance.StartSound(SFXManager.Instance.bubbleInflatingSound, 0.5f, GameParameters.Instance.InflatingVolume);
            RemainingSoap -= GameParameters.Instance.SoapFlowRate * Time.deltaTime;
            var localScale = bubble.transform.localScale.x;
            if (localScale < GameParameters.Instance.MaximalBubbleSize)
            {
                var increasedScaleRation = localScale / localScaleBase;
                var multiplier = 0.75f / Mathf.Sqrt(increasedScaleRation);
                var diff = GameParameters.Instance.BubbleInflationRate * Time.deltaTime * multiplier;

                bubble.transform.localScale += diff * Vector3.one;
                var divider = localScale + diff > 1.5 && localScale + diff < 2.5 ? 2 : 4;
                bubble.transform.position += (diff * Vector3.down) / divider;
                bubble.GetComponent<Rigidbody2D>().mass = bubble.transform.localScale.x;
            }
        }

        private GameObject CreateBubble(Color color)
        {
            Debug.Log("Creating bubble with color " + color.ToString());
            // Vector2 newPos = new(0, GameParameters.Instance.InitialBubbleSize / -2);
            GameObject newBubble = Instantiate(BubblePrefab, transform.position, Quaternion.identity);
            newBubble.transform.parent = transform;
            newBubble.GetComponent<Rigidbody2D>().simulated = true;
            newBubble.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            newBubble.transform.localScale = new Vector2(GameParameters.Instance.InitialBubbleSize, GameParameters.Instance.InitialBubbleSize);

            Bubble bubbleComponent = newBubble.GetComponent<Bubble>();
            bubbleComponent.SetColor(color);

            bubbleComponent.OnBoundaryCollision += (boundary) =>
            {
                // ignore bottom border
                if (boundary.transform.position.y < -0.5f) return;
                if (newBubble.layer != LayerMask.NameToLayer("ShooterBubble")) return;

                Debug.Log("Drop on boundary !" + boundary + boundary.transform.position);
                DropBubble();
            };
            newBubble.GetComponent<Bubble>().OnBubblePopped += HandleBubblePopped;

            return newBubble;
        }

        private void HandleBubblePopped()
        {
            canInflate = false;
            isInflating = false;
        }

        private void DropBubble()
        {
            if (bubble != null) {
                bubble.GetComponent<Bubble>().OnBubblePopped -= HandleBubblePopped;
            }
            isInflating = false;
            SFXManager.Instance.StopSound();

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
            bubble = null;
        }

        private Color GetNextColor()
        {
            int colorIndex = UnityEngine.Random.Range(0, Math.Min(GameParameters.Instance.BubbleColorsCount, colors.Count));
            return colors[colorIndex];
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

        public void AddSoap(float amountToAdd, Action onComplete = null)
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
                GameParameters.Instance.DurationOfSoapRefill
            )
            .OnComplete(() => onComplete?.Invoke());
        }
    }
}
