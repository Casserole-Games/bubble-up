using UnityEngine;

namespace Assets._Scripts
{
    internal class BubbleSpawnerMouvements : MonoBehaviour
    {
        public float speed = 5f;
        public float leftLimit = -5f;
        public float rightLimit = 5f;

        void Update()
        {
            transform.Translate(speed * Time.deltaTime * Vector3.right);
            if (transform.position.x >= rightLimit)
                speed *= -1;
            else if (transform.position.x <= leftLimit)
                speed *= -1;
        }

    }
}
