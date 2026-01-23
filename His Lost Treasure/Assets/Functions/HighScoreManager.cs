using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HighScoreManager : MonoBehaviour
{
    public Text scoreText;
    public Text highScoreText;
    private int score;
    private int badScore;

    void Start()
    {
        score = 0;
    }

    void Update()
    {
        
    }

    IEnumerator Scoring()
    {
        yield return new WaitForSeconds(1f); // Wait for one second
        if (GameManager.Instance.isGameOver == false) score += 10; else badScore++; // Increment the score
        //StartCoroutine(count()); // Loop back to the start of the method
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
