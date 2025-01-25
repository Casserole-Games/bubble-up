using UnityEngine;

namespace Assets._Scripts
{
    internal class Bubble : MonoBehaviour
    {
        internal bool alreadyCollided = false;

        private Color color = Color.white;

        public void Pop()
        {
            Debug.Log("Popped!");
            Destroy(gameObject);
        }

        internal void SetColor(Color color)
        {
            this.color = color;
            GetComponent<SpriteRenderer>().color = color;
        }

        void OnCollisionEnter2D(Collision2D col)
        {
            alreadyCollided = true;

            Bubble bubble = col.gameObject.GetComponent<Bubble>();
            if (bubble == null) return;
        }
    }
}
