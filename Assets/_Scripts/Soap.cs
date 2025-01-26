using UnityEngine;

namespace Assets._Scripts
{
    public class Soap : MonoBehaviour
    {
        public GameObject particleEffect;

        void OnTriggerEnter2D(Collider2D col)
        {
            if (!col.gameObject.CompareTag("Bubble")) return;
            if (HighFinder.Instance.localHighestY < 850) return;

            BubbleSpawner.RemainingSoap = GameParameters.Instance.MaxSoapAmount;
            UIManager.Instance.SetTankValue((int)GameParameters.Instance.MaxSoapAmount);

            Destroy(gameObject);
            Destroy(particleEffect);
        }
    }
}