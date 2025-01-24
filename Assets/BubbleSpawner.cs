using Unity.VisualScripting;
using UnityEngine;

public class BubbleSpawner : MonoBehaviour
{
    public GameObject BubblePrefab;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space key was pressed");
            Instantiate(BubblePrefab, transform.position, Quaternion.identity);
        }
    }
}
