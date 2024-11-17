using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DraggableSprayCan : MonoBehaviour
{
    public GameObject sprayCanImage; // Assign the spray can image (child of Canvas) in the inspector
    public GameObject CheckMenu;
    private InputAction touchPressAction;
    private InputAction touchPositionAction;

    void Awake()
    {
        // Setup input actions
        touchPressAction = new InputAction(type: InputActionType.PassThrough, binding: "<Touchscreen>/press");
        touchPositionAction = new InputAction(type: InputActionType.PassThrough, binding: "<Touchscreen>/primaryTouch/position");

        // Enable input actions
        touchPressAction.Enable();
        touchPositionAction.Enable();
    }

    void Update()
    {
        if (CheckMenu.activeSelf)
        {
            // Check if the screen is being touched
            bool isTouching = touchPressAction.ReadValue<float>() > 0;

            if (isTouching)
            {
                // Make the spray can image visible
                sprayCanImage.SetActive(true);

                // Get the touch position
                Vector2 touchPosition = touchPositionAction.ReadValue<Vector2>();

                // Convert the touch position to the canvas space
                RectTransform canvasRect = sprayCanImage.transform.parent as RectTransform;
                RectTransform sprayRect = sprayCanImage.GetComponent<RectTransform>();

                Vector2 canvasPos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvasRect,
                    touchPosition,
                    null,
                    out canvasPos);

                // Apply an offset so the touch position is at the top of the image
                Vector2 offset = new Vector2(0, -sprayRect.rect.height / 3);
                sprayRect.anchoredPosition = canvasPos + (offset * 2);
            }
            else
            {
                // Hide the spray can image when not touching
                sprayCanImage.SetActive(false);
            }
        }
    }


    private void OnDestroy()
    {
        // Disable input actions
        touchPressAction.Disable();
        touchPositionAction.Disable();
    }
}
