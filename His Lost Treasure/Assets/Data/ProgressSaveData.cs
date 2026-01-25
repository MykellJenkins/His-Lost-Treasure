using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProgressSaveData
{
    // 1. Metadata for Version Control and Cloud Sync
    public int version = 1;
    public long lastUpdatedTicks;

    // 2. Primary Data
    public string currentNodeId;
    public List<string> unlockedNodes = new();

    // 3. Runtime Lookup (Not Serialized)
    // We use this for high-performance checks during gameplay
    [NonSerialized] private HashSet<string> _lookupCache;

    public ProgressSaveData()
    {
        lastUpdatedTicks = DateTime.UtcNow.Ticks;
    }

    /// <summary>
    /// Checks if a node is unlocked. Very fast O(1) operation.
    /// </summary>
    public bool IsNodeUnlocked(string nodeId)
    {
        if (string.IsNullOrEmpty(nodeId)) return false;

        // Lazy-initialize the cache if it's null (after loading from JSON)
        _lookupCache ??= new HashSet<string>(unlockedNodes);
        return _lookupCache.Contains(nodeId);
    }

    /// <summary>
    /// Unlocks a node and keeps the list and cache in sync.
    /// </summary>
    public void UnlockNode(string nodeId)
    {
        if (IsNodeUnlocked(nodeId)) return;

        unlockedNodes.Add(nodeId);
        _lookupCache.Add(nodeId);
        lastUpdatedTicks = DateTime.UtcNow.Ticks;
    }

    /// <summary>
    /// Returns a human-readable date for UI (e.g., "Last Saved: 10/24/2026")
    /// </summary>
    public DateTime GetLastSaveDate() => new DateTime(lastUpdatedTicks);
}
