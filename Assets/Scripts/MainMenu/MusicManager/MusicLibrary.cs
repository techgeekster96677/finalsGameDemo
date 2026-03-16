using UnityEngine;

[System.Serializable]
public struct MusicTrack
{
    public string trackName;
    public AudioClip audioClip;
}
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
