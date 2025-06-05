using Assets._Scripts;
using UnityEngine;
using UnityEngine.UI;

public class PlayAgainButtonController : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => {
            SFXManager.Instance.PlayOneShot("button", GameParameters.Instance.UIClickVolume, 0.9f, 0.9f);
            GameManager.Instance.RestartGame(); 
        });
    }
}
