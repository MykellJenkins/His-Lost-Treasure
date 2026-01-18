using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
    public void resume()
    {
        GameManager.instance.stateUnpause();
    }

    public void restart()
    {
        GameManager.instance.rmInstance.RespawnPlayer(GameManager.instance.player);
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
