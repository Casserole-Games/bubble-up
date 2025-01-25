using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Scripts
{
    internal class BubbleSpawner : MonoBehaviour
    {
        public GameObject BubblePrefab;
        public GameObject BubbleContainer;

        private GameObject bubble;

        List<Color> bubbleColors;

        void Start()
        {
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
            bubble = CreateBubble(Color.white);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Space key was pressed");
                bubble.transform.parent = BubbleContainer.transform;
                bubble.GetComponent<Rigidbody2D>().simulated = true;
                Color color = GetNextColor();
                bubble = CreateBubble(color);
            }
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

        private Color GetNextColor()
        {
            int colorIndex = Random.Range(0, bubbleColors.Count);

            Color color = bubbleColors[colorIndex];

            return color;
        }
    }
}