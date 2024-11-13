using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARGraffiti : MonoBehaviour
{
    private AudioSource spraySound;
    private InputAction touchAction;
    private InputAction keyAction;
    private bool isTouchingOrKeyHeld = false;

    public Camera arCamera;                  // Reference to the AR camera
    public GameObject paintPrefab;           // Prefab containing the LineRenderer
    public ARRaycastManager arRaycastManager; // Reference to AR Raycast Manager
    private List<ARRaycastHit> hits = new List<ARRaycastHit>(); // List to store raycast hits
    private LineRenderer currentLineRenderer; // Current LineRenderer instance
    private List<Vector3> points = new List<Vector3>(); // Points for the line

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

    private void Update()
    {
        bool isTouching = touchAction.ReadValue<float>() > 0;
        bool isKeyPressed = keyAction.ReadValue<float>() > 0;

        // If touch or key is held, start painting and sound
        if (isTouching || isKeyPressed)
        {
            // Play sound if it’s not already playing
            if (!isTouchingOrKeyHeld)
            {
                spraySound.Play();
                isTouchingOrKeyHeld = true;
            }

            // Raycast from the camera to detect surfaces
            Ray ray = arCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            if (arRaycastManager.Raycast(ray, hits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;

                // Start a new line if needed
                if (currentLineRenderer == null)
                {
                    GameObject newLine = Instantiate(paintPrefab, hitPose.position, Quaternion.identity);
                    currentLineRenderer = newLine.GetComponent<LineRenderer>();
                    points.Clear();
                }

                // Add a new point to the line
                AddPoint(hitPose.position);
            }
        }
        else if (!isTouching && !isKeyPressed && isTouchingOrKeyHeld)
        {
            spraySound.Stop();
            isTouchingOrKeyHeld = false;
            currentLineRenderer = null; // Reset the line
        }
    }

    // Add a point to the LineRenderer
    private void AddPoint(Vector3 point)
    {
        points.Add(point);
        currentLineRenderer.positionCount = points.Count;
        currentLineRenderer.SetPositions(points.ToArray());
    }

    private void OnDestroy()
    {
        // Disable actions
        touchAction.Disable();
        keyAction.Disable();
    }
}
