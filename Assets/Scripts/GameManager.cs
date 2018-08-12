using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    static private GameManager _instance;
    static public GameManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<GameManager>();
            return _instance;
        }
    }
    public float WarningTime = 0.2f;

    public AudioSource EffectsAudioSource;
    public AudioSource MovingSoundSource;
    public AudioSource MusicAudioSource;
    public Character Character;

    public ParticleSystem PistonWarningParticles;
    public AudioClip PistonWarningSound;
    public ParticleSystem PistonActivationParticles;
    public AudioClip PistonActivationSound;
    public AudioClip PistonFinishedSound;

    public AudioClip JumpSound;
    public AudioClip DeathSound;
    public AudioClip WinSound;
    public AudioClip ClickSound;
    public ParticleSystem DeathParticles;
    public ParticleSystem WinParticles;


    public List<Piston> ActivePistons;
    public List<Piston> AllPistons;

    public GameObject CharacterPrefab;

    public bool LevelStarted = false;
    public bool LevelFinished = false;
    public bool Paused = false;

    private System.DateTime _startDateTime;
    private System.DateTime _endDateTime;
    private Piston _standingOn;
    // Use this for initialization
    void Start()
    {
        LoadLevel();
    }

    public void Click()
    {
        EffectsAudioSource.PlayOneShot(ClickSound);
    }

    public void LoadLevel()
    {
        ActivePistons = new List<Piston>();
        AllPistons = new List<Piston>();
        LevelStarted = false;
        LevelFinished = false;
        GameMenu.Instance.Init();

        if (GameState.Instance.DebugLevel)
        {
            LevelLoaded();
            return;
        }

        if (GameState.Instance.CurrentLevel == null)
        {
            GameState.Instance.DebugInit();
        }

        MusicAudioSource.volume = GameState.Instance.GameConfig.MusicVolume;
        MovingSoundSource.volume = GameState.Instance.GameConfig.EffectsVolume;
        EffectsAudioSource.volume = GameState.Instance.GameConfig.EffectsVolume;

        SceneManager.LoadSceneAsync(GameState.Instance.CurrentLevel.SceneName, LoadSceneMode.Additive);
    }


    public void NextLevel()
    {
        SceneManager.UnloadScene(GameState.Instance.CurrentLevel.SceneName);
        GameState.Instance.CurrentLevel = GameState.Instance.NextLevel;
        LoadLevel();
    }

    public void RestartLevel()
    {
        if (Character != null)
            Destroy(Character.gameObject);
        SceneManager.UnloadScene(GameState.Instance.CurrentLevel.SceneName);
        LoadLevel();
    }

    public void GoMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LevelLoaded()
    {
        GameState.Instance.CurrentLevelTime = 0.0;
        GameMenu.Instance.ShowLevel();
        var go = GameObject.Instantiate(CharacterPrefab, new Vector3(0, 2f, 0), Quaternion.identity);
        Character = go.GetComponent<Character>();
    }

    public void StartGame()
    {
        _startDateTime = System.DateTime.Now;
        GameMenu.Instance.StartGame();
        GameManager.Instance.LevelStarted = true;
    }

    public void PistonWarning(Piston piston)
    {
        EffectsAudioSource.PlayOneShot(PistonWarningSound);
        CameraShake.ShakeCamera(0.5f);
        PistonWarningParticles.transform.position = piston.transform.position + Quaternion.Euler(0, 0, piston.transform.rotation.eulerAngles.z) * new Vector3(0f, 0f, 0);
        PistonWarningParticles.transform.rotation = piston.transform.rotation;
        PistonWarningParticles.transform.localScale = new Vector3(piston.OriginalSize.x, 1f, 1f);
        PistonWarningParticles.Emit(100);
    }

    public List<Piston> ChildPistons(Piston piston)
    {
        return piston.ChildPistons;
    }

    public void Finish(Character character)
    {

        EffectsAudioSource.PlayOneShot(WinSound);
        WinParticles.transform.position = character.transform.position;
        WinParticles.Emit(100);
        LevelFinished = true;
        GameState.Instance.CurrentLevelTime = (System.DateTime.Now - _startDateTime).TotalSeconds;

        Destroy(character.gameObject);
        character.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        GameState.Instance.OpenNextLevel();
        if (GameState.Instance.CurrentLevel.Record < 1.0 || GameState.Instance.CurrentLevelTime < GameState.Instance.CurrentLevel.Record)
        {
            GameState.Instance.CurrentLevel.Record = GameState.Instance.CurrentLevelTime;
            GameMenu.Instance.ShowWin(true);
        }
        else
        {
            GameMenu.Instance.ShowWin(false);
        }

    }
    public void Jump(Character character)
    {
        EffectsAudioSource.PlayOneShot(JumpSound);
    }
    public void Die(Character character)
    {
        EffectsAudioSource.PlayOneShot(DeathSound);
        LevelFinished = true;
        GameState.Instance.CurrentLevelTime = (System.DateTime.Now - _startDateTime).TotalSeconds;
        DeathParticles.transform.position = character.transform.position;
        DeathParticles.Emit(50);
        Destroy(character.gameObject);
        GameMenu.Instance.ShowLost();
    }

    public void StandingOn(Piston piston)
    {
        if (_standingOn != piston)
        {
            _standingOn = piston;

            foreach (var p in ChildPistons(_standingOn))
            {
                p.AllowedToActivate = true;
            }
        }

    }

    public void PistonActivated(Piston piston)
    {
        ActivePistons.Add(piston);
        EffectsAudioSource.PlayOneShot(PistonActivationSound);
        CameraShake.ShakeCamera(2f);
        PistonActivationParticles.transform.position = piston.transform.position + Quaternion.Euler(0, 0, piston.transform.rotation.eulerAngles.z) * new Vector3(0f, 0.1f, 0);
        PistonActivationParticles.transform.rotation = piston.transform.rotation;
        PistonActivationParticles.transform.localScale = new Vector3(piston.OriginalSize.x, 1f, 1f);
        PistonActivationParticles.Emit(200);
    }

    public void PistonStopped(Piston piston)
    {
        ActivePistons.Remove(piston);
        EffectsAudioSource.PlayOneShot(PistonFinishedSound);
    }

    void FixedUpdate()
    {
        if (LevelStarted && !LevelFinished)
        {
            GameState.Instance.CurrentLevelTime = (System.DateTime.Now - _startDateTime).TotalSeconds;
        }
        var mass = 0f;
        foreach (var piston in ActivePistons)
        {
            mass += piston.OriginalSize.x * piston.OriginalSize.y;
        }
        mass /= 400;
        if (mass > 0.1f)
        {
            CameraShake.ShakeCamera(mass);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (ActivePistons.Count > 0)
        {
            MovingSoundSource.volume = GameState.Instance.GameConfig.EffectsVolume;
        }
        else
        {
            MovingSoundSource.volume = 0f;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartLevel();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Paused)
            {
                Paused = false;
                Character.Unfreeze();
                GameMenu.Instance.HidePause();
            }
            else
            {
                Paused = true;
                Character.Freeze();
                GameMenu.Instance.ShowPause();
            }
        }

    }
}
