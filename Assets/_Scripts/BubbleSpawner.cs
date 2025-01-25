using System.Collections.Generic;
using UnityEngine;

namespace Assets._Scripts
{
    internal class BubbleSpawner : MonoBehaviour
    {
        public GameObject BubblePrefab;
        public GameObject BubbleContainer;

        private GameObject bubble;

        private float initialBubbleSize;
        private float inflatedBubbleSize;
        private float soapFlowRate;

        List<Color> bubbleColors;

        private float remainingSoap;

        public KeyCode keyToDetect = KeyCode.Space;

        private bool isKeyPressed = false;
        private float bubbleInflationRate;
        private int colorCount;

        void Start()
        {
            initialBubbleSize = GameParameters.Instance.InitialBubbleSize;
            soapFlowRate = GameParameters.Instance.SoapFlowRate;
            bubbleInflationRate = GameParameters.Instance.BubbleInflationRate;
            colorCount = GameParameters.Instance.BubbleColorsCount;

            remainingSoap = 100f;

            bubbleColors = new List<Color>()
            {
                Color.red,
                Color.green,
                Color.blue,
                Color.yellow,
                Color.cyan,
                Color.magenta,
                Color.grey,
                Color.black
            };
            inflatedBubbleSize = initialBubbleSize;
            bubble = CreateBubble(GetNextColor(), inflatedBubbleSize);
        }


        void Update()
        {
            if (Input.GetKey(keyToDetect) && remainingSoap > 0)
            {
                if (!isKeyPressed)
                {
                    isKeyPressed = true;
                }
                InflateBubble();
            }

            if (isKeyPressed && Input.GetKeyUp(keyToDetect))
            {
                // simulate gravity
                Debug.Log("Drop !");
                bubble.transform.SetParent(BubbleContainer.transform);
                bubble.GetComponent<Rigidbody2D>().simulated = true;
                
                // create new bubble
                if (remainingSoap > 0)
                {
                    Color color = GetNextColor();
                    inflatedBubbleSize = initialBubbleSize;
                    bubble = CreateBubble(color, inflatedBubbleSize);
                    bubble.transform.parent = transform;
                }
            }
        }

        private void InflateBubble()
        {
            Debug.Log("Inflate !");
            remainingSoap -= soapFlowRate * Time.deltaTime;
            inflatedBubbleSize += bubbleInflationRate * Time.deltaTime;
            bubble.transform.localScale = new Vector3(inflatedBubbleSize, inflatedBubbleSize, 1);

            int newSoapValue = (int)remainingSoap;
            GameManager.Instance.SetTankValue(newSoapValue);
        }

        private GameObject CreateBubble(Color color)
        {
            Debug.Log("Creating bubble with color " + color.ToString());
            GameObject bubble = Instantiate(BubblePrefab, transform.position, Quaternion.identity);
            bubble.transform.parent = transform;
            bubble.GetComponent<Rigidbody2D>().simulated = false;
            Bubble bubbleComponent = bubble.GetComponent<Bubble>();
            bubbleComponent.SetColor(color);
            return bubble;
        }

        private GameObject CreateBubble(Color color, float size)
        {
            GameObject bubble = CreateBubble(color);
            bubble.transform.localScale = new Vector3(size, size, 1);
            return bubble;
        }

        private Color GetNextColor()
        {
            int colorIndex = Random.Range(0, colorCount);

            Color color = bubbleColors[colorIndex];

            return color;
        }
    }
}