using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    private void Start()
    {
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
}