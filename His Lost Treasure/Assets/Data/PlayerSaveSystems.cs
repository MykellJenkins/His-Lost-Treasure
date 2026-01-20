using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks; // Required for Task.Yield

public class PlayerSaveSystem : MonoBehaviour
{
    private Player player;

    void Start()
    {
        // FIX LINE 10: Find the player by its tag instead of GetComponent on this object
        GameObject playerGO = GameObject.FindWithTag("Player");

        if (playerGO == null)
        {
            // Line 16: Log the error and disable the system gracefully
            Debug.LogError("Player GameObject not found in scene! Cannot initialize save system.");
            enabled = false; // Disable this script
            return;
        }

        player = playerGO.GetComponent<Player>();

        if (SceneManager.GetActiveScene().name == "NodeMap") { enabled = false; return; }

        LoadPlayerProgress();
    }


    public async void SavePlayerProgress()
    {
        // FIX: Check for null reference before accessing 'player'
        if (player == null)
        {
            // Line 38 error message is generated here, preventing a crash
            Debug.LogError("Cannot save player progress: Player reference is null in SavePlayerSystem.");
            return; // Exit the method early
        }

        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;

        // This line is now safe to run:
        PlayerSaveData pdata = new PlayerSaveData(player.maxLives, player.transform, currentLevelIndex);

        await SavePlayerData.Instance.SaveDataAsync(pdata, "player.json");
    }

    public async void LoadPlayerProgress()
    {
        // 1. Retrieve the data
        PlayerSaveData pdata = SavePlayerData.Instance.LoadPlayer();
        if (pdata == null) return;

        // 2. Check if we need to switch scenes
        if (pdata.currentLevel != SceneManager.GetActiveScene().buildIndex)
        {
            AsyncOperation loadOp = SceneManager.LoadSceneAsync(pdata.currentLevel);

            // Wait until the scene is fully loaded
            while (!loadOp.isDone)
            {
                await Task.Yield();
            }

            // 3. FIX: Re-find the player in the new scene (since the original was destroyed)
            GameObject playerGO = GameObject.FindWithTag("Player");
            if (playerGO != null)
            {
                player = playerGO.GetComponent<Player>();
            }
        }

        // 4. Apply the data
        if (player != null) ApplyPlayerData(pdata);
    }

    private void ApplyPlayerData(PlayerSaveData data)
    {
        player.maxLives = data.maxLives;

        // Direct conversion using the helper method
        if (data.position.x != 0 || data.position.y != 0 || data.position.z != 0)
        {
            player.transform.position = data.position.ToVector3();
        }
    }

    void OnApplicationQuit()
    {
        // Final save on exit
        SavePlayerProgress();
    }
}