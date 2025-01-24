using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Script
{
    internal class BubbleSpawnerMouvements : MonoBehaviour
    {
        public float speed = 5f;
        public float leftLimit = -5f;
        public float rightLimit = 5f;

        private bool movingRight = true;

        void Update()
        {
            // Vérifier la direction et déplacer le canon
            if (movingRight)
            {
                transform.Translate(Vector3.right * speed * Time.deltaTime);
                if (transform.position.x >= rightLimit)
                    movingRight = false;
            }
            else
            {
                transform.Translate(Vector3.left * speed * Time.deltaTime);
                if (transform.position.x <= leftLimit)
                    movingRight = true;
            }
        }

    }
}
