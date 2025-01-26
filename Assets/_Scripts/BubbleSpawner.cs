using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Scripts
{
    internal class BubbleSpawner : MonoBehaviour
    {
        public GameObject BubblePrefab;
        public GameObject BubbleContainer;

        private KeyCode keyToDetect = KeyCode.Space;

        private const float localScaleBase = 0.25f;

        private float minimumSoapConsumption => GameParameters.Instance.BubbleMinimumSoapConsumption;

        private GameObject bubble;
        private float lastRemainingSoap;
        private bool isInflating = false;

        public static float RemainingSoap;
        public bool IsPaused = false;
        public bool IsGameOver = false;

        private static List<Color> colors = new(){
            new Color32(0x4C, 0xBF, 0xFF, 0xFF), // Cyan
            new Color32(0xFD, 0xFE, 0xFF, 0xFF), // White
            new Color32(0xB0, 0xFF, 0xDC, 0xFF), // Green
            new Color32(0xFF, 0x42, 0x45, 0xFF), // Red
            new Color32(0xFF, 0x97, 0x8B, 0xFF), // Orange

            // new Color32(0x00, 0x6D, 0xD2, 0xFF), // Blue
            // new Color32(0xFF, 0xC4, 0x82, 0xFF), // Yellow
            // new Color32(0xFF, 0xB0, 0xD3, 0xFF), // Pink
        };
        private bool emptyTankTriggered;

        public static BubbleSpawner Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        void Start()
        {
            RemainingSoap = GameParameters.Instance.StartSoapAmount;
            UIManager.Instance.SetTankValue((int)RemainingSoap);
        }

        void Update()
        {
            if (IsPaused || IsGameOver)
            {
                if (Input.GetKeyUp(keyToDetect))
                {
                    UIManager.Instance.ButtonPressed();
                }
                return;
            }

            if ((bubble == null || transform.childCount < 2) && RemainingSoap > 0)
            {
                bubble = CreateBubble(GetNextColor());
                bubble.layer = LayerMask.NameToLayer("ShooterBubble");
            }

            if (Input.GetKey(keyToDetect) && RemainingSoap > 0)
            {
                isInflating = true;
                lastRemainingSoap = RemainingSoap;
                InflateBubble();
            }
            if (Input.GetKeyUp(keyToDetect) || (bubble != null && bubble.transform.localScale.x >= GameParameters.Instance.MaximalBubbleSize))
            {
                isInflating = false;
                DropBubble();
                if (lastRemainingSoap - RemainingSoap < minimumSoapConsumption)
                {
                    RemainingSoap = lastRemainingSoap - minimumSoapConsumption;
                }
            }
            if (RemainingSoap <= 0 && !emptyTankTriggered && !isInflating)
            {
                Debug.Log("Empty tank !");
                emptyTankTriggered = true;
                GameManager.Instance.TriggerEmptyTank();
            }
        }

        public void ResumeGame()
        {
            emptyTankTriggered = false;
            IsPaused = false;
        }

        private void InflateBubble()
        {
            MusicManager.Instance.StartMusic(MusicManager.Instance.bubbleInflatingSound, 0.5f);
            // Debug.Log("Inflate !");
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

            int newSoapValue = (int)RemainingSoap;
            UIManager.Instance.SetTankValue(newSoapValue);
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

            return newBubble;
        }

        private void DropBubble()
        {
            MusicManager.Instance.StopMusic();
            MusicManager.Instance.PlaySound(MusicManager.Instance.bubbleDropSound, 0.75f, 1.25f);
            // simulate gravity
            Debug.Log("Drop !");
            bubble.transform.parent = BubbleContainer.transform;
            bubble.layer = LayerMask.NameToLayer("Bubble");

            Rigidbody2D rb = bubble.GetComponent<Rigidbody2D>();
            rb.simulated = true;
            rb.constraints = RigidbodyConstraints2D.None;

            float baseSpeed = GameParameters.Instance.BubbleFlowSpeed;
            baseSpeed *= (float)Math.Sqrt(rb.mass);
            float appliedSpeed = Mathf.Min(baseSpeed, GameParameters.Instance.GunLatSpeed);
            rb.AddForce(new Vector2(appliedSpeed * BubbleSpawnerMouvements.direction, -0.5f), ForceMode2D.Impulse);
        }

        private Color GetNextColor()
        {
            int colorIndex = UnityEngine.Random.Range(0, Math.Min(GameParameters.Instance.BubbleColorsCount, colors.Count));
            return colors[colorIndex];
        }

        internal void PauseGame()
        {
            IsPaused = true;
        }
    }
}