using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
    static private Level _instance;
    static public Level Instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<Level>();
            return _instance;
        }
    }


    private List<Piston> _pistons;
    public float LevelTime;

    public bool Loaded = false;
    public bool LevelStarted
    {
        get
        {
            if (GameManager.Instance == null)
                return false;
            return GameManager.Instance.LevelStarted;
        }
    }


    // Use this for initialization
    void Start()
    {
        if (GameManager.Instance == null)
        {
            GameState.Instance.DebugInit();
            GameState.Instance.DebugLevel = true;
            SceneManager.LoadScene("MainLevel", LoadSceneMode.Additive);
        }
        _pistons = new List<Piston>();
        _pistons.AddRange(gameObject.GetComponentsInChildren<Piston>());
    }


    // Update is called once per frame
    void Update()
    {
        if (!Loaded)
        {
            Loaded = true;
            if (GameManager.Instance == null)
                return;
            GameManager.Instance.AllPistons = _pistons;
            foreach (var p in _pistons)
            {
                if (!p.Loaded) Loaded = false;
            }
            if (Loaded)
            {

                foreach (var p in _pistons)
                {
                    p.Value = 0;
                    p.UpdateSize();
                }
                GameManager.Instance.LevelLoaded();
            }
        }
    }
}
