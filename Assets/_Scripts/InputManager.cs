using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class InputManager
{
    private static bool IsPointerOverButton()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var raycastResults = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        foreach (RaycastResult result in raycastResults)
        {
            if (result.gameObject.GetComponent<Button>() != null)
            {
                return true;
            }
        }

        return false;
    }

    public static bool InputDown()
    {
        if (IsPointerOverButton()) return false;
        if (Input.touchSupported && Input.touchCount > 0)
        {
            // Use only touch input on mobile if touch exists
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                return true;
            }
            return false;
        }
        return Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0);
    }

    public static bool InputHeld()
    {
        if (IsPointerOverButton()) return false;
        if (Input.touchSupported && Input.touchCount > 0)
        {
            TouchPhase phase = Input.GetTouch(0).phase;
            return phase == TouchPhase.Stationary || phase == TouchPhase.Moved;
        }
        return Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0);
    }

    public static bool InputUp()
    {
        if (Input.touchSupported && Input.touchCount > 0)
        {
            // On mobile, only return true if a touch exists and it ended
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                return true;
            }
            return false;
        }
        return Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(0);
    }
}
