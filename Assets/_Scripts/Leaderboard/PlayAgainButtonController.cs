using Assets._Scripts;
using UnityEngine;
using UnityEngine.UI;

public class PlayAgainButtonController : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => {
            SFXManager.Instance.PlayOneShot("button", 0.5f);
            GameManager.Instance.RestartGame(); 
        });
    }
}
