using UnityEngine;

namespace Assets._Scripts
{
    internal class BubbleSpawner : MonoBehaviour
    {
        public GameObject BubblePrefab;
        public GameObject BubbleContainer;

        private GameObject bubble;

        void Start()
        {
            bubble = CreateBubble();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Space key was pressed");
                bubble.transform.parent = BubbleContainer.transform;
                bubble.GetComponent<Rigidbody2D>().simulated = true;
                bubble = CreateBubble();
            }
        }

        private GameObject CreateBubble()
        {
            GameObject bubble = Instantiate(BubblePrefab, transform.position, Quaternion.identity);
            bubble.transform.parent = transform;
            bubble.GetComponent<Rigidbody2D>().simulated = false;
            return bubble;
        }
    }
}