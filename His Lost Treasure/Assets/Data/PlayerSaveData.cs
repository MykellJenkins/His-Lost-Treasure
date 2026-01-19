using UnityEngine;

[System.Serializable]
public class PlayerSaveData
{
    public int maxLives = 3;
    public Vector3 playerPosition;
    public int currentLevel = 0;
    // Add other player states if need
}