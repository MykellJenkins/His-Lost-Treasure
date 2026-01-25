using UnityEngine;
using System;

[Serializable]
public class PlayerSaveData
{
    public int maxLives;
    public SerializableVector3 position;
    public int currentLevel;

    // REQUIRED for LoadData<T>()
    public PlayerSaveData()
    {
        maxLives = 3;
        position = new SerializableVector3(Vector3.zero);
        currentLevel = 0;
    }

    public PlayerSaveData(int lives, Transform t, int level)
    {
        maxLives = lives;
        position = new SerializableVector3(t.position);
        currentLevel = level;
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