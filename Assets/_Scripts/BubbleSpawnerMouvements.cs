using UnityEngine;

namespace Assets._Scripts
{
    internal class BubbleSpawnerMouvements : MonoBehaviour
    {
        private float speed => GameParameters.Instance.BubbleSpawnerSpeed * direction;
        private int direction = 1;
        public float leftLimit = -5f;
        public float rightLimit = 5f;


        void Update()
        {
            transform.Translate(speed * Time.deltaTime * Vector3.right);
            if (transform.position.x >= rightLimit && speed > 0)
                direction *= -1;
            else if (transform.position.x <= leftLimit && speed < 0)
                direction *= -1;
        }

    }
}
