using System.Collections.Generic;
using MobileARTemplateAssets.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARGraffiti : MonoBehaviour
{
    private AudioSource spraySound;
    private InputAction touchPressAction;
    private InputAction touchPositionAction;
    private InputAction keyAction;
    private bool isTouchingOrKeyHeld = false;

    public Camera arCamera;
    public GameObject paintPrefab;
    public ParticleSystem sprayParticles; // Particle system to simulate spray paint
    public ARRaycastManager arRaycastManager;
    public TextMeshProUGUI Thickness; // Slider text indicating thickness
    public GameObject CheckMenu;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    // Reference to the ColourPicker script
    public ColourPicker colorPicker;

    void Awake()
    {
        touchPressAction = new InputAction(type: InputActionType.PassThrough, binding: "<Touchscreen>/press");
        touchPositionAction = new InputAction(type: InputActionType.PassThrough, binding: "<Touchscreen>/primaryTouch/position");
        keyAction = new InputAction(type: InputActionType.PassThrough, binding: "<Keyboard>/t");

        touchPressAction.Enable();
        touchPositionAction.Enable();
        keyAction.Enable();
    }

    void Start()
    {
        spraySound = GetComponent<AudioSource>();

        if (sprayParticles != null)
        {
            var emission = sprayParticles.emission;
            emission.enabled = false;
        }
    }

    private void Update()
    {
        // Check if CheckMenu is active, and if so, stop the spray functionality
        if (CheckMenu.activeSelf)
        {
            if (isTouchingOrKeyHeld)
            {
                spraySound.Stop();
                isTouchingOrKeyHeld = false;

                var emission = sprayParticles.emission;
                emission.enabled = false;
            }
            return;
        }

        bool isTouching = touchPressAction.ReadValue<float>() > 0;
        bool isKeyPressed = keyAction.ReadValue<float>() > 0;

        if (isTouching || isKeyPressed)
        {
            if (!isTouchingOrKeyHeld)
            {
                spraySound.Play();
                isTouchingOrKeyHeld = true;

                var emission = sprayParticles.emission;
                emission.enabled = true;
            }

            Vector2 touchPosition = isTouching ? touchPositionAction.ReadValue<Vector2>() : new Vector2(Screen.width / 2, Screen.height / 2);
            Ray ray = arCamera.ScreenPointToRay(touchPosition);

            if (arRaycastManager.Raycast(ray, hits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;

                // Position the particle system at the raycast hit point
                sprayParticles.transform.position = hitPose.position + hitPose.up * 0.1f; // Move 0.1 units back
                sprayParticles.transform.rotation = Quaternion.LookRotation(-hitPose.up, hitPose.forward);
            }
        }
        else if (!isTouching && !isKeyPressed && isTouchingOrKeyHeld)
        {
            spraySound.Stop();
            isTouchingOrKeyHeld = false;

            var emission = sprayParticles.emission;
            emission.enabled = false;
        }
    }


    private void OnDestroy()
    {
        touchPressAction.Disable();
        touchPositionAction.Disable();
        keyAction.Disable();
    }

    void OnParticleCollision(GameObject other)
    {
        List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
        int numCollisionEvents = sprayParticles.GetCollisionEvents(other, collisionEvents);

        for (int i = 0; i < numCollisionEvents; i++)
        {
            Vector3 collisionPoint = collisionEvents[i].intersection;
            Vector3 collisionNormal = collisionEvents[i].normal;

            Quaternion rotation = Quaternion.LookRotation(collisionNormal, Vector3.up) * Quaternion.Euler(90, 0, 0);

            // Instantiate the paint prefab and set its color from the ColourPicker
            GameObject paintInstance = Instantiate(paintPrefab, collisionPoint, rotation);
            Renderer paintRenderer = paintInstance.GetComponent<Renderer>();

            if (paintRenderer != null)
            {
                Color sprayColor = Color.HSVToRGB(colorPicker.currentHue, colorPicker.currentSat, colorPicker.currentValue);
                paintRenderer.material.color = sprayColor;
            }

            // Adjust the size of the paintPrefab based on Thickness
            float thicknessValue = Mathf.Clamp(float.Parse(Thickness.text), 0.1f, 1f); // Ensure a valid range
            paintInstance.transform.localScale = new Vector3(thicknessValue / 20, thicknessValue / 20, thicknessValue / 20);
        }
    }
}
