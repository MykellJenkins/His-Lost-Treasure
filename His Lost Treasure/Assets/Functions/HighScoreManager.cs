using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HighScoreManager : MonoBehaviour
{
    public static HighScoreManager Instance { get; private set; }

    public Text scoreText;
    public Text highScoreText;
    private int score;
    private int badScore;
    public int points;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        score = 0;
        points = 0;
        badScore = 0;
    }

    void Update()
    {
        Scoring();
    }

    IEnumerator Scoring()
    {
        yield return new WaitForSeconds(1f); // Wait for one second
        if (GameManager.Instance.isGameOver == false) score += 100; else badScore++; // Increment the score
        //StartCoroutine(count()); // Loop back to the start of the method
    }

    public void AddPoints(int amount)
    {
        points += amount;
        Debug.Log("Added points: " + amount);

    }

    void UpdateScore()
    {
        scoreText.text = "Score: " + score.ToString();
    }

    void CheckHighScore()
    {
        if (score > PlayerPrefs.GetInt("HighScore", 0))
        {
            PlayerPrefs.SetInt("HighScore", score);
            UpdateHighScore();
        }
    }

    void UpdateHighScore()
    {
        highScoreText.text = "High Score: " + PlayerPrefs.GetInt("HighScore", 0).ToString();
    }
}
