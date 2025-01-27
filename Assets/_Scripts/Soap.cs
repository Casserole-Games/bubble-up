using System;
using UnityEngine;

namespace Assets._Scripts
{
    public class Soap : MonoBehaviour
    {
        public GameObject particleEffect;

        void OnTriggerEnter2D(Collider2D col)
        {
            if (!GameManager.Instance.CanRefillSoap || !col.gameObject.CompareTag("Bubble")) return;
            //if (HighFinder.Instance.localHighestY < 850) return;

            if (BubbleSpawner.RemainingSoap > GameParameters.Instance.MaxSoapAmount / 2)
            {
                BubbleSpawner.RemainingSoap = Math.Min(BubbleSpawner.RemainingSoap + (GameParameters.Instance.MaxSoapAmount / 8), GameParameters.Instance.MaxSoapAmount);
            }
            else
            {
                BubbleSpawner.RemainingSoap = GameParameters.Instance.MaxSoapAmount / 2;
            }

            Destroy(gameObject);
            Destroy(particleEffect);
        }
    }
}