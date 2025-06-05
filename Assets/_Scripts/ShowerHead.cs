using Assets._Scripts;
using UnityEngine;

public class ShowerHead : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bubble") && collision.gameObject.GetComponent<Bubble>().alreadyCollided)
        {
            collision.gameObject.GetComponent<Bubble>().Pop();
        }
    }
}
