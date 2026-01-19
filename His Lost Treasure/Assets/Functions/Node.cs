using UnityEngine;
using UnityEngine.SceneManagement;

public class Node : MonoBehaviour
{
    [Header("Identity")]
    [SerializeField] private string nodeId;
    [SerializeField] private string levelName;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer icon;
    [SerializeField] private Color lockedColor = Color.gray;
    [SerializeField] private Color unlockedColor = Color.white;

    [Header("Connections")]
    [SerializeField] private Node nextNode;

    public string NodeId => nodeId;

    void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        bool unlocked = NodeMapManager.Instance.IsNodeUnlocked(nodeId);
        icon.color = unlocked ? unlockedColor : lockedColor;
    }

    void OnMouseDown()
    {
        if (!NodeMapManager.Instance.IsNodeUnlocked(nodeId)) return;

        SceneManager.LoadScene(levelName);
    }

    // Call this when player completes the level
    public void CompleteLevel()
    {
        if (nextNode != null)
        {
            NodeMapManager.Instance.UnlockNode(nextNode.NodeId);
        }
    }

    public string GetLevelName()
    {
        return levelName;
    }

    public Node GetNextNode()
    {
        return nextNode;
    }
}


