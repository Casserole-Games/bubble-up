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
        return Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
    }

    public static bool InputHeld()
    {
        if (IsPointerOverButton()) return false;
        return Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved);
    }

    public static bool InputUp()
    {
        return Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended);
    }

    public static bool ControlMusic()
    {
        return Input.GetKeyUp(KeyCode.M);
    }
}
