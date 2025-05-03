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

        // if (musicSource.clip == null || !musicSource.isPlaying)
        // {
        //     PlayMusic("Theme");
        // }
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
            musicSource.loop = (name != "GameOver");
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();   
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

    // ====== FADE SYSTEM BELOW ======

    public void FadeOutMusicAndPlay(string nextMusic, float fadeDuration = 2f)
    {
        StartCoroutine(FadeOutCoroutine(nextMusic, fadeDuration));
    }

    private IEnumerator FadeOutCoroutine(string nextMusic, float fadeDuration)
    {
        float startVolume = musicSource.volume;

        // Fade out current music
        while (musicSource.volume > 0)
        {
            musicSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        musicSource.Stop();
        musicSource.volume = startVolume; // Reset volume before fade-in

        // Find and set next music clip
        Sound s = Array.Find(musicSounds, x => x.name == nextMusic);

        if (s == null)
        {
            Debug.LogWarning("Music Sound Not Found: " + nextMusic);
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.loop = (nextMusic != "GameOver");
            StartCoroutine(FadeInCoroutine(fadeDuration)); // Fade in new music
        }
    }

    private IEnumerator FadeInCoroutine(float fadeDuration = 2f)
    {
        musicSource.volume = 0f;
        musicSource.Play();

        float targetVolume = 1f;
        while (musicSource.volume < targetVolume)
        {
            musicSource.volume += Time.deltaTime / fadeDuration;
            yield return null;
        }

        musicSource.volume = targetVolume; // Clamp to final volume
    }
}
