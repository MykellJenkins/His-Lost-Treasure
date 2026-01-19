using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSaveSystem : MonoBehaviour
{
    private Player player;

    void Start()
    {
        player = GetComponent<Player>();
        LoadPlayerProgress();
    }

    public void SavePlayerProgress()
    {
        PlayerSaveData pdata = new PlayerSaveData
        {
            maxLives = player.maxLives,
            playerPosition = player.transform.position,
            currentLevel = SceneManager.GetActiveScene().buildIndex
        };

        SavePlayerData.Instance.SavePlayer(pdata);
    }

    public void LoadPlayerProgress()
    {
        PlayerSaveData pdata = SavePlayerData.Instance.LoadPlayer();
        if (pdata == null) return;

        player.maxLives = pdata.maxLives;
        player.transform.position = pdata.playerPosition;

        // Optional: Load saved level
        if (pdata.currentLevel != SceneManager.GetActiveScene().buildIndex)
        {
            SceneManager.LoadScene(pdata.currentLevel);
        }
    }

    void OnApplicationQuit()
    {
        SavePlayerProgress();
    }
}