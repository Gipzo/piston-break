using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    static private MenuManager _instance;
    static public MenuManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<MenuManager>();
            return _instance;
        }
    }

    public GameConfiguration GameConfiguration;

    public GameObject MenuPanel;
    public GameObject LevelPanel;
    public GameObject SettingsPanel;
    public GameObject HelpPanel;

    public GameObject LevelContainer;

    public GameObject LevelButtonPrefab;

    public Slider MusicSlider;
    public Slider EffectsSlider;

    public AudioSource MusicAudioSource;
    public AudioSource EffectsAudioSource;

    public AudioClip ClickSound;

    void Awake()
    {

    }



    void Start()
    {

        for (int i = 0; i < GameConfiguration.Levels.Length; i++)
        {
            var level = GameConfiguration.Levels[i];
            if (level.OpenedByDefault)
                level.Opened = true;
            var go = GameObject.Instantiate(LevelButtonPrefab);
            var lb = go.GetComponent<LevelButton>();
            lb.Init(level, i);
            lb.transform.SetParent(LevelContainer.transform, false);

        }
        MusicSlider.value = GameConfiguration.MusicVolume;
        EffectsSlider.value = GameConfiguration.EffectsVolume;

        LevelPanel.SetActive(false);
        SettingsPanel.SetActive(false);
        HelpPanel.SetActive(false);
        MenuPanel.SetActive(true);

    }

    public void OnSliderChange()
    {
        GameConfiguration.MusicVolume = MusicSlider.value;
        GameConfiguration.EffectsVolume = EffectsSlider.value;
        MusicAudioSource.volume = 0.7f * GameConfiguration.MusicVolume;
        EffectsAudioSource.volume = 0.7f * GameConfiguration.EffectsVolume;
    }


    public void Play()
    {
        EffectsAudioSource.PlayOneShot(ClickSound);
        MenuPanel.SetActive(false);
        LevelPanel.SetActive(true);
    }

    public void Settings()
    {
        EffectsAudioSource.PlayOneShot(ClickSound);
        MenuPanel.SetActive(false);
        SettingsPanel.SetActive(true);
    }

    public void Help()
    {
        EffectsAudioSource.PlayOneShot(ClickSound);
        MenuPanel.SetActive(false);
        HelpPanel.SetActive(true);
    }

    public void Return()
    {
        EffectsAudioSource.PlayOneShot(ClickSound);
        LevelPanel.SetActive(false);
        SettingsPanel.SetActive(false);
        HelpPanel.SetActive(false);
        MenuPanel.SetActive(true);
    }

    public void Exit()
    {
        Application.Quit();
    }


    public void SelectLevel(LevelSettings level)
    {
        EffectsAudioSource.PlayOneShot(ClickSound);
        var state = GameState.Instance;
        state.CurrentLevel = level;
        state.GameConfig = GameConfiguration;
        SceneManager.LoadScene("MainLevel");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
