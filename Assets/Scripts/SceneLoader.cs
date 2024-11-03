using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void SceneChange(int index)
    {
        SceneManager.LoadScene(index);
    }
}
