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
        Application.Quit();
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