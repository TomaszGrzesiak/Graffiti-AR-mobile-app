using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Slider = UnityEngine.UI.Slider;

namespace MobileARTemplateAssets.Scripts
{
    public class ColourPicker : MonoBehaviour
    {
        public float currentHue, currentSat, currentValue;
        [SerializeField] private RawImage hueImage, satValueImage, outputImage;
        [SerializeField] private Slider hueSlider;
        [SerializeField] private TMP_InputField hexInputField;
        private Texture2D hueTexture, svTexture, outputTexture;

        private void Start()
        {
            CreateHueImage();
            CreateSVImage();
            CreateOutputImage();
            UpdateOutputImage();
        }

        private void CreateHueImage()
        {
            hueTexture = new Texture2D(1, 16);
            hueTexture.wrapMode = TextureWrapMode.Clamp;
            hueTexture.name = "HueTexture";

            for (int i = 0; i < hueTexture.height; i++)
            {
                hueTexture.SetPixel(0, i, Color.HSVToRGB((float)i / hueTexture.height, 1, 0.9f));
            }

            hueTexture.Apply();
            currentHue = 0;

            hueImage.texture = hueTexture;
        }

        private void CreateSVImage()
        {
            svTexture = new Texture2D(16, 16);
            svTexture.wrapMode = TextureWrapMode.Clamp;
            svTexture.name = "SatValTexture";

            for (int y = 0; y < svTexture.height; y++)
            {
                for (int x = 0; x < svTexture.width; x++)
                {
                    svTexture.SetPixel(x, y, Color.HSVToRGB(currentHue, (float)x / svTexture.width, (float)y / svTexture.height));
                }
            }

            svTexture.Apply();
            currentSat = 0;
            currentValue = 0;

            satValueImage.texture = svTexture;
        }

        private void CreateOutputImage()
        {
            outputTexture = new Texture2D(1, 16);
            outputTexture.wrapMode = TextureWrapMode.Clamp;
            outputTexture.name = "OutputTexture";

            Color currentColor = Color.HSVToRGB(currentHue, currentSat, currentValue);

            for (int i = 0; i < outputTexture.height; i++)
            {
                outputTexture.SetPixel(0, i, currentColor);
            }

            outputTexture.Apply();
            outputImage.texture = outputTexture;
        }

        private void UpdateOutputImage()
        {
            Color currentColor = Color.HSVToRGB(currentHue, currentSat, currentValue);

            // Apply the color to all pixels of the texture
            for (int i = 0; i < outputTexture.height; i++)
            {
                outputTexture.SetPixel(0, i, currentColor);
            }

            outputTexture.Apply();
            outputImage.texture = outputTexture;  // Make sure the texture is applied to the RawImage
        }


        public void SetSV(float S, float V)
        {
            currentSat = S;
            currentValue = V;

            UpdateOutputImage();
        }

        public void UpdateSVImage()
        {
            currentHue = hueSlider.value;

            for (int i = 0; i < svTexture.height; i++)
            {
                for (int x = 0; x < svTexture.width; x++)
                {
                    svTexture.SetPixel(x, i, Color.HSVToRGB(currentHue, (float)x / svTexture.width, (float)i / svTexture.height));
                }
            }

            svTexture.Apply();
            UpdateOutputImage();
        }
    }
}
