using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    GameObject BackgroundMusic;
    AudioSource BGM;

    void Awake()
    {
        BackgroundMusic = GameObject.Find("BackgroundMusic");
        BGM = BackgroundMusic.GetComponent<AudioSource>();
        if (BGM.isPlaying)
        {
            return;
        }
        else
        {
            BGM.Play();
            DontDestroyOnLoad(BackgroundMusic);
        }
    }
}