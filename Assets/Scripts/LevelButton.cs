using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{

    public Text Label;
    public LevelSettings Settings;


    // Use this for initialization
    void Start()
    {

    }

    public void Init(LevelSettings settings, int index)
    {
        Settings = settings;
        GetComponent<Button>().interactable = Settings.Opened;

        var record = "\n";
        if (Settings.Opened && Settings.Record > 0)
        {
            record = LevelSettings.SecondsToTime(Settings.Record);
        }

        Label.text = string.Format("LEVEL {0}\n\n{1}", index + 1, record);

    }

    public void SelectLevel()
    {
        MenuManager.Instance.SelectLevel(Settings);

    }

    // Update is called once per frame
    void Update()
    {

    }
}
