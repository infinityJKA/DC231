using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Debug.Log("AudioManager Start() - Scene: " + SceneManager.GetActiveScene().name);

        if (musicSource.clip == null || !musicSource.isPlaying)
        {
            PlayMusic("Theme");
        }
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);

        if (s == null)
        {
            Debug.LogWarning("Music Sound Not Found: " + name);
        }
        else
        {
            if (musicSource.clip == s.clip && musicSource.isPlaying)
            {
                Debug.Log("Music '" + name + "' already playing.");
                return;
            }

            Debug.Log("Playing music: " + name);
            musicSource.clip = s.clip;
            musicSource.loop = (s.name == "Theme");
            musicSource.Play();
        }
    }

    public void PlaySFX(string name)
    {
        if (sfxSource.volume <= 0f || sfxSource.mute) return;

        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.LogWarning("SFX Sound Not Found: " + name);
        }
        else
        {
            sfxSource.PlayOneShot(s.clip);
        }
    }

    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;
    }

    public void ToggleSFX()
    {
        sfxSource.mute = !sfxSource.mute;
    }

    public void MusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("Force Music Check (M pressed)");
            if (musicSource != null)
            {
                Debug.Log("Music Source is " + (musicSource.isPlaying ? "playing" : "not playing") +
                          " | Clip: " + (musicSource.clip != null ? musicSource.clip.name : "none") +
                          " | Volume: " + musicSource.volume +
                          " | Muted: " + musicSource.mute);
            }
            else
            {
                Debug.LogWarning("Music Source is NULL");
            }
        }
    }
}
