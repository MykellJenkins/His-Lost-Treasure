using UnityEngine;
using System;

[Serializable]
public class PlayerSaveData
{
    public int maxLives = 3;
    public int currentLevel;

    // Use custom structs for cleaner JSON readability
    public SerializableVector3 position;
    public SerializableVector3 rotation;

    // Default constructor for JsonUtility
    public PlayerSaveData() { }

    // Better: Pass the specific data needed or a small interface
    public PlayerSaveData(int lives, Transform playerTransform, int sceneIndex)
    {
        maxLives = lives;
        currentLevel = sceneIndex;
        position = new SerializableVector3(playerTransform.position);
        rotation = new SerializableVector3(playerTransform.eulerAngles);
    }
}

[Serializable]
public struct SerializableVector3
{
    public float x, y, z;

    public SerializableVector3(Vector3 v)
    {
        x = v.x; y = v.y; z = v.z;
    }

    public Vector3 ToVector3() => new Vector3(x, y, z);
}