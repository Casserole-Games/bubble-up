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
        
        private void Merge(Bubble otherBubble, Vector3 pos)
        {
            // make an instance of itself
            Bubble newBubble = Instantiate(this, pos, Quaternion.identity);
            newBubble.SetColor(color);
            newBubble.alreadyCollided = true;

            float newScale = Mathf.Sqrt(transform.localScale.x * transform.localScale.x 
                + otherBubble.transform.localScale.x * otherBubble.transform.localScale.x);

            newBubble.transform.localScale = new Vector3(newScale, newScale, 1);
            newBubble.transform.parent = transform.parent;

            Destroy(otherBubble.gameObject);
            Destroy(gameObject);
        }

        void OnCollisionEnter2D(Collision2D col)
        {
            if (Camera.main.WorldToScreenPoint(col.transform.position).y <= HighFinder.Instance.globalHighestY) {
                alreadyCollided = true;
            }

            if (col.gameObject.CompareTag("Bubble"))
            {
                Bubble bubble = col.gameObject.GetComponent<Bubble>();
                if (bubble == null) return;

                var newPos = (transform.position + col.gameObject.transform.position) / 2;

                // don't call it on both bubbles
                if (bubble.color == color && col.transform.position.y >= transform.position.y)
                {
                    Merge(bubble, newPos);
                }
            }
        }
    }
}
