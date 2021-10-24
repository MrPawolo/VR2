using UnityEngine;

public class SpawnAudtioSource : MonoBehaviour
{
    float vol;
    public void SetVolume(float volume)
    {
        vol = volume;
    }
    public void PlayClipAtPoint(AudioClip clip)
    {
        AudioSource.PlayClipAtPoint(clip, transform.position, vol);
    }
}
