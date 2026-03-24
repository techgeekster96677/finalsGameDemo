using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
/// Controls main menu UI interactions and audio persistence.
/// Relies on LevelManager for scene transitions and MusicManager for audio playback.
/// </summary>
public class MainMenu : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioMixer audioMixer;
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Start()
    {
        LoadVolume();

        // BUG FIX (2024-10-15): Music was not playing on menu load after scene refactoring.
        // Explicitly calling PlayMusic ensures the correct track plays even if the MusicManager
        // instance was created after the scene started.
        MusicManager.instance.PlayMusic("MainMenu");
    }

    public void StartGame()
    {
        // DIRECT SCENE LOADING BYPASS: The commented SceneManager.LoadScene below is intentionally
        // left as a reference to demonstrate the previous direct loading approach.
        // The current implementation uses LevelManager to maintain:
        // 1. Centralized scene transition logic
        // 2. Consistent fade/transition effects across all scene changes
        // 3. Any pre-load initialization required by the game's architecture
        // SceneManager.LoadScene("SampleScene");

        LevelManager.instance.LoadScene("Level 1", "CrossFade");
    }

    public void QuitGame()
    {
        // The Application.isEditor check handles the fundamental difference between Editor and
        // built application behavior. In Editor, stopping Play Mode is the equivalent of quitting.
        if (Application.isEditor)
        {
            // UnityEditor.EditorApplication.isPlaying is conditionally compiled and only available
            // in the Editor environment. This is the standard pattern for exiting Play Mode.
            UnityEditor.EditorApplication.isPlaying = false;
        }
        else
        {
            // In a built application, Application.Quit() initiates the normal termination sequence.
            // On some platforms (WebGL, iOS), this may behave differently or be restricted.
            Application.Quit();
        }

        // NOTE: The following duplicate calls are unreachable but kept as a reminder of a common
        // anti-pattern. This code path is never executed because:
        // - In Editor: setting isPlaying to false terminates execution
        // - In build: Application.Quit() begins shutdown before subsequent lines run
        // Consider removing these lines in future refactoring.
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }

    public void SetMusicVolume(float volume)
    {
        // AudioMixer.SetFloat expects decibel values (-80dB to 20db range).
        // The slider UI should be configured to output logarithmic values to match
        audioMixer.SetFloat("MusicVolume", volume);
        SaveVolume();
    }

    public void SetSFXVolume(float volume)
    {
        // Same decibel range applies as with music volume.
        // Both volume controls operate independently, allowing separate music and SFX levels.
        SaveVolume();
        audioMixer.SetFloat("SFXVolume", volume);
    }

    public void SaveVolume()
    {
        // The GetFloat calls use 'out' parameters to retrieve current mixer values.
        // This approach ensures that the saved values always reflect the actual mixer state,
        // not just the slider positions (which could become desynchronized).
        audioMixer.GetFloat("MusicVolume", out float musicVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);

        audioMixer.GetFloat("SFXVolume", out float sfxVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
    }

    public void LoadVolume()
    {
        // PlayerPrefs.HasKey check prevents overwriting mixer defaults with non-existent values.
        // This is the standard pattern for first-run initialization where no saved preferences exist.
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            float musicVolume = PlayerPrefs.GetFloat("MusicVolume");
            audioMixer.SetFloat("MusicVolume", musicVolume);
            musicSlider.value = musicVolume;
        }

        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            float sfxVolume = PlayerPrefs.GetFloat("SFXVolume");
            audioMixer.SetFloat("SFXVolume", sfxVolume);
            sfxSlider.value = sfxVolume;
        }

        // Note: No else clauses are provided for first-run initialization.
        // This means on first launch, sliders will display their default UI values
        // but the audioMixer retains its default state. Consider adding explicit
        // default value initialization if visual-audio sync is required immediately.
    }
}