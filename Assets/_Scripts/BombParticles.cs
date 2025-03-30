using Assets._Scripts;
using UnityEngine;

public class BombParticles : MonoBehaviour
{
    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Bubble"))
        {
            other.GetComponent<Bubble>().Pop();
        }
    }
}
