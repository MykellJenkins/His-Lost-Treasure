using System.IO;
using UnityEngine;

public class SavePlayerData : MonoBehaviour
{
    public static SavePlayerData Instance;

    string playerPath;
    string menuPath;
    string progressPath;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        playerPath = Application.persistentDataPath + "/player.json";
        menuPath = Application.persistentDataPath + "/menu.json";
    }

    // ---------- PLAYER ----------
    public void SavePlayer(PlayerSaveData data)
    {
        File.WriteAllText(playerPath, JsonUtility.ToJson(data, true));
    }

    public PlayerSaveData LoadPlayer()
    {
        if (!File.Exists(playerPath)) return null;
        return JsonUtility.FromJson<PlayerSaveData>(File.ReadAllText(playerPath));
    }

    // ---------- MENU ----------
    public void SaveMenu(MenuSaveData data)
    {
        File.WriteAllText(menuPath, JsonUtility.ToJson(data, true));
    }

    public MenuSaveData LoadMenu()
    {
        if (!File.Exists(menuPath)) return null;
        return JsonUtility.FromJson<MenuSaveData>(File.ReadAllText(menuPath));
    }

    // ---------- NODE PROGRESS ----------
    public void SaveProgress(ProgressSaveData data)
    {
        File.WriteAllText(progressPath, JsonUtility.ToJson(data, true));
    }

    public ProgressSaveData LoadProgress()
    {
        if (!File.Exists(progressPath)) return null;
        return JsonUtility.FromJson<ProgressSaveData>(File.ReadAllText(progressPath));
    }


}
