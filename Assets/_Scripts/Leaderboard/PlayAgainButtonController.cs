using Assets._Scripts;
using UnityEngine;
using UnityEngine.UI;

public class PlayAgainButtonController : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(GameManager.Instance.RestartGame);
    }
}
