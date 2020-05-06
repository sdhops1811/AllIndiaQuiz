using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource backgrndMusic;
    public AudioSource normalClickSource;

    void Awake()
    {
        if (PlayerPrefs.GetInt(SingletonClass.instance.IS_MUSIC_OFF, 0) == 0)
        {
            backgrndMusic.Play();
        }
    }

    public void MuteMusic()
    {
        backgrndMusic.Pause();
    }

    public void PlayMusicFirstTime()
    {
        backgrndMusic.Play();
    }

    public void PlayMusic()
    {
        backgrndMusic.UnPause();
    }

    public void PlayNormalClick()
    {
        if (PlayerPrefs.GetInt(SingletonClass.instance.IS_SOUND_OFF) == 0)
        {
            normalClickSource.Play();
        }
    }
}
