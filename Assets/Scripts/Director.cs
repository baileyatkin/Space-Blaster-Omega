using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FuzzyLogic;
using UnityEngine.SceneManagement;

public class Director : MonoBehaviour
{

    public Enemies[] easyWaves;
    public Enemies[] mediumWaves;
    public Enemies[] hardWaves;

    public SpeederEnemy speederPrefab;

    public GameObject spawnLeft;
    public GameObject spawnMiddle;
    public GameObject spawnRight;

    public float playTimeSeconds = 0;
    public float playTimeMinutes = 0;

    private Player player;
    private FuzzyVariable averageScore;
    private FuzzyVariable playerSide;
    private UIManager uiManager;

    public string gameDifficulty;
    public string gameMode;
    public bool dynamicEnabled;

    private bool waveAlive;
    private bool gameOver;
    private bool gameStarted;
    private bool paused;
    private bool speederSpawned;

    private int healthLost;
    private int playerHealth;
    private int currentDifficulty;
    private int wave;

    private Dictionary<int, float> waveScorePerMinute = new Dictionary<int, float>();
    private Dictionary<int, string> difficulties = new Dictionary<int, string>();

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        gameMode = MenuController.gameMode;
        gameDifficulty = MenuController.gameDifficulty;
        dynamicEnabled = MenuController.dynamicEnabled;
        gameStarted = false;

        if (dynamicEnabled)
        {
            difficulties[1] = "Easy";
            difficulties[2] = "Standard";
            difficulties[3] = "Hard";

            averageScore = new FuzzyVariable();

            FuzzySetTrapezoid lowScore = new FuzzySetTrapezoid("low", 0.0f, 0.0f, 525.0f, 575.0f);
            FuzzySetTriangle mediumScore = new FuzzySetTriangle("medium", 565.0f, 650.0f, 725.0f);
            FuzzySetTrapezoid highScore = new FuzzySetTrapezoid("high", 700.0f, 775.0f, 9999999999.0f, 9999999999.0f);
            averageScore.AddFuzzySet("low", lowScore);
            averageScore.AddFuzzySet("medium", mediumScore);
            averageScore.AddFuzzySet("high", highScore);

            gameDifficulty = "Standard";
        }
        //Set up fuzzy logic for deciding where to send Speeder Ship
        playerSide = new FuzzyVariable();
        FuzzySetTrapezoid leftSide = new FuzzySetTrapezoid("left", -16.0f, -15.0f, -8.0f, -4.0f);
        FuzzySetTrapezoid middle = new FuzzySetTrapezoid("middle", -5.0f, -3.0f, 3.0f, 5.0f);
        FuzzySetTrapezoid rightSide = new FuzzySetTrapezoid("right", 4.0f, 8.0f, 15.0f, 16.0f);
        playerSide.AddFuzzySet("left", leftSide);
        playerSide.AddFuzzySet("middle", middle);
        playerSide.AddFuzzySet("right", rightSide);

        if (gameDifficulty == "Easy")
        {
            currentDifficulty = 1;
        }
        else if (gameDifficulty == "Standard")
        {
            currentDifficulty = 2;
        }
        else if (gameDifficulty == "Hard")
        {
            currentDifficulty = 3;
        }

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        gameOver = false;
        Debug.Log("Director Loaded");
        StartCoroutine(PlayTimer());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!paused)
            {
                paused = true;
                uiManager.EnablePauseMenu();
            }
            else
            {
                uiManager.FinalDifficulty(gameDifficulty);
                uiManager.FinalTime(playTimeSeconds);
                uiManager.FinalScore(player.score);
                uiManager.GetWaveScores(waveScorePerMinute);
                uiManager.CreateText();
                SceneManager.LoadScene("MainMenu");
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && paused)
        {
            uiManager.DisablePauseMenu();
            paused = false;
        }

        if (!waveAlive)
        {
            if (!gameStarted)
            {
                healthLost = 0;
                wave = 0;
                playerHealth = player.currentHealth;
                StartCoroutine(SpawnWave());
                gameStarted = true;
                Debug.Log("GAME STARTED");
            }
            else
            {
                float avgScore = player.score / playTimeMinutes;
                waveScorePerMinute[wave] = avgScore;
                if (dynamicEnabled)
                {
                    averageScore.SetDOM(avgScore);
                    string fuzzyScoreOutput = averageScore.GetHighestDOM();
                    //Fuzzy Rules, in a more complicated system, they would have their own class
                    if (fuzzyScoreOutput == "high" && healthLost >= 1)
                    {
                        Debug.Log("Score High + Health lost, no change in difficulty");
                    }
                    else if (fuzzyScoreOutput == "high" && healthLost == 0)
                    {
                        Debug.Log("Score High + No Health lost, increased difficulty");
                        if (currentDifficulty != 3)
                        {
                            currentDifficulty++;
                        }
                    }
                    else if (fuzzyScoreOutput == "medium" && healthLost > 1)
                    {
                        Debug.Log("Score Medium + Health lost, decrease difficulty");
                        if (currentDifficulty != 1)
                        {
                            currentDifficulty--;
                        }
                    }
                    else if (fuzzyScoreOutput == "medium" && healthLost <= 1)
                    {
                        Debug.Log("Score Medium + No Health lost, no change in difficulty");
                    }
                    else if (fuzzyScoreOutput == "low" && healthLost >= 1)
                    {
                        Debug.Log("Score Low + Health lost, difficulty decreased");
                        if (currentDifficulty != 1)
                        {
                            currentDifficulty--;
                        }
                    }
                    else if (fuzzyScoreOutput == "low" && healthLost == 0)
                    {
                        Debug.Log("Score Low + No Health lost, difficulty decreased");
                        if (currentDifficulty != 1)
                        {
                            currentDifficulty--;
                        }
                    }

                    gameDifficulty = difficulties[currentDifficulty];
                }
                healthLost = 0;
                playerHealth = player.currentHealth;
                StartCoroutine(SpawnWave());
            }
        }
        else
        {
            if (player.currentHealth != 0 && player.currentHealth < playerHealth)
            {
                healthLost = playerHealth - player.currentHealth;
            }
        }

        if (player.currentHealth == 0)
        {
            uiManager.FinalDifficulty(gameDifficulty);
            uiManager.FinalTime(playTimeSeconds);
            uiManager.GetWaveScores(waveScorePerMinute);
            gameOver = true;
        }
    }


    private IEnumerator PlayTimer()
    {
        while (!gameOver)
        {
            yield return new WaitForSeconds(1);
            playTimeSeconds += 1;
            playTimeMinutes = (playTimeSeconds / 60) % 60;
        }
    }

    private IEnumerator SpawnWave()
    {
        waveAlive = true;
        yield return new WaitForSeconds(3.0f);

        if (gameDifficulty == "Easy")
        {
            int selectedWave = Random.Range(0, 6);
            Enemies newWave = Instantiate(this.easyWaves[selectedWave], this.transform.position, Quaternion.identity);
            newWave.currentDifficulty = gameDifficulty;
            newWave.waveKilled += WaveKilled;
            newWave.spawnSpeeder += SpawnSpeeder;
        }
        else if (gameDifficulty == "Standard")
        {
            int selectedWave = Random.Range(0, 6);
            Enemies newWave = Instantiate(this.mediumWaves[selectedWave], this.transform.position, Quaternion.identity);
            newWave.currentDifficulty = gameDifficulty;
            newWave.waveKilled += WaveKilled;
            newWave.spawnSpeeder += SpawnSpeeder;
        }
        else if (gameDifficulty == "Hard")
        {
            int selectedWave = Random.Range(0, 6);
            Enemies newWave = Instantiate(this.hardWaves[selectedWave], this.transform.position, Quaternion.identity);
            newWave.currentDifficulty = gameDifficulty;
            newWave.waveKilled += WaveKilled;
            newWave.spawnSpeeder += SpawnSpeeder;
        }
        speederSpawned = false;
        wave++;
    }

    private void WaveKilled()
    {
        waveAlive = false;
    }

    private void SpawnSpeeder()
    {
        if (!speederSpawned)
        {
            if (gameDifficulty == "Hard")
            {
                Vector3 playerPos = player.transform.position;
                float playerPosX = playerPos.x;
                playerSide.SetDOM(playerPosX);
                string fuzzyPosOutput = playerSide.GetHighestDOM();
                Debug.Log(fuzzyPosOutput);
                if (fuzzyPosOutput == "left")
                {
                    SpeederEnemy speeder = Instantiate(this.speederPrefab, spawnRight.transform.position, Quaternion.identity);
                }
                else if (fuzzyPosOutput == "middle")
                {
                    int spawnNum = Random.Range(1, 2);
                    if (spawnNum == 1)
                    {
                        SpeederEnemy speeder = Instantiate(this.speederPrefab, spawnRight.transform.position, Quaternion.identity);
                    }
                    else
                    {
                        SpeederEnemy speeder = Instantiate(this.speederPrefab, spawnLeft.transform.position, Quaternion.identity);
                    }
                }
                else if (fuzzyPosOutput == "right")
                {
                    SpeederEnemy speeder = Instantiate(this.speederPrefab, spawnLeft.transform.position, Quaternion.identity);
                }
            }
            else
            {
                int spawnNum = Random.Range(1, 3);
                if (spawnNum == 1)
                {
                    SpeederEnemy speeder = Instantiate(this.speederPrefab, spawnLeft.transform.position, Quaternion.identity);
                }
                else if (spawnNum == 2)
                {
                    SpeederEnemy speeder = Instantiate(this.speederPrefab, spawnMiddle.transform.position, Quaternion.identity);
                }
                else
                {
                    SpeederEnemy speeder = Instantiate(this.speederPrefab, spawnRight.transform.position, Quaternion.identity);
                }
            }

            speederSpawned = true;
        }
    }
}
