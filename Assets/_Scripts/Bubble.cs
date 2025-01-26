using System;
using UnityEngine;

namespace Assets._Scripts
{
    internal class Bubble : MonoBehaviour
    {
        public GameObject burstEffect;

        internal bool alreadyCollided = false;
        internal event Action<GameObject> OnBoundaryCollision;


        private Color color = Color.white;

        public void Pop()
        {
            Debug.Log("Starting popping!");
            if (this == null) return;
            burstEffect = Instantiate(burstEffect, transform.position, Quaternion.identity);
            // burstEffect.transform.localScale = transform.localScale / 4;

            ParticleSystem particleSystem = burstEffect.GetComponent<ParticleSystem>();
            ParticleSystem.MainModule main = particleSystem.main;
            main.startColor = color;
            main.startSize = Math.Min(transform.localScale.x / 8, 0.2f);

            ParticleSystem.ShapeModule shape = particleSystem.shape;
            shape.radius = transform.localScale.x / 4;

            ParticleSystem.MinMaxCurve count = particleSystem.emission.GetBurst(0).count;
            count.constant = (int)(transform.localScale.x * 100);

            Destroy(gameObject);

            MusicManager.Instance.PlaySound(MusicManager.Instance.bubblePopSound, 0.75f, 1.25f);
        }

        internal void SetColor(Color color)
        {
            this.color = color;
            GetComponent<SpriteRenderer>().color = color;
        }

        private void Merge(Bubble otherBubble, Vector3 pos)
        {
            Debug.Log("Merged!");
            MusicManager.Instance.PlaySound(MusicManager.Instance.bubbleMergeSound, 0.75f, 1.25f, 0.2f);
            // make an instance of itself
            Bubble newBubble = Instantiate(this, pos, Quaternion.identity);
            newBubble.SetColor(color);
            newBubble.alreadyCollided = true;

            float newScale = Mathf.Sqrt(transform.localScale.x * transform.localScale.x
                + otherBubble.transform.localScale.x * otherBubble.transform.localScale.x);

            newBubble.transform.localScale = new Vector3(newScale, newScale, 1);
            newBubble.GetComponent<Rigidbody2D>().mass = newScale;
            newBubble.transform.parent = transform.parent;

            Destroy(otherBubble.gameObject);
            Destroy(gameObject);
        }

        public void RemoveEvents()
        {
            OnBoundaryCollision = null;
        }

        void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.CompareTag("Boundary"))
            {
                OnBoundaryCollision?.Invoke(col.gameObject);
                RemoveEvents();
            }

            if (Camera.main.WorldToScreenPoint(col.transform.position).y <= HighFinder.Instance.globalHighestY)
            {
                alreadyCollided = true;
                RemoveEvents();
            }

            if (col.contacts.Length > 0 && col.gameObject.CompareTag("Bubble") && alreadyCollided)
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
