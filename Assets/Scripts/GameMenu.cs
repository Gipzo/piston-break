using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{

    static private GameMenu _instance;
    static public GameMenu Instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<GameMenu>();
            return _instance;
        }
    }

    public Text WinTime;
    public GameObject WinRecord;
    public GameObject PausePanel;
    public GameObject NextLevelButton;
    public GameObject LoadingPanel;
    public Text StatusText;
    public GameObject WinPanel;
    public GameObject LostPanel;

    void Start()
    {
    }

    public void Init()
    {

        WinPanel.SetActive(false);
        LostPanel.SetActive(false);
        PausePanel.SetActive(false);
        StatusText.text = "PRESS ANY KEY TO START";
        LoadingPanel.SetActive(true);
    }

    public void ShowLevel()
    {
        LoadingPanel.SetActive(false);
    }

    public void ShowPause()
    {
        PausePanel.SetActive(true);
    }
    public void HidePause()
    {
        PausePanel.SetActive(false);
    }

    public void StartGame()
    {
        Debug.Log("Start");
        StatusText.text = "";
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameState.Instance != null)
        {
            StatusText.text = LevelSettings.SecondsToTime(GameState.Instance.CurrentLevelTime);
        }
        else
        {
            StatusText.text = "";
        }

    }


    public void ShowWin(bool record = false)
    {
        WinTime.text = LevelSettings.SecondsToTime(GameState.Instance.CurrentLevelTime);
        WinRecord.SetActive(record);
        WinPanel.SetActive(true);
        NextLevelButton.SetActive(GameState.Instance.NextLevel != null);
    }

    public void ShowLost()
    {
        LostPanel.SetActive(true);
    }

    public void NextLevel()
    {
        GameManager.Instance.NextLevel();
    }

    public void Retry()
    {
        GameManager.Instance.RestartLevel();
    }

    public void Menu()
    {
        GameManager.Instance.GoMenu();

    }
}
