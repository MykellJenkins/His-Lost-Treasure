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
        DontDestroyOnLoad(gameObject);
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
        while (player == null)
        {
            GameObject go = GameObject.FindWithTag("Player");
            if (go != null)
                player = go.GetComponent<Player>();

            await Task.Yield();
        }
    }

    public void SavePlayerProgress()
    {
        if (player == null) return;

        PlayerSaveData data = new PlayerSaveData(
            player.maxLives,
            player.feet.position,
            SceneManager.GetActiveScene().buildIndex
        );

        SavePlayerData.Instance.SavePlayer(data);
    }

    public async void LoadPlayerProgress(bool forceSceneLoad = false)
    {
        PlayerSaveData data = SavePlayerData.Instance.LoadPlayer();
        if (data == null) return;

        if (forceSceneLoad &&
            SceneManager.GetActiveScene().buildIndex != data.currentLevel)
        {
            await SceneManager.LoadSceneAsync(data.currentLevel);
        }

        await FindPlayerAsync();
        ApplyPlayerData(data);
    }

    void ApplyPlayerData(PlayerSaveData data)
    {
        player.maxLives = data.maxLives;
        player.TeleportFromFeet(data.feetPosition.ToVector3());
    }

    void OnApplicationQuit()
    {
        if (SceneManager.GetActiveScene().name != "NodeMap")
            SavePlayerProgress();
    }
}