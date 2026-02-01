using System;
using UnityEngine;

[Serializable]
public class PlayerSaveData
{
    public int maxLives;
    public SerializableVector3 feetPosition;
    public int currentLevel;

    public PlayerSaveData(int lives, Vector3 feetPos, int level)
    {
        maxLives = lives;
        feetPosition = new SerializableVector3(feetPos);
        currentLevel = level;
    }
}

[Serializable]
public struct SerializableVector3
{
    public float x, y, z;

    public SerializableVector3(Vector3 v)
    {
        x = v.x;
        y = v.y;
        z = v.z;
    }

    public Vector3 ToVector3() => new Vector3(x, y, z);
}
