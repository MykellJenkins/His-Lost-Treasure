using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class PlayerSaveSystem : MonoBehaviour
{
    public static PlayerSaveSystem Instance { get; private set; }
    private Player player;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

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
        int attempts = 0;
        while (player == null && attempts < 300) // ~5 seconds at 60fps
        {
            GameObject playerGO = GameObject.FindWithTag("Player");
            if (playerGO != null)
            {
                player = playerGO.GetComponent<Player>();
                break;
            }

            await Task.Yield();
            attempts++;
        }

        if (player == null)
            Debug.LogWarning("Player not found after waiting! Check scene setup.");
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

    public async void LoadPlayerProgress(bool forceSceneLoad = false)
    {
        PlayerSaveData data = SavePlayerData.Instance.LoadPlayer();
        if (data == null) return;

        int currentBuildIndex = SceneManager.GetActiveScene().buildIndex;

        // Only load the scene if forced (from main menu) AND it's a different scene
        if (forceSceneLoad && data.currentLevel != currentBuildIndex)
        {
            await SceneManager.LoadSceneAsync(data.currentLevel);
            return;
        }

        // If we are already in the scene, or loading is not forced, just apply stats
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