using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class PlayerSaveSystem : MonoBehaviour
{
    private Player player;

    async void Start()
    {
        if (SceneManager.GetActiveScene().name == "NodeMap")
        {
            enabled = false;
            return;
        }

        await FindPlayerAsync();
        LoadPlayerProgress();
    }

    async Task FindPlayerAsync()
    {
        while (player == null)
        {
            GameObject playerGO = GameObject.FindWithTag("Player");
            if (playerGO != null)
                player = playerGO.GetComponent<Player>();

            await Task.Yield();
        }
    }

    public void SavePlayerProgress()
    {
        if (player == null) return;

        PlayerSaveData data = new PlayerSaveData(
            player.maxLives,
            player.transform,
            SceneManager.GetActiveScene().buildIndex
        );

        SavePlayerData.Instance.SavePlayer(data);
    }

    public async void LoadPlayerProgress()
    {
        PlayerSaveData data = SavePlayerData.Instance.LoadPlayer();
        if (data == null) return;

        if (data.currentLevel != SceneManager.GetActiveScene().buildIndex)
        {
            await SceneManager.LoadSceneAsync(data.currentLevel);
            await FindPlayerAsync();
        }

        ApplyPlayerData(data);
    }

    void ApplyPlayerData(PlayerSaveData data)
    {
        player.maxLives = data.maxLives;
        player.transform.position = data.position.ToVector3();
    }

    void OnApplicationQuit()
    {
        SavePlayerProgress(); // synchronous = safe
    }
}