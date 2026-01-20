using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections;

public class ButtonFunctions : MonoBehaviour
{
    private void ResetSceneState()
    {
        Time.timeScale = 1f;
        if (GameManager.Instance != null) GameManager.Instance.StateUnpause();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Resume() => GameManager.Instance.StateUnpause();

    public void Restart()
    {
        ResetSceneState();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMenu()
    {
        ResetSceneState();
        SceneManager.LoadScene("Menu");
    }

    public void LevelSelect()
    {
        ResetSceneState();
        SceneManager.LoadScene("NodeMap");
    }

    public void NewGame()
    {
        ResetSceneState();

        string savePath = Path.Combine(Application.persistentDataPath, "Saves");
        if (Directory.Exists(savePath))
        {
            File.Delete(Path.Combine(savePath, "player.json"));
            File.Delete(Path.Combine(savePath, "progress.json"));
        }

        PlayerPrefs.DeleteKey("LastSessionTime");
        PlayerPrefs.Save();

        SceneManager.LoadScene("NodeMap");
    }

    public void LoadGame()
    {
        string playerPath = Path.Combine(Application.persistentDataPath, "Saves", "player.json");

        if (!File.Exists(playerPath))
        {
            NewGame();
            return;
        }

        ResetSceneState();
        StartCoroutine(LoadGameAsync("NodeMap"));
    }

    private IEnumerator LoadGameAsync(string sceneName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone) yield return null;
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}