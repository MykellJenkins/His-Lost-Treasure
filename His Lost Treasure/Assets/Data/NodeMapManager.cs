using System.Collections.Generic;
using UnityEngine;

public class NodeMapManager : MonoBehaviour
{
    public static NodeMapManager Instance;

    private HashSet<string> unlockedNodes = new();  // Fast lookup of unlocked nodes

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadProgress();  // Load saved unlocked nodes on start
    }

    // Check if a node is unlocked
    public bool IsNodeUnlocked(string nodeId)
    {
        return unlockedNodes.Contains(nodeId);
    }

    // Unlock a node by ID
    public void UnlockNode(string nodeId)
    {
        if (unlockedNodes.Contains(nodeId)) return;

        unlockedNodes.Add(nodeId);
        SaveProgress();
        RefreshAllNodes();  // Update visuals for all nodes
    }

    // Refresh visuals for every node
    private void RefreshAllNodes()
    {
        foreach (Node node in FindObjectsByType<Node>(FindObjectsSortMode.None))
            node.Refresh();
    }

    // Save unlocked nodes and current node
    public void SaveProgress()
    {
        // Load existing progress or create new
        ProgressSaveData data = SavePlayerData.Instance.LoadProgress() ?? new ProgressSaveData();
        data.unlockedNodes = new List<string>(unlockedNodes);

        // Save current node if MapController exists
        var mapControllers = FindObjectsByType<MapController>(FindObjectsSortMode.None);
        if (mapControllers != null && mapControllers.Length > 0)
        {
            var currentNode = mapControllers[0].GetCurrentNode();
            data.currentNodeId = currentNode != null ? currentNode.name : null;
        }
           

        SavePlayerData.Instance.SaveProgress(data);
    }

    // Load unlocked nodes from save
    public void LoadProgress()
    {
        ProgressSaveData data = SavePlayerData.Instance.LoadProgress();
        if (data != null && data.unlockedNodes != null)
        {
            unlockedNodes = new HashSet<string>(data.unlockedNodes);
        }
    }
}