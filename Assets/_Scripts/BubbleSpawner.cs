using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Scripts
{
    internal class BubbleSpawner : MonoBehaviour
    {
        public GameObject BubblePrefab;
        public GameObject BubbleContainer;
        public KeyCode keyToDetect = KeyCode.Space;

        private GameObject bubble;

        private float remainingSoap;

        private static List<Color> colors = new List<Color>(){
            Color.red,
            Color.green,
            Color.blue,
            Color.yellow,
            Color.cyan,
            Color.magenta,
            Color.grey,
            Color.black
        };

        void Start()
        {
            remainingSoap = 100f;
        }

        void Update()
        {
            if ((bubble == null || transform.childCount < 2) && remainingSoap > 0)
            {
                bubble = CreateBubble(GetNextColor());
                bubble.layer = LayerMask.NameToLayer("ShooterBubble");
            }

            if (Input.GetKey(keyToDetect) && remainingSoap > 0)
            {
                InflateBubble();
            }
            else if (Input.GetKeyUp(keyToDetect))
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
        }

        private void InflateBubble()
        {
            Debug.Log("Inflate !");
            remainingSoap -= GameParameters.Instance.SoapFlowRate * Time.deltaTime;
            if (bubble.transform.localScale.x < GameParameters.Instance.MaximalBubbleSize)
            {
                var diff = GameParameters.Instance.BubbleInflationRate * Time.deltaTime;
                bubble.transform.position += (diff * Vector3.down) / 2;
                bubble.transform.localScale += diff * Vector3.one;
                bubble.GetComponent<Rigidbody2D>().mass = bubble.transform.localScale.x;
            }

            int newSoapValue = (int)remainingSoap;
            UIManager.Instance.SetTankValue(newSoapValue);
        }

        private GameObject CreateBubble(Color color)
        {
            Debug.Log("Creating bubble with color " + color.ToString());
            Vector2 newPos = new(0, GameParameters.Instance.InitialBubbleSize / -2);
            GameObject newBubble = Instantiate(BubblePrefab, newPos, Quaternion.identity);
            newBubble.transform.SetParent(transform, false);
            newBubble.GetComponent<Rigidbody2D>().simulated = false;
            newBubble.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            newBubble.transform.localScale = new Vector2(GameParameters.Instance.InitialBubbleSize, GameParameters.Instance.InitialBubbleSize);

            Bubble bubbleComponent = newBubble.GetComponent<Bubble>();
            bubbleComponent.SetColor(color);

            return newBubble;
        }

        private Color GetNextColor()
        {
            int colorIndex = UnityEngine.Random.Range(0, Math.Min(GameParameters.Instance.BubbleColorsCount, colors.Count));
            return colors[colorIndex];
        }
    }
}