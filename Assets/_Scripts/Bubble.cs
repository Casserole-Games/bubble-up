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
        
        private void Merge(Bubble otherBubble)
        {
            transform.localScale += otherBubble.transform.localScale; 
            Destroy(otherBubble.gameObject);
        }

        void OnCollisionEnter2D(Collision2D col)
        {
            if (Camera.main.WorldToScreenPoint(col.transform.position).y <= HighFinder.Instance.globalHighestY) {
                alreadyCollided = true;
            }

            Bubble bubble = col.gameObject.GetComponent<Bubble>();
            if (bubble == null) return;

            if (bubble.color == color)
            {
                Merge(bubble);
            }
        }
    }
}
