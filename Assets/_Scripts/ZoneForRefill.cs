using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Scripts
{
    internal class ZoneForRefill : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("GreenLine"))
            {
                GameManager.Instance.SetSoapRefill(true);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("GreenLine"))
            {
                GameManager.Instance.SetSoapRefill(false);
            }
        }
    }
}
