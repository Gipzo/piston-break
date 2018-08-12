using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelSettings
{
    public string SceneName;

    public bool OpenedByDefault = false;
    public bool Opened
    {
        get
        {
            return PlayerPrefs.GetInt(SceneName + "_Opened", 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt(SceneName + "_Opened", value == true ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
    public double Record
    {
        get
        {
            return System.Convert.ToDouble(PlayerPrefs.GetFloat(SceneName + "_Record", 0));
        }
        set
        {
            PlayerPrefs.SetFloat(SceneName + "_Record", (float)value);
            PlayerPrefs.Save();
        }
    }

    public static string SecondsToTime(double seconds)
    {
        System.TimeSpan t = System.TimeSpan.FromSeconds(seconds);

        return string.Format("{0:D2}:{1:D2}.{2:D3}",
                        t.Minutes,
                        t.Seconds,
                        t.Milliseconds);
    }
}

[CreateAssetMenu()]
public class GameConfiguration : ScriptableObject
{
    public LevelSettings[] Levels;
    public float MusicVolume
    {
        get
        {
            return PlayerPrefs.GetFloat("Music_Volume", 1f);
        }
        set
        {
            PlayerPrefs.SetFloat("Music_Volume", value);
            PlayerPrefs.Save();
        }
    }
    public float EffectsVolume
    {
        get
        {
            return PlayerPrefs.GetFloat("Effects_Volume", 1f);
        }
        set
        {
            PlayerPrefs.SetFloat("Effects_Volume", value);
            PlayerPrefs.Save();
        }
    }
}
