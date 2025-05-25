using System;
using UnityEngine;

namespace Assets._Scripts
{
    internal class Bubble : MonoBehaviour
    {
        public event Action OnBubblePopped;

        public GameObject burstEffect;

        public bool alreadyCollided;
        public bool alreadyDropped;
        public event Action<GameObject> OnBoundaryCollision;

        private Color _color;
        public Color Color
        {
            get { return _color; }
            set { SetColor(value); }
        }

        private Rigidbody2D _rb;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        public bool IsSettled() {
            if (_rb == null) return false;
            return Math.Abs(_rb.linearVelocityY) < 0.1f; 
        }

        public void Pop()
        {
            Debug.Log("Starting popping!");
            if (this == null) return;
            burstEffect = Instantiate(burstEffect, transform.position, Quaternion.identity);

            ParticleSystem particleSystem = burstEffect.GetComponent<ParticleSystem>();
            ParticleSystem.MainModule main = particleSystem.main;
            main.startColor = _color;
            main.startSize = Math.Min(transform.localScale.x / 8, 0.2f);

            ParticleSystem.ShapeModule shape = particleSystem.shape;
            shape.radius = transform.localScale.x / 4;

            ParticleSystem.MinMaxCurve count = particleSystem.emission.GetBurst(0).count;
            count.constant = (int)(transform.localScale.x * 100);

            OnBubblePopped?.Invoke();
            Destroy(gameObject);

            SFXManager.Instance.PlayOneShot("pop", GameParameters.Instance.PopVolume, 0.75f, 1.25f);
        }

        internal void SetColor(Color color)
        {
            this._color = color;
            GetComponent<SpriteRenderer>().color = color;
        }

        private void Merge(Bubble otherBubble, Vector3 pos)
        {
            Debug.Log("Merged!");
            SFXManager.Instance.PlayOneShot("merge", GameParameters.Instance.MergeVolume, 0.90f, 1.10f);
            // make an instance of itself
            Bubble newBubble = Instantiate(this, pos, Quaternion.identity);
            newBubble.SetColor(Color);
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
            // touch the bottom
            if (col.gameObject.CompareTag("Bottom") && !alreadyCollided)
            {
                alreadyCollided = true;
                RemoveEvents();
            }

            // touch another bubble
            if (col.gameObject.CompareTag("Bubble"))
            {
                Bubble otherBubble = col.gameObject.GetComponent<Bubble>();
                if (otherBubble != null)
                {
                    if (otherBubble.alreadyCollided && !alreadyCollided)
                    {
                        alreadyCollided = true;
                        RemoveEvents();
                    }

                    if (alreadyCollided && alreadyDropped && otherBubble.alreadyDropped)
                    {
                        if (otherBubble.Color == Color && col.transform.position.y >= transform.position.y)
                        {
                            var newPos = new Vector2(
                                (transform.position.x + col.gameObject.transform.position.x) / 2,
                                Math.Min(transform.position.y, col.gameObject.transform.position.y)
                            );
                            Merge(otherBubble, newPos);
                        }
                    }
                }
            }
        }
    }
}
