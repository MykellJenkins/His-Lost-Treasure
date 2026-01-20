using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class SavePlayerData : MonoBehaviour
{
    public static SavePlayerData Instance { get; private set; }

    // Use a subfolder to keep persistentDataPath clean
    private string SaveDirectory => Path.Combine(Application.persistentDataPath, "Saves");

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Ensure the save directory exists immediately
        if (!Directory.Exists(SaveDirectory)) Directory.CreateDirectory(SaveDirectory);

        // Initialize default files if they don't exist
        InitializeDefaultSave<MenuSaveData>("menu.json");
        InitializeDefaultSave<ProgressSaveData>("progress.json");
        InitializeDefaultSave<PlayerSaveData>("player.json");
    }

    // ---------- GENERIC CORE LOGIC ----------

    public async Task SaveDataAsync<T>(T data, string fileName)
    {
        string path = Path.Combine(SaveDirectory, fileName);
        try
        {
            string json = JsonUtility.ToJson(data, true);
            await File.WriteAllTextAsync(path, json);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Save failed for {fileName}: {e.Message}");
        }
    }

    public T LoadData<T>(string fileName) where T : new()
    {
        string path = Path.Combine(SaveDirectory, fileName);
        if (!File.Exists(path)) return new T(); // Return a fresh instance if file is missing

        try
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<T>(json);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Load failed for {fileName}: {e.Message}");
            return new T();
        }
    }

    private void InitializeDefaultSave<T>(string fileName) where T : new()
    {
        string path = Path.Combine(SaveDirectory, fileName);
        if (!File.Exists(path))
        {
            // Run synchronously during Awake to ensure data is ready for the first frame
            string json = JsonUtility.ToJson(new T(), true);
            File.WriteAllText(path, json);
        }
    }

    // ---------- CONVENIENCE WRAPPERS ----------
    // You can still keep these for easier calling from other scripts

    public async void SavePlayer(PlayerSaveData data) => await SaveDataAsync(data, "player.json");
    public PlayerSaveData LoadPlayer() => LoadData<PlayerSaveData>("player.json");

    public async void SaveMenu(MenuSaveData data) => await SaveDataAsync(data, "menu.json");
    public MenuSaveData LoadMenu() => LoadData<MenuSaveData>("menu.json");

    public async void SaveProgress(ProgressSaveData data) => await SaveDataAsync(data, "progress.json");
    public ProgressSaveData LoadProgress() => LoadData<ProgressSaveData>("progress.json");
}