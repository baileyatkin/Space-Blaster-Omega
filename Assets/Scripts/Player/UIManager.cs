using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text scoreText;

    [SerializeField]
    private Sprite[] healthSprites;

    [SerializeField]
    private Image healthImg;

    [SerializeField]
    private GameObject gameOverPanel;

    [SerializeField]
    private Text gameOverText;

    [SerializeField]
    private Text endScoreText;

    [SerializeField]
    private Text endScore;

    [SerializeField]
    private Text endDifficultyText;

    [SerializeField]
    private Text endDifficulty;

    [SerializeField]
    private Text backToMenu;

    [SerializeField]
    private GameObject pausePanel;

    private bool gameOver = false;
    private Player player;
    private float playedTime;
    private Dictionary<int, float> finalWaveScores = new Dictionary<int, float>();

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        scoreText.text = "Score: " + 0;
        gameOverPanel.SetActive(false);
        gameOverText.gameObject.SetActive(false);
        endScoreText.gameObject.SetActive(false);
        endScore.gameObject.SetActive(false);
        endDifficultyText.gameObject.SetActive(false);
        endDifficulty.gameObject.SetActive(false);
        backToMenu.gameObject.SetActive(false);
        pausePanel.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver)
        {
            GameOverSequence();
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene("MainMenu");
            }
        }
    }

    public void UpdateScore(int playerScore)
    {
        scoreText.text = "Score: " + playerScore.ToString();
    }

    public void UpdateHealth(int playerHealth)
    {
        healthImg.sprite = healthSprites[playerHealth];
        if (playerHealth == 0)
        {
            gameOver = true;
        }
    }

    public void FinalScore(int playerScore)
    {
        endScore.text = playerScore.ToString();
    }

    public void FinalDifficulty(string difficulty)
    {
        endDifficulty.text = difficulty;
    }

    public void FinalTime(float playedTime)
    {
        this.playedTime = playedTime;
    }

    public void GetWaveScores(Dictionary<int, float> waveScores)
    {
        finalWaveScores = waveScores;
    }

    public void EnablePauseMenu()
    {
        Time.timeScale = 0;
        pausePanel.gameObject.SetActive(true);
    }


    public void DisablePauseMenu()
    {
        Time.timeScale = 1;
        pausePanel.gameObject.SetActive(false);
    }

    public void GameOverSequence()
    {
        Time.timeScale = 0;
        gameOverPanel.SetActive(true);
        gameOverText.gameObject.SetActive(true);
        endScoreText.gameObject.SetActive(true);
        endScore.gameObject.SetActive(true);
        endDifficultyText.gameObject.SetActive(true);
        endDifficulty.gameObject.SetActive(true);
        backToMenu.gameObject.SetActive(true);
        CreateText();
    }

    public void CreateText()
    {
        float finalScore = float.Parse(endScore.text);
        float convertedTime = (this.playedTime / 60) % 60;
        double playedMinutes = Math.Truncate(convertedTime);
        double playedSeconds = (convertedTime - Math.Truncate(convertedTime)) * 60;
        float averageScore = finalScore / (convertedTime);
        string endDiff = endDifficulty.text;
        string path = "TestData.txt";

        using (StreamWriter writer = new StreamWriter(path))
        {
            writer.WriteLine("Final Score: " + finalScore);
            writer.WriteLine("Time Played: " + playedMinutes + " minutes and " + playedSeconds + " seconds.");
            writer.WriteLine("Difficulty: " + endDiff);
            foreach (KeyValuePair<int, float> wave in finalWaveScores)
            {
                writer.WriteLine("Wave " + wave.Key + " Average Score per Minute: " + wave.Value);
            }
            writer.WriteLine("Final Average Score per Minute: " + averageScore);
        }

    }
}
