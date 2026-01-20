using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Node : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private string nodeId;
    [SerializeField] private string levelName;
    [SerializeField] private Node nextNode;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer icon;
    [SerializeField] private Color lockedColor = Color.gray;
    [SerializeField] private Color unlockedColor = Color.white;

    public string NodeId => nodeId;

    public void Refresh()
    {
        bool unlocked = NodeMapManager.Instance.IsNodeUnlocked(nodeId);
        if (icon != null)
            icon.color = unlocked ? unlockedColor : lockedColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (!NodeMapManager.Instance.IsNodeUnlocked(nodeId)) return;

        // Set current node in GameManager before loading level
        GameManager.Instance.currentNode = this;

        SceneManager.LoadScene(levelName);
    }

    public void CompleteLevel()
    {
        if (nextNode != null)
        {
            Debug.Log($"Unlocking next node: {nextNode.nodeId}");
            NodeMapManager.Instance.UnlockNode(nextNode.nodeId);
        }
        else
        {
            Debug.LogWarning("Next node is null for node: " + nextNode);
        }
    }

    public Node GetNextNode() => nextNode;
    public string GetLevelName() => levelName;
}


