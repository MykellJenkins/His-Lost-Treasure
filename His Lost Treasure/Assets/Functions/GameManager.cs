using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;

    public GameObject player;
    public Player playerScript;

    public bool isPaused = false;
    public bool endOfLevel;
    
    float timeScaleOG;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        timeScaleOG = Time.timeScale;
        endOfLevel = false;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if(menuActive == null)
            {
                statePause();
            }
            else if(menuActive == menuPause)
            {
                stateUnpause();
            }
        }
        else if(GameManager.instance.playerScript.maxLives == 0)
        {
            stateLose();
        }
        else if(endOfLevel == true)
        {
            stateWin();
        }
    }

    public void statePause()
    {
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        menuActive = menuPause;
        menuActive.SetActive(true);
    }

    public void stateUnpause()
    {
        isPaused = false;
        Time.timeScale = timeScaleOG;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }

    public void stateLose()
    {
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public void stateWin()
    {
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        menuActive = menuWin;
        menuActive.SetActive(true);
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
}
