using UnityEngine;

/// <summary>
/// Serializable data structure for mapping track names to AudioClip assets.
/// Configured in the Unity Inspector to allow designers to assign music tracks.
/// </summary>
[System.Serializable]
public struct MusicTrack
{
    public string trackName;
    public AudioClip audioClip;
}

/// <summary>
/// Repository for music tracks that provides lookup by name.
/// Decouples MusicManager from direct AudioClip references, allowing
/// tracks to be managed and swapped in the Inspector without code changes.
/// </summary>
public class MusicLibrary : MonoBehaviour
{
    public MusicTrack[] musicTracks;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public AudioClip GetClipFromName(string trackName)
    {
        foreach (var track in musicTracks)
        {
            if (track.trackName == trackName)
            {
                return track.audioClip;
            }
        }
        Debug.LogWarning($"Track with name {trackName} not found in MusicLibrary.");
        return null;
    }
}