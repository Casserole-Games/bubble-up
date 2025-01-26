using UnityEngine;

namespace Assets._Scripts
{
    public class Soap : MonoBehaviour
    {
        void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.tag != "Bubble") return;
            if (HighFinder.Instance.localHighestY < 850) return;

            BubbleSpawner.RemainingSoap = 100f;
            UIManager.Instance.SetTankValue(100);

            Destroy(gameObject);
        }
    }
}