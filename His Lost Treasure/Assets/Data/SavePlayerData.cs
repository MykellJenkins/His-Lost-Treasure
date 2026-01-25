using System.IO;
using UnityEngine;

public class SavePlayerData : MonoBehaviour
{
    public static SavePlayerData Instance { get; private set; }
    private string SaveDirectory => Path.Combine(Application.persistentDataPath, "Saves");

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (!Directory.Exists(SaveDirectory))
            Directory.CreateDirectory(SaveDirectory);
    }

    public void SaveData<T>(T data, string fileName)
    {
        string path = Path.Combine(SaveDirectory, fileName);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
    }

    public T LoadData<T>(string fileName) where T : new()
    {
        string path = Path.Combine(SaveDirectory, fileName);
        if (!File.Exists(path)) return new T();

        string json = File.ReadAllText(path);
        T data = JsonUtility.FromJson<T>(json);
        return data == null ? new T() : data;
    }

    public void SaveMenu(MenuSaveData data) => SaveData(data, "menu.json");
    public MenuSaveData LoadMenu() => LoadData<MenuSaveData>("menu.json");

    public void SaveProgress(ProgressSaveData data) => SaveData(data, "progress.json");
    public ProgressSaveData LoadProgress() => LoadData<ProgressSaveData>("progress.json");

    public void SavePlayer(PlayerSaveData data) => SaveData(data, "player.json");
    public PlayerSaveData LoadPlayer() => LoadData<PlayerSaveData>("player.json");
}