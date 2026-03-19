using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour
{
    public AudioMixer audioMixer;

    public Slider musicSlider;
    public Slider sfxSlider;

    private void Start()
    {
        LoadVolume();
        MusicManager.instance.PlayMusic("MainMenu");
    }
    public void StartGame()
    {
        // This bypasses your LevelManager - remove or comment this out
        // SceneManager.LoadScene("SampleScene");

        // Use the LevelManager instead:
        LevelManager.instance.LoadScene("Level 1", "CrossFade");
    }

    public void QuitGame()
    {
        // Check if we are in the Unity Editor
        if (Application.isEditor)
        {
            // Stop play mode in Editor
            Debug.Log("Stopping game in Editor...");
            UnityEditor.EditorApplication.isPlaying = false;
        }
        else
        {
            // Quit the actual game application
            Debug.Log("Quitting game application...");
            Application.Quit();
        }
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", volume);
    }

    public void SaveVolume()
    {
        audioMixer.GetFloat("MusicVolume", out float musicVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);

        audioMixer.GetFloat("SFXVolume", out float sfxVolume);  
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
    }

    public void LoadVolume()
    {
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
    }
}