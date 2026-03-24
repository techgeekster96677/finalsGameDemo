using UnityEngine;

/// <summary>
/// Centralized audio manager for playing 2D and 3D sound effects.
/// Implements singleton pattern to provide global access across scenes.
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField]
    private SoundLibrary soundLibrary;
    [SerializeField]
    private AudioSource sfx2DSource;

    /// <summary>
    /// Singleton initialization. First instance persists; duplicates self-destruct.
    /// </summary>
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

    /// <summary>
    /// Plays a 3D sound at the specified world position using an AudioClip reference.
    /// 
    /// TROUBLESHOOTING: If no sound is heard:
    /// - Verify clip is assigned and not null
    /// - Check that the listener (usually camera) is within hearing range
    /// - AudioSource.PlayClipAtPoint creates a temporary AudioSource that auto-destructs after playback
    /// Reference: https://docs.unity3d.com/ScriptReference/AudioSource.PlayClipAtPoint.html
    /// </summary>
    public void PlaySound3D(AudioClip clip, Vector3 pos)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, pos);
        }
    }

    /// <summary>
    /// Plays a 3D sound by name, retrieving the corresponding AudioClip from SoundLibrary.
    /// Overload allows callers to use string identifiers instead of direct AudioClip references.
    /// 
    /// NOTE: If soundName is not found, soundLibrary.GetClipFromName returns null,
    /// and PlaySound3D's null check prevents playback. Check console for warnings.
    /// </summary>
    public void PlaySound3D(string soundName, Vector3 pos)
    {
        PlaySound3D(soundLibrary.GetClipFromName(soundName), pos);
    }

    /// <summary>
    /// Plays a 2D sound (non-positional) using the dedicated 2D AudioSource.
    /// PlayOneShot allows overlapping sounds without interrupting currently playing audio.
    /// 
    /// TROUBLESHOOTING: If no sound is heard:
    /// - Verify sfx2DSource is assigned in the Inspector
    /// - Verify soundName exists in SoundLibrary
    /// - Check that sfx2DSource volume is not 0 or muted by AudioMixer
    /// </summary>
    public void PlaySound2D(string soundName)
    {
        sfx2DSource.PlayOneShot(soundLibrary.GetClipFromName(soundName));
    }
}