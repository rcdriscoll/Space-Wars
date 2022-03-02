using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
    [SerializeField] float delayInSeconds = 2f;
    GameSession gameSession;

    public void LoadStartMenu()
    {
        //loads the first scene (start menu)
        SceneManager.LoadScene(0);
    }

    //onlyl one game scene so no parameters
    //look at block breaker to see how to load different scenes
    public void LoadGame()
    {
        //uses a string reference instead of the index of order of levels
        SceneManager.LoadScene("Game");

        gameSession = FindObjectOfType<GameSession>();

        if(gameSession)
        {
            gameSession.ResetGame();

        }
    }

    public void LoadGameOver()
    {
        StartCoroutine(WaitAndLoad());
    }

    IEnumerator WaitAndLoad()
    {
        yield return new WaitForSeconds(delayInSeconds);
        SceneManager.LoadScene("End Game");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
