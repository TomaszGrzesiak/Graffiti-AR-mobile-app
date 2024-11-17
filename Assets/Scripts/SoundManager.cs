using UnityEngine;
using UnityEngine.InputSystem;

public class SoundManager : MonoBehaviour
{
    private AudioSource spraySound;
    private InputAction touchAction;
    private InputAction keyAction;
    private bool isTouchingOrKeyHeld = false;

    void Awake()
    {
        // Initialize actions
        touchAction = new InputAction(type: InputActionType.PassThrough, binding: "<Touchscreen>/press");
        keyAction = new InputAction(type: InputActionType.PassThrough, binding: "<Keyboard>/t");

        // Enable actions
        touchAction.Enable();
        keyAction.Enable();
    }

    void Start()
    {
        spraySound = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Check if the touch or key press is held
        bool isTouching = touchAction.ReadValue<float>() > 0;
        bool isKeyPressed = keyAction.ReadValue<float>() > 0;

        // Start playing sound if either input is active and the sound is not already playing
        if ((isTouching || isKeyPressed) && !isTouchingOrKeyHeld)
        {
            spraySound.Play();
            isTouchingOrKeyHeld = true; // Mark that the sound is playing due to a continuous touch or key press
        }
        // Stop sound if neither input is active
        else if (!isTouching && !isKeyPressed && isTouchingOrKeyHeld)
        {
            spraySound.Stop();
            isTouchingOrKeyHeld = false; // Reset the flag as the input is no longer active
        }
    }

    private void OnDestroy()
    {
        // Disable actions
        touchAction.Disable();
        keyAction.Disable();
    }
}
