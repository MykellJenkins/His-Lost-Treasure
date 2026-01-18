using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
    public void resume()
    {
        GameManager.instance.stateUnpause();
    }

    public void Restart()
    {
        Time.timeScale = 1f;   // FORCE reset
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void backToMenu()
    {
        SceneManager.LoadScene("Menu");
        // stateUnpause()?
    }

    public void levelSelect()
    {
        SceneManager.LoadScene("NodeMap");
        // stateUnpause()?
    }

    public void quit()
    {
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}
