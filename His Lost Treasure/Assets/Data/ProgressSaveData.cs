using System.Collections.Generic;

[System.Serializable]
public class ProgressSaveData
{
    public List<string> unlockedNodes = new();   // IDs of unlocked nodes
    public string currentNodeId;                 // ID of the current node
}
