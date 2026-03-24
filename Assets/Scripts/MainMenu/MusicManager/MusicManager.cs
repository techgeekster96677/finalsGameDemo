using UnityEngine;
using System.Collections;

/// <summary>
/// Manages background music playback with crossfade transitions.
/// Implements singleton pattern to persist music across scene loads.
/// </summary>
public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    // Singleton pattern to ensure only one instance of MusicManager exists
    [SerializeField]
    private MusicLibrary musicLibrary;
    [SerializeField]
    private AudioSource musicSource;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Check if an instance of MusicManager already exists
        if (instance != null)
            Destroy(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Public method to play music by track name with optional fade duration
    public void PlayMusic(string trackName, float fadeDuration = 0.5f)
    {
        StartCoroutine(AnimateMusicCrossFade(musicLibrary.GetClipFromName(trackName), fadeDuration));
    }


    // Coroutine to handle cross-fading between music tracks
    IEnumerator AnimateMusicCrossFade(AudioClip nextTrack, float fadeDuration = 0.0f)
    {
        // If fade duration is zero, switch tracks immediately without fading
        float percent = 0;

        // Fade out current track
        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / fadeDuration;
            musicSource.volume = Mathf.Lerp(1f, 0, percent);
            yield return null;
        }

        // Switch to the next track and start playing
        musicSource.clip = nextTrack;
        musicSource.Play();

        // Fade in the new track
        percent = 0;

        // Fade in new track
        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / fadeDuration;
            musicSource.volume = Mathf.Lerp(0, 1f, percent);
            yield return null;
        }
    }
}