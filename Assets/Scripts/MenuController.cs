using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public static string gameDifficulty;
    public static string gameMode;
    public static bool dynamicEnabled;
    public void NewGameEasy()
    {
        gameDifficulty = "Easy";
        gameMode = "Standard";
        dynamicEnabled = false;
        SceneManager.LoadScene("GameScene");
    }

    public void NewGameStandard()
    {
        gameDifficulty = "Standard";
        gameMode = "Standard";
        dynamicEnabled = false;
        SceneManager.LoadScene("GameScene");
    }

    public void NewGameHard()
    {
        gameDifficulty = "Hard";
        gameMode = "Standard";
        dynamicEnabled = false;
        SceneManager.LoadScene("GameScene");
    }

    public void NewGameDynamic()
    {
        gameDifficulty = "Standard";
        gameMode = "Standard";
        dynamicEnabled = true;
        SceneManager.LoadScene("GameScene");
    }

    public void DevMode()
    {
        gameDifficulty = "Standard";
        gameMode = "Dev";
        dynamicEnabled = false;
        SceneManager.LoadScene("GameScene");
    }
}
