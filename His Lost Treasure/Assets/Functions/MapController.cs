using UnityEngine;
using UnityEngine.SceneManagement;

public class MapController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject player;
    [SerializeField] private Node startNode;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float snapDistance = 0.02f;

    private Node currentNode;
    private Node targetNode;
    private bool isMoving;

    void Start()
    {
        // Load the last node from progress or default to start
        currentNode = LoadCurrentNode() ?? startNode;
        player.transform.position = currentNode.transform.position;

        // Ensure start node is unlocked
        if (!NodeMapManager.Instance.IsNodeUnlocked(currentNode.NodeId))
            NodeMapManager.Instance.UnlockNode(currentNode.NodeId);
    }

    void Update()
    {
        if (isMoving)
        {
            MoveToNode();
            return;
        }

        HandleInput();
    }

    private void HandleInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");

        if (horizontal > 0.5f)
            TryMove(currentNode.GetNextNode());

        if (horizontal < -0.5f)
            TryMove(GetPreviousNode());

        if (Input.GetButtonDown("Jump"))
            SceneManager.LoadScene(currentNode.GetLevelName());
    }

    private void TryMove(Node next)
    {
        if (next == null) return;

        if (!NodeMapManager.Instance.IsNodeUnlocked(next.NodeId)) return;

        targetNode = next;
        isMoving = true;
    }

    private void MoveToNode()
    {
        player.transform.position = Vector3.MoveTowards(
            player.transform.position,
            targetNode.transform.position,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(player.transform.position, targetNode.transform.position) < snapDistance)
        {
            currentNode = targetNode;
            targetNode = null;
            isMoving = false;
            SaveCurrentNode();
        }
    }
    public Node GetCurrentNode()
    {
        return currentNode;
    }

    private void SaveCurrentNode()
    {
        ProgressSaveData data = SavePlayerData.Instance.LoadProgress() ?? new ProgressSaveData();
        data.currentNodeId = currentNode.NodeId;
        SavePlayerData.Instance.SaveProgress(data);
    }

    public Node LoadCurrentNode()
    {
        ProgressSaveData data = SavePlayerData.Instance.LoadProgress();
        if (data == null || string.IsNullOrEmpty(data.currentNodeId)) return null;

        foreach (Node node in FindObjectsByType<Node>(FindObjectsSortMode.None))
        {
            if (node.NodeId == data.currentNodeId)
                return node;
        }

        return null;
    }

    private Node GetPreviousNode()
    {
        // Find previous node by checking all nodes whose next node is current
        foreach (Node node in FindObjectsByType<Node>(FindObjectsSortMode.None))
        {
            if (node.GetNextNode() == currentNode)
                return node;
        }
        return null;
    }
}