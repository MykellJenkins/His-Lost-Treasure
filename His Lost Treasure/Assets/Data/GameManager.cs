using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;
using UnityEngine.InputSystem;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private PlayerInputActions inputActions;

    [Header("Manager References")]
    public GameObject player;
    public Player playerScript;
    //public PlayerSaveSystem playersave;
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
    [SerializeField] AudioClip pauseMenuMusic;
    [SerializeField] AudioClip returnMenuMusic;
    //[SerializeField] float audioFadeDuration = 0.5f;

    private PlayerSaveSystem data;
    public bool isPaused { get; private set; }
    public bool isGameOver = false;
    //public bool endOfLevel = false;

    //private bool playerReady = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        data = PlayerSaveSystem.Instance;
        inputActions = new PlayerInputActions();


        if (rmInstance == null)
            rmInstance = RespawnManager.Instance;
    }

    void Start()
    {

        Debug.Log("Keyboard detected: " + (Keyboard.current != null));
        PlayerSaveSystem.Instance.LoadPlayerProgress();
        StartCoroutine(InitializePlayerCoroutine());
        Time.timeScale = 1f;
        Application.targetFrameRate = 30;
        if (gameplayMusic != null) gameplayMusic.Play();
        SetState(false);
    }

    // ---------------- INITIALIZATION ----------------
    private IEnumerator InitializePlayerCoroutine()
    {
        yield return null; // Wait one frame for all objects to initialize

        // Find player in scene
        //if (playerScript == null)
        //    playerScript = FindFirstObjectByType<Player>();

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

        //playerReady = true;
    }

    // ---------------- UPDATE ----------------
    void Update()
    {
        // if (!playerReady) return;

        //HandleInput();

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

        //if (endOfLevel == true && isGameOver == false)
        //{
        //    StateWin();
        //    endOfLevel = false;
        //}
    }

    // ---------------- INPUT ----------------
    //private void HandleInput()
    //{
    //    if (Input.anyKeyDown)
    //        Debug.Log("Key pressed");

    //    if (Input.GetKeyDown(KeyCode.P))
    //        Debug.Log("P pressed");

    //    if (Input.GetKeyDown(KeyCode.P))
    //    {
    //        if (!isPaused)
    //        {
    //            StatePause();
    //        }
    //        else
    //        {
    //            // If paused, always unpause unless in settings
    //            if (menuActive == menuSetting)
    //                StateBackToPause();
    //            else
    //                StateUnpause();
    //        }
    //    }
    //}
    private void OnPause(InputAction.CallbackContext context)
    {
        //Block pause if game is over
        if (isGameOver)
            return;

        if (!isPaused)
        {
            StatePause();
        }
        else
        {
            if (menuActive == menuSetting)
                StateBackToPause();
            else
                StateUnpause();
        }
    }

    // ---------------- GAME STATES ----------------
    public void StatePause()
    {
        if (isGameOver) return;
        Debug.Log("Paused state BEFORE: " + isPaused);
        SetState(true);
        playerScript.enabled = false;
        AudioManagement.instance.SwapTrack(pauseMenuMusic);
        ShowMenu(menuPause);
    }

    public void StateUnpause()
    {
        SetState(false);

        if (playerScript != null)
            playerScript.enabled = true;

        Rigidbody rb = playerScript.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        AudioManagement.instance.SwapTrack(returnMenuMusic);
        ShowMenu(null);
    }

    public void StateWin()
    {
        Debug.Log("STATE WIN ENTERED");
        if (isGameOver) return;
        isGameOver = true;
        inputActions.UI.Pause.Disable();
        SetState(true);
        ShowMenu(menuWin);
        currentNode?.CompleteLevel();

        // Get the PlayerSaveSystem instance when saving
        if (PlayerSaveSystem.Instance != null)
        {
            PlayerSaveSystem.Instance.SavePlayerProgress();
        }
        else
        {
            Debug.LogError("PlayerSaveSystem instance is null! Cannot save progress.");
        }

        ProgressSaveData datap = SavePlayerData.Instance.LoadProgress() ?? new ProgressSaveData();
        datap.currentNodeId = currentNode?.NodeId;
        SavePlayerData.Instance.SaveProgress(datap);
    }

    public void StateLose()
    {
        Debug.Log("StateLose called");
        if (isGameOver) return;
        isGameOver = true;

        Debug.Log("Setting state true");
        SetState(true);
        inputActions.UI.Pause.Disable();
        if (menuLose == null) Debug.LogError("menuLose not assigned!");

        Debug.Log("Showing Lose Menu");
        ShowMenu(menuLose);

        // Get the PlayerSaveSystem instance when saving
        if (PlayerSaveSystem.Instance != null)
        {
            PlayerSaveSystem.Instance.SavePlayerProgress();
        }
        else
        {
            Debug.LogError("PlayerSaveSystem instance is null! Cannot save progress.");
        }
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
        Debug.Log("ShowMenu called with: " + (menu != null ? menu.name : "NULL"));
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

    public void TriggerWin()
    {
        Debug.Log("TriggerWin called");

        if (isGameOver) return;
        StateWin();
    }

    public void RestartLevel()
    {
        // Hard reset global state
        Time.timeScale = 1f;
        isPaused = false;
        isGameOver = false;
        RespawnManager.Instance?.ResetCheckpoint();

        // Reset cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Reset input
        inputActions.UI.Pause.Enable();

        // Clear menus
        ShowMenu(null);

        // Reload scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnEnable()
    {
        if (inputActions != null)
            inputActions.UI.Pause.performed += OnPause;
        inputActions?.Enable();
    }

    private void OnDisable()
    {
        if (inputActions != null)
            inputActions.UI.Pause.performed -= OnPause;
        inputActions?.Disable();
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

