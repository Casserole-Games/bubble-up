using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets._Scripts
{
    internal class Bubble : MonoBehaviour
    {
        private Color color = Color.white;

        public void Pop()
        {
            Debug.Log("Popped!");
            Destroy(this.gameObject);
        }

        internal void SetColor(Color color)
        {
            this.color = color;
            this.GetComponent<SpriteRenderer>().color = color;
        }
    }
}
