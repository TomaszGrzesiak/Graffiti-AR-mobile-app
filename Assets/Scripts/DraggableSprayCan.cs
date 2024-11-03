using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableSprayCan : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private Quaternion originalRotation;
    private Vector2 offset;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        if (canvas == null)
        {
            Debug.LogError("The image must be a child of a canvas.");
        }

        originalRotation = rectTransform.rotation;
    }

    // Called when pointer/touch is down on the image
    public void OnPointerDown(PointerEventData eventData)
    {
        // Rotate the image by 20 degrees
        rectTransform.Rotate(0, 0, 20f);

        // Calculate offset so the image follows the top center
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPointerPosition);

        // Calculate the offset from the center of the top edge
        Vector2 imageCenterTopEdge = new Vector2(0, rectTransform.rect.height / 2);
        offset = localPointerPosition - imageCenterTopEdge;
    }

    // Called when the image is dragged
    public void OnDrag(PointerEventData eventData)
    {
        if (canvas == null)
            return;

        Vector2 pointerPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out pointerPosition);

        // Adjust position based on offset
        rectTransform.anchoredPosition = pointerPosition - offset;
    }

    // Called when the pointer/touch is released
    public void OnPointerUp(PointerEventData eventData)
    {
        // Rotate the image back to the original rotation
        rectTransform.rotation = originalRotation;
    }
}
