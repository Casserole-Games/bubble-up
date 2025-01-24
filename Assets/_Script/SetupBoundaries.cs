using System;
using UnityEngine;

public class SetupBoundaries : MonoBehaviour
{
    void Start()
    {
        GameObject boundaries = new("Boundaries");

        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector2(0, 0));
        Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        {
            GameObject left = new("Left");
            left.transform.parent = boundaries.transform;
            BoxCollider2D collider = left.AddComponent<BoxCollider2D>();

            // set height to match the size of the screen
            collider.size = new Vector2(1f, Math.Abs(topRight.y - bottomLeft.y) + 1);

            left.transform.position = new Vector3(bottomLeft.x - (collider.size.x / 2), 0, 0);
        }

        {
            GameObject right = new("Right");
            right.transform.parent = boundaries.transform;
            BoxCollider2D collider = right.AddComponent<BoxCollider2D>();

            // set height to match the size of the screen
            collider.size = new Vector2(1f, Math.Abs(topRight.y - bottomLeft.y) + 1);

            right.transform.position = new Vector3(topRight.x + (collider.size.x / 2), 0, 0);
        }

        {
            GameObject bottom = new("Bottom");
            bottom.transform.parent = boundaries.transform;
            BoxCollider2D collider = bottom.AddComponent<BoxCollider2D>();

            // set width to match the size of the screen
            collider.size = new Vector2(Math.Abs(bottomLeft.x) * 2 + 1, 1f);

            bottom.transform.position = new Vector3(0, bottomLeft.y - (collider.size.y / 2), 0);
        }
    }
}
