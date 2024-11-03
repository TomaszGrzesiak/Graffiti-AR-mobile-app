using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARGraffiti : MonoBehaviour
{
    public Camera arCamera;                  // Reference to the AR camera
    public GameObject paintPrefab;           // Prefab containing the LineRenderer
    public ARRaycastManager arRaycastManager; // Reference to AR Raycast Manager
    private List<ARRaycastHit> hits = new List<ARRaycastHit>(); // List to store raycast hits

    private LineRenderer currentLineRenderer; // Current LineRenderer instance
    private List<Vector3> points = new List<Vector3>(); // Points for the line

    private void Update()
    {
        // Continuously raycast in front of the camera
        Ray ray = new Ray(arCamera.transform.position, arCamera.transform.forward);

        // Perform the raycast
        if (arRaycastManager.Raycast(ray, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;

            // Instantiate the paint line if there is no current line
            if (currentLineRenderer == null)
            {
                GameObject newLine = Instantiate(paintPrefab, hitPose.position, Quaternion.identity);
                currentLineRenderer = newLine.GetComponent<LineRenderer>();
                points.Clear();
            }

            // Update the paint line's position
            AddPoint(hitPose.position);
        }
        else
        {
            // Reset if no plane is detected
            if (currentLineRenderer != null)
            {
                currentLineRenderer = null; // Reset current line
            }
        }
    }

    // Add a point to the LineRenderer
    private void AddPoint(Vector3 point)
    {
        points.Add(point);
        currentLineRenderer.positionCount = points.Count;
        currentLineRenderer.SetPositions(points.ToArray());
    }
}
