using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapController : MonoBehaviour
{
    public static MapController Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject player;
    [SerializeField] private Node startNode;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;      // speed for node movement
    [SerializeField] private float snapDistance = 0.05f;

    private Node currentNode;
    private Node targetNode;
    private bool isMoving;

    private Dictionary<string, Node> nodeLookup = new();

    void Awake()
    {
        player = player != null ? player : GameObject.FindWithTag("Player");
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        CacheNodes();
        LoadNodeOrFallback();
        StartCoroutine(SpawnPlayerAtNode());
        UnlockStartNode();
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

    // ---------------- NODE CACHING ----------------
    void CacheNodes()
    {
        Node[] nodes = FindObjectsByType<Node>(FindObjectsSortMode.None);
        foreach (Node node in nodes)
        {
            if (!nodeLookup.ContainsKey(node.NodeId))
                nodeLookup.Add(node.NodeId, node);
        }
    }

    void LoadNodeOrFallback()
    {
        currentNode = startNode;

        ProgressSaveData data = SavePlayerData.Instance?.LoadProgress();
        if (data != null && !string.IsNullOrEmpty(data.currentNodeId) && nodeLookup.TryGetValue(data.currentNodeId, out Node savedNode))
            currentNode = savedNode;

        if (currentNode == null)
            Debug.LogError("MapController: No start node assigned!");
    }

    IEnumerator SpawnPlayerAtNode()
    {
        yield return null; // wait one frame

        if (player != null && currentNode != null)
        {
            CharacterController controller = player.GetComponent<CharacterController>();
            Rigidbody rb = player.GetComponent<Rigidbody>();

            if (controller != null) controller.enabled = false;
            if (rb != null) { rb.linearVelocity = Vector3.zero; rb.angularVelocity = Vector3.zero; }

            player.transform.position = currentNode.transform.position;

            // Set checkpoint
            RespawnManager.Instance.SetCheckPoint(currentNode.transform.position);

            if (controller != null) controller.enabled = true;
        }
    }

    void UnlockStartNode()
    {
        if (!NodeMapManager.Instance.IsNodeUnlocked(currentNode.NodeId))
            NodeMapManager.Instance.UnlockNode(currentNode.NodeId);
    }

    public Node GetCurrentNode() => currentNode;

    // ---------------- INPUT / NAVIGATION ----------------
    void HandleInput()
    {
        if (isMoving) return; // prevent input while moving

        // Move to next node (Right / D)
        if (Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            Node next = currentNode.GetNextNode();
            if (next != null && NodeMapManager.Instance.IsNodeUnlocked(next.NodeId))
                SetTargetNode(next);
        }

        // Move to previous node (Left / A)
        if (Input.GetKeyDown(KeyCode.L) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Node prev = GetPreviousNode();
            if (prev != null)
                SetTargetNode(prev);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            if (currentNode != null)
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.currentNode = currentNode;
                    SceneManager.LoadScene(currentNode.GetLevelName());
                }
                else
                {
                    Debug.LogError("GameManager instance is null! Make sure GameManager exists in the scene.");
                }
            }
            else
            {
                Debug.LogError("Current node is null! Cannot load level.");
            }
        }
    }

    void SetTargetNode(Node node)
    {
        targetNode = node;
        isMoving = true;
    }

    void MoveToNode()
    {
        if (player == null || targetNode == null) return;

        // If player has a CharacterController, disable it for manual movement
        CharacterController controller = player.GetComponent<CharacterController>();
        Rigidbody rb = player.GetComponent<Rigidbody>();

        if (controller != null) controller.enabled = false;
        if (rb != null) rb.linearVelocity = Vector3.zero;

        // Smooth move
        player.transform.position = Vector3.MoveTowards(player.transform.position,
            targetNode.transform.position,
            moveSpeed * Time.deltaTime);

        // Snap to target
        if (Vector3.Distance(player.transform.position, targetNode.transform.position) <= snapDistance)
        {
            currentNode = targetNode;
            targetNode = null;
            isMoving = false;

            // Update checkpoint
            RespawnManager.Instance.SetCheckPoint(currentNode.transform.position);

            if (controller != null) controller.enabled = true;
        }
        else
        {
            if (controller != null) controller.enabled = true;
        }
    }

    Node GetPreviousNode()
    {
        foreach (var node in nodeLookup.Values)
        {
            if (node.GetNextNode() == currentNode)
                return node;
        }
        return null;
    }
}

