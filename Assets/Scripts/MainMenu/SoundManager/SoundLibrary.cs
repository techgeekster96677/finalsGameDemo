using UnityEngine;

/// <summary>
/// Serializable data structure for grouping sound effects.
/// Allows multiple audio clips under a single group ID for optional randomized playback.
/// </summary>
[System.Serializable]
public struct SoundEffect
{
    public string groupID;
    public AudioClip[] clips;
}

/// <summary>
/// Repository for sound effects that provides clip selection by group ID.
/// Decouples SoundManager from direct AudioClip references.
/// Supports multiple clips per group for sound variation if added in the future.
/// </summary>
public class SoundLibrary : MonoBehaviour
{
    public SoundEffect[] soundEffects;

    /// <summary>
    /// Retrieves an AudioClip from the specified sound group.
    /// If multiple clips exist in the group, one is selected at random.
    /// 
    /// TROUBLESHOOTING: If no sound is heard:
    /// - Verify the soundName matches a groupID exactly (case-sensitive)
    /// - Ensure the soundEffect entry exists in the Inspector array
    /// - Confirm the AudioClip field is assigned in the Inspector
    /// </summary>
    public AudioClip GetClipFromName(string soundName)
    {
        foreach (var soundEffect in soundEffects)
        {
            if (soundEffect.groupID == soundName)
            {
                return soundEffect.clips[Random.Range(0, soundEffect.clips.Length)];
            }
        }
        return null;
    }

}