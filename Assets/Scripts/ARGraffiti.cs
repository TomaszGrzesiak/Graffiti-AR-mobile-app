using System.Collections.Generic;
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
    public ARRaycastManager arRaycastManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private LineRenderer currentLineRenderer;
    private List<Vector3> points = new List<Vector3>();

    void Awake()
    {
        // Initialize actions
        touchPressAction = new InputAction(type: InputActionType.PassThrough, binding: "<Touchscreen>/press");
        touchPositionAction = new InputAction(type: InputActionType.PassThrough, binding: "<Touchscreen>/primaryTouch/position");
        keyAction = new InputAction(type: InputActionType.PassThrough, binding: "<Keyboard>/t");

        // Enable actions
        touchPressAction.Enable();
        touchPositionAction.Enable();
        keyAction.Enable();
    }

    void Start()
    {
        spraySound = GetComponent<AudioSource>();
    }

    private void Update()
    {
        bool isTouching = touchPressAction.ReadValue<float>() > 0;
        bool isKeyPressed = keyAction.ReadValue<float>() > 0;

        if (isTouching || isKeyPressed)
        {
            if (!isTouchingOrKeyHeld)
            {
                spraySound.Play();
                isTouchingOrKeyHeld = true;
            }

            Vector2 touchPosition = isTouching ? touchPositionAction.ReadValue<Vector2>() : new Vector2(Screen.width / 2, Screen.height / 2);
            Ray ray = arCamera.ScreenPointToRay(touchPosition);

            if (arRaycastManager.Raycast(ray, hits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;

                if (currentLineRenderer == null)
                {
                    GameObject newLine = Instantiate(paintPrefab, hitPose.position, Quaternion.identity);
                    currentLineRenderer = newLine.GetComponent<LineRenderer>();
                    points.Clear();
                }

                AddPoint(hitPose.position);
            }
        }
        else if (!isTouching && !isKeyPressed && isTouchingOrKeyHeld)
        {
            spraySound.Stop();
            isTouchingOrKeyHeld = false;
            currentLineRenderer = null;
        }
    }

    private void AddPoint(Vector3 point)
    {
        points.Add(point);
        currentLineRenderer.positionCount = points.Count;
        currentLineRenderer.SetPositions(points.ToArray());
    }

    private void OnDestroy()
    {
        touchPressAction.Disable();
        touchPositionAction.Disable();
        keyAction.Disable();
    }
}
