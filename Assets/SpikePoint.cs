using Assets._Scripts;
using UnityEngine;

public class SpikePoint : MonoBehaviour
{
    private Collider2D pointyCollider;
    

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Spike pointy end: Collision");
        if (collision.gameObject.CompareTag("Bubble"))
        {
            Debug.Log("Spike pointy end: Collision with bubble");
            collision.gameObject.GetComponent<Bubble>().Pop();
        }
    }
}
