using UnityEngine;

public class QuitApplication : MonoBehaviour
{
    // Method to be called when the quit button is clicked
    public void QuitGame()
    {
        // Logs a message to confirm the quit action when running in the Unity Editor
        Debug.Log("Quit button pressed. The application will quit if built and run.");

        // Quits the application
        Application.Quit();
    }
}