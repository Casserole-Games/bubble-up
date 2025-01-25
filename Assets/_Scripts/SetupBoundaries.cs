using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets._Scripts
{
    internal class SetupBoundaries : MonoBehaviour
    {
        public GameObject boundaryPrefab;

        void Start()
        {
            GameObject boundaries = new("Boundaries");

            Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector2(0, 0));
            Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
            {
                GameObject left = Instantiate(boundaryPrefab);
                left.transform.parent = boundaries.transform;
                left.layer = LayerMask.NameToLayer("Boundaries");
                BoxCollider2D collider = left.GetOrAddComponent<BoxCollider2D>();

                // set height to match the size of the screen
                collider.size = new Vector2(1f, Math.Abs(topRight.y - bottomLeft.y) + 1);

                left.transform.position = new Vector3(bottomLeft.x - (collider.size.x / 2), 0, 0);
            }

            {
                GameObject right = Instantiate(boundaryPrefab);
                right.transform.parent = boundaries.transform;
                right.layer = LayerMask.NameToLayer("Boundaries");
                BoxCollider2D collider = right.GetOrAddComponent<BoxCollider2D>();

                // set height to match the size of the screen
                collider.size = new Vector2(1f, Math.Abs(topRight.y - bottomLeft.y) + 1);

                right.transform.position = new Vector3(topRight.x + (collider.size.x / 2), 0, 0);
            }

            {
                GameObject bottom = Instantiate(boundaryPrefab);
                bottom.transform.parent = boundaries.transform;
                bottom.layer = LayerMask.NameToLayer("Boundaries");
                BoxCollider2D collider = bottom.GetOrAddComponent<BoxCollider2D>();
                collider.sharedMaterial = new PhysicsMaterial2D(collider.sharedMaterial.name)
                {
                    bounciness = 0.2f
                };

                // set width to match the size of the screen
                collider.size = new Vector2(Math.Abs(bottomLeft.x) * 2 + 1, 1f);

                bottom.transform.position = new Vector3(0, bottomLeft.y - (collider.size.y / 2), 0);
            }
        }
    }
}
