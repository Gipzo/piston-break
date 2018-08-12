using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : Singleton<GameState>
{

    public LevelSettings CurrentLevel;
    public LevelSettings NextLevel;
    public GameConfiguration GameConfig;
    public bool DebugLevel = false;
    public double CurrentLevelTime = 0.0;

    // Use this for initialization
    void Start()
    {

    }

    public void DebugInit()
    {
        GameConfig = Resources.Load<GameConfiguration>("GameConfig");
        CurrentLevel = GameConfig.Levels[0];
        NextLevel = GameConfig.Levels[1];
        CurrentLevelTime = 0.0;
    }

    public void OpenNextLevel()
    {
        for (int i = 0; i < GameConfig.Levels.Length - 1; i++)
        {
            if (CurrentLevel == GameConfig.Levels[i])
            {
                GameConfig.Levels[i + 1].Opened = true;
                NextLevel = GameConfig.Levels[i + 1];
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}