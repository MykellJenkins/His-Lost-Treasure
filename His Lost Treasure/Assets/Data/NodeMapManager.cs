using System.Collections.Generic;
using UnityEngine;

public class NodeMapManager : MonoBehaviour
{
    public static NodeMapManager Instance { get; private set; }

    [SerializeField] private string startNodeId; // assign the first node in inspector

    private HashSet<string> unlockedNodes = new();
    private Dictionary<string, Node> nodeLookup = new();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // Cache nodes
        Node[] nodes = FindObjectsByType<Node>(FindObjectsSortMode.None);
        foreach (Node node in nodes)
        {
            if (!nodeLookup.ContainsKey(node.NodeId))
                nodeLookup.Add(node.NodeId, node);
        }

        LoadProgress();

        // Ensure at least one node is unlocked
        if (unlockedNodes.Count == 0 && !string.IsNullOrEmpty(startNodeId))
        {
            unlockedNodes.Add(startNodeId);
            SaveProgress();
        }

        RefreshAllNodes();
    }

    // ---------------- PUBLIC ----------------
    public bool IsNodeUnlocked(string nodeId) => unlockedNodes.Contains(nodeId);

    public void UnlockNode(string nodeId)
    {
        if (!unlockedNodes.Contains(nodeId))
        {
            unlockedNodes.Add(nodeId);
            SaveProgress();
        }

        RefreshAllNodes(); // update all node visuals
    }

    // ---------------- SAVE / LOAD ----------------
    public void SaveProgress()
    {
        ProgressSaveData data = SavePlayerData.Instance.LoadProgress() ?? new ProgressSaveData();
        data.unlockedNodes = new List<string>(unlockedNodes);

        MapController map = FindFirstObjectByType<MapController>();
        if (map != null && map.GetCurrentNode() != null)
            data.currentNodeId = map.GetCurrentNode().NodeId;

        SavePlayerData.Instance.SaveProgress(data);
    }

    private void LoadProgress()
    {
        ProgressSaveData data = SavePlayerData.Instance.LoadProgress();
        if (data != null && data.unlockedNodes != null)
            unlockedNodes = new HashSet<string>(data.unlockedNodes);
    }

    // ---------------- VISUALS ----------------
    private void RefreshAllNodes()
    {
        foreach (Node node in nodeLookup.Values)
            node.Refresh();
    }
}