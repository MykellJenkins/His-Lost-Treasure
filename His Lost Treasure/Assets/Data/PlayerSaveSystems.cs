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
        if (!IsPlayableLevel(SceneManager.GetActiveScene().buildIndex)) return;
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

        int currentBuildIndex = SceneManager.GetActiveScene().buildIndex;

        // Only load the scene if we are NOT currently in it.
        // IMPORTANT: Ensure your Main Menu or Launcher is the one calling this, 
        // not a script that exists inside the level itself.
        if (data.currentLevel != currentBuildIndex)
        {
            // If this script is on a GameObject that persists (DontDestroyOnLoad), 
            // this is okay. If not, this script is destroyed mid-execution.
            await SceneManager.LoadSceneAsync(data.currentLevel);
            return;
        }

        // If we are already in the right scene, just apply the stats
        await FindPlayerAsync();
        ApplyPlayerData(data);
    }

    void ApplyPlayerData(PlayerSaveData data)
    {
        player.maxLives = data.maxLives;
        player.transform.position = data.position.ToVector3();
    }

    void OnApplicationQuit()
    {
        if (SceneManager.GetActiveScene().name != "NodeMap")
            SavePlayerProgress();
    }

    bool IsPlayableLevel(int buildIndex)
    {
        string name = SceneManager.GetSceneByBuildIndex(buildIndex).name;
        return name != "NodeMap"; // Add other hubs if needed
    }
}