using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Manager References")]
    public Player playerScript;
    public Node currentNode;
    public RespawnManager rmInstance; // Added RespawnManager reference

    [Header("UI Menus")]
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuSetting;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    private GameObject menuActive;

    [Header("Audio")]
    [SerializeField] AudioSource gameplayMusic;
    [SerializeField] AudioSource pauseMenuMusic;
    [SerializeField] float audioFadeDuration = 0.5f;

    public bool isPaused { get; private set; }
    public bool isGameOver = false;
    public bool endOfLevel = false;

    private bool playerReady = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Assign RespawnManager singleton if not set
        if (rmInstance == null) rmInstance = RespawnManager.Instance;
    }

    void Start()
    {
        StartCoroutine(InitializePlayerCoroutine());
        Time.timeScale = 1f;

        if (gameplayMusic != null) gameplayMusic.Play();
    }

    // ---------------- INITIALIZATION ----------------
    private IEnumerator InitializePlayerCoroutine()
    {
        yield return null; // Wait one frame for all objects to initialize

        // Find player in scene
        if (playerScript == null)
            playerScript = FindFirstObjectByType<Player>();

        if (playerScript == null)
        {
            Debug.LogError("GameManager: Player not found in scene!");
            yield break;
        }

        // Load save data
        PlayerSaveData loadedPlayerData = SavePlayerData.Instance.LoadPlayer();
        if (loadedPlayerData != null)
            playerScript.LoadFromSave(loadedPlayerData);
        else
            playerScript.ResetPlayer();

        // Set checkpoint at current NodeMap node
        if (MapController.Instance != null)
        {
            currentNode = MapController.Instance.GetCurrentNode();

            if (rmInstance != null)
                rmInstance.SetCheckPoint(currentNode.transform.position);

            // Move player to node
            CharacterController controller = playerScript.GetComponent<CharacterController>();
            Rigidbody rb = playerScript.GetComponent<Rigidbody>();

            if (controller != null) controller.enabled = false;
            if (rb != null) { rb.linearVelocity = Vector3.zero; rb.angularVelocity = Vector3.zero; }

            playerScript.transform.position = currentNode.transform.position;

            if (controller != null) controller.enabled = true;
        }

        playerReady = true;
    }

    // ---------------- UPDATE ----------------
    void Update()
    {
        if (!playerReady) return;

        HandleInput();

        if (isPaused || isGameOver) return;

        if (playerScript != null && rmInstance != null)
        {
            // Player dies
            if (playerScript.maxLives <= 0)
            {
                StateLose();
            }
            // Player hurt ? respawn
            else if (playerScript.isHurt)
            {
                rmInstance.RespawnPlayer(playerScript);
                playerScript.isHurt = false;
            }
        }

        if (endOfLevel && !isGameOver)
        {
            StateWin();
            endOfLevel = false;
        }
    }

    // ---------------- INPUT ----------------
    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!isPaused) StatePause();
            else if (menuActive == menuPause) StateUnpause();
            else if (menuActive == menuSetting) StateBackToPause();
        }
    }

    // ---------------- GAME STATES ----------------
    public void StatePause()
    {
        SetState(true);
        StartCoroutine(FadeAudio(gameplayMusic, pauseMenuMusic, audioFadeDuration));
        ShowMenu(menuPause);
    }

    public void StateUnpause()
    {
        SetState(false);
        StartCoroutine(FadeAudio(pauseMenuMusic, gameplayMusic, audioFadeDuration));
        ShowMenu(null);
    }

    public void StateWin()
    {
        if (isGameOver) return;
        isGameOver = true;

        SetState(true);
        gameplayMusic.Stop();
        pauseMenuMusic.Stop();

        // Unlock next node
        currentNode?.CompleteLevel();

        // Save progress: current node is this one
        ProgressSaveData data = SavePlayerData.Instance.LoadProgress() ?? new ProgressSaveData();
        data.currentNodeId = currentNode?.NodeId;
        SavePlayerData.Instance.SaveProgress(data);

        ShowMenu(menuWin);
    }

    public void StateLose()
    {
        if (isGameOver) return;
        isGameOver = true;

        SetState(true);
        gameplayMusic?.Stop();
        pauseMenuMusic?.Stop();

        ShowMenu(menuLose);
    }

    // ---------------- HELPERS ----------------
    private void SetState(bool paused)
    {
        isPaused = paused;
        Time.timeScale = paused ? 0f : 1f;
        Cursor.visible = paused;
        Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void ShowMenu(GameObject menu)
    {
        if (menuActive != null) menuActive.SetActive(false);
        menuActive = menu;
        if (menuActive != null) menuActive.SetActive(true);
    }

    public void StateSettings() => ShowMenu(menuSetting);
    public void StateBackToPause() => ShowMenu(menuPause);

    private IEnumerator FadeAudio(AudioSource from, AudioSource to, float duration)
    {
        float t = 0f;
        if (!to.isPlaying) to.Play();

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            if (from != null) from.volume = Mathf.Lerp(1, 0, t / duration);
            if (to != null) to.volume = Mathf.Lerp(0, 1, t / duration);
            yield return null;
        }
        from?.Pause();
    }
}



// If we need a goal count
//
//public void updateGameGoal(int amount)
//{
//    gameGoalCount += amount;
//    if(gameGoalCount <= 0)
//    {
//        //you win
//        statePause();
//        menuActive = menuWin;
//        menuActive.SetActive(true);
//    }
//}

