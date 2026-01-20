using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    void Start()
    {
        // Pull the saved data from your SavePlayerData singleton
        PlayerSaveData data = SavePlayerData.Instance.LoadPlayer();

        // Apply it to your player/world
        ApplyLoadedData(data);
    }

    void ApplyLoadedData(PlayerSaveData data)
    {
        // Example: Move player to saved position or set health
        Debug.Log("Game Data Loaded Successfully!");
    }
}
