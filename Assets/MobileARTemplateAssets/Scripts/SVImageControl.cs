using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MobileARTemplateAssets.Scripts
{
    public class SVImageControl : MonoBehaviour, IDragHandler, IPointerClickHandler
    {
        [SerializeField]
        private Image pickerImage;
        private RawImage SVimage;
        private ColourPicker CC;
        private RectTransform rectTransform, pickerTransform;
    
        private void Awake()
        { 
            SVimage = GetComponent<RawImage>();
            CC = FindFirstObjectByType<ColourPicker>();
            rectTransform = GetComponent<RectTransform>();
        
            pickerTransform = pickerImage.GetComponent<RectTransform>();
            pickerTransform.position = new Vector2(-(rectTransform.sizeDelta.x * 0.5f), -(rectTransform.sizeDelta.y * 0.5f));
        }

        void UpdateColour(PointerEventData eventData)
        {
            Vector3 pos = rectTransform.InverseTransformPoint(eventData.position);
        
            float deltaX = rectTransform.sizeDelta.x * 0.5f;
            float deltaY = rectTransform.sizeDelta.y * 0.5f;

            if (pos.x < -deltaX)
            {
                pos.x = -deltaX;
            }
            else if (pos.x > deltaX)
            {
                pos.x = deltaX;
            }

            if (pos.y < -deltaY)
            {
                pos.y = -deltaY;
            }
            else if (pos.y > deltaY)
            {
                pos.y = deltaY;
            }
        
            float x = pos.x + deltaX;
            float y = pos.y + deltaY;
        
            float xNormalized = x / rectTransform.sizeDelta.x;
            float yNormalized = y / rectTransform.sizeDelta.y;

            pickerTransform.localPosition = pos;
            pickerImage.color = Color.HSVToRGB(0, 0, 1 - yNormalized);
        
            CC.SetSV(xNormalized, yNormalized);
        }
    
        public void OnDrag(PointerEventData eventData)
        {
            UpdateColour(eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            UpdateColour(eventData);
        }
    }
}
