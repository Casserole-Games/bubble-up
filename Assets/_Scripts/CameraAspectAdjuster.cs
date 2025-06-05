using UnityEngine;

public class CameraResizer : MonoBehaviour
{
    public float defaultWidth = 1920f;
    public float defaultHeight = 1080f;

    private int lastScreenWidth;
    private int lastScreenHeight;

    void Awake()
    {
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
        AdjustCamera();
    }

    void Update()
    {
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
            AdjustCamera();
        }
    }

    void AdjustCamera()
    {
        float targetAspect = defaultWidth / defaultHeight;
        float currentAspect = (float)Screen.width / (float)Screen.height;

        Camera cam = GetComponent<Camera>();

        if (currentAspect >= targetAspect)
        {
            cam.orthographicSize = defaultHeight / 200f / 2f;
        }
        else
        {
            float differenceInSize = targetAspect / currentAspect;
            cam.orthographicSize = (defaultHeight / 200f / 2f) * differenceInSize;
        }

        Debug.Log($"Adjusted camera to {Screen.width}x{Screen.height}, orthoSize = {cam.orthographicSize}");
    }
}
