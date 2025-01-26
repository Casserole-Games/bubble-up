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
        public static float RemainingSoap;
        private float lastRemainingSoap;

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

        void Start()
        {
            Color color = new Color32(0, 0, 0, 0);
            RemainingSoap = 100f;
        }

        void Update()
        {
            if ((bubble == null || transform.childCount < 2) && RemainingSoap > 0)
            {
                bubble = CreateBubble(GetNextColor());
                bubble.layer = LayerMask.NameToLayer("ShooterBubble");
            }

            if (Input.GetKey(keyToDetect) && RemainingSoap > 0)
            {
                lastRemainingSoap = RemainingSoap;
                InflateBubble();
            }
            if (Input.GetKeyUp(keyToDetect) || bubble.transform.localScale.x >= GameParameters.Instance.MaximalBubbleSize)
            {
                DropBubble();
                if (lastRemainingSoap - RemainingSoap < 0.5f)
                {
                    RemainingSoap = lastRemainingSoap - 0.5f;
                }
            }
        }

        private void InflateBubble()
        {
            // Debug.Log("Inflate !");
            RemainingSoap -= GameParameters.Instance.SoapFlowRate * Time.deltaTime;
            var localScale = bubble.transform.localScale.x;
            if (localScale < GameParameters.Instance.MaximalBubbleSize)
            {
                var increasedScaleRation = localScale / localScaleBase;
                var multiplier = 1 / (increasedScaleRation * increasedScaleRation);
                var diff = GameParameters.Instance.BubbleInflationRate * Time.deltaTime * multiplier;

                bubble.transform.localScale += diff * Vector3.one;
                bubble.transform.position += (diff * Vector3.down) / 4;
                bubble.GetComponent<Rigidbody2D>().mass = bubble.transform.localScale.x;
            }

            int newSoapValue = (int)RemainingSoap;
            UIManager.Instance.SetTankValue(newSoapValue);
        }

        private GameObject CreateBubble(Color color)
        {
            Debug.Log("Creating bubble with color " + color.ToString());
            Vector2 newPos = new(0, GameParameters.Instance.InitialBubbleSize / -2);
            GameObject newBubble = Instantiate(BubblePrefab, newPos, Quaternion.identity);
            newBubble.transform.SetParent(transform, false);
            newBubble.GetComponent<Rigidbody2D>().simulated = true;
            newBubble.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            newBubble.transform.localScale = new Vector2(GameParameters.Instance.InitialBubbleSize, GameParameters.Instance.InitialBubbleSize);

            Bubble bubbleComponent = newBubble.GetComponent<Bubble>();
            bubbleComponent.SetColor(color);

            bubbleComponent.OnBoundaryCollision += (boundary) =>
            {
                // ignore bottom border
                if (boundary.transform.position.y < minimumSoapConsumption) return;

                DropBubble();
            };

            return newBubble;
        }

        private void DropBubble()
        {
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
    }
}