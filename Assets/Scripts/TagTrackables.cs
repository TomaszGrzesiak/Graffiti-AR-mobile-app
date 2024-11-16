using UnityEngine;

public class TagTrackables : MonoBehaviour
{
    private void Update()
    {
        // Find the "Trackables" child
        Transform trackables = transform.Find("Trackables");

        // If "Trackables" exists, tag all its children
        if (trackables != null)
        {
            foreach (Transform child in trackables)
            {
                if (child.gameObject.tag != "ARSurface") // Avoid redundant tagging
                {
                    child.gameObject.tag = "ARSurface";
                }
            }
        }
    }
}
