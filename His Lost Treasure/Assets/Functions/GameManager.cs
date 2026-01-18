using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuSetting;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] AudioSource gameplayMusic;
    [SerializeField] AudioSource pauseMenuMusic;

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

        AudioListener.volume = PlayerPrefs.GetFloat("Volume", 5f);
        gameplayMusic.Play();
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
            else if (menuActive == menuSetting)
            {
                stateBackToPause();
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
        
        StartCoroutine(FadeAudio(gameplayMusic, pauseMenuMusic, 0.5f));

        menuActive = menuPause;
        menuActive.SetActive(true);
    }

    public void stateUnpause()
    {
        isPaused = false;
        Time.timeScale = timeScaleOG;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
      
        StartCoroutine(FadeAudio(gameplayMusic, pauseMenuMusic, 0.5f));

        menuActive.SetActive(false);
        menuActive = null;
    }
    public void stateSettings()
    {
        menuActive.SetActive(false);
        menuActive = menuSetting;
        menuActive.SetActive(true);
    }

    //  BACK TO PAUSE MENU
    public void stateBackToPause()
    {
        menuActive.SetActive(false);
        menuActive = menuPause;
        menuActive.SetActive(true);
    }

    public void stateLose()
    {
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        gameplayMusic.Stop();
        pauseMenuMusic.Stop();

        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public void stateWin()
    {
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        gameplayMusic.Stop();
        pauseMenuMusic.Stop();

        menuActive = menuWin;
        menuActive.SetActive(true);
    }



    // VOLUME SLIDER
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("Volume", volume);
    }

    IEnumerator FadeAudio(AudioSource from, AudioSource to, float duration)
    {
        float t = 0;
        to.volume = 0;
        to.Play();

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            from.volume = Mathf.Lerp(1, 0, t / duration);
            to.volume = Mathf.Lerp(0, 1, t / duration);
            yield return null;
        }

        from.Pause();
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
