using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MainManager : MonoBehaviour
{
    public Text ScoreText;
    public Text PlayerName;
    public Text BestScore;
    public GameObject GameOverText;

    public Transform playerTransform;
    public CameraController cameraMovement;

    private PlayerController playerController;

    public bool m_GameOver = false;
    private int m_Points;

    void Start()
    {
        cameraMovement.Setup(() => playerTransform.position);

        // Sets the text on the player name UI.
        PlayerName.text = "Player Name: " + WinnerList.instance.playerName;

        SetBestPlayer();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
    }
    void Update()
    {
        // Calls GameOver method if the player is out of life.
        if (playerController.life < 1)
        {
            GameOver();
        }

        // Lets player restart the game.
        if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    // Adds the amount of points gained from killing an enemy to the total score.
    public void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
        WinnerList.instance.score = m_Points;
    }

    // Calls the CheckBestPlayer method and activates the game over screen.
    public void GameOver()
    {
        m_GameOver = true;
        CheckBestPlayer();
        GameOverText.SetActive(true);
    }

    // Checks if run was a new best, then calls method to save it if it is.
    public void CheckBestPlayer()
    {
        if (WinnerList.instance.score >= WinnerList.instance.bestScore)
        {
            WinnerList.instance.bestPlayer = WinnerList.instance.playerName;
            WinnerList.instance.bestScore = WinnerList.instance.score;
        }
        WinnerList.instance.SaveWinnerData(WinnerList.instance.bestPlayer, WinnerList.instance.bestScore);
    }

    // Sets the text to be displayed.
    public void SetBestPlayer()
    {
        if (WinnerList.instance.bestPlayer == null && WinnerList.instance.bestScore == 0)
        {
            BestScore.text = " ";
        }
        else
        {
            BestScore.text = "Best Score: " + WinnerList.instance.bestPlayer + ": " + WinnerList.instance.bestScore;
        }
    }
}
