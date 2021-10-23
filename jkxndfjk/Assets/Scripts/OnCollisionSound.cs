using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCollisionSound : MonoBehaviour
{
    public AudioClip audioClip;
    public float volume = 1;

    private void OnCollisionEnter(Collision collision)
    {
        float vol = collision.relativeVelocity.magnitude;

        
        AudioSource audioSource = new AudioSource();
        audioSource.volume = vol * volume / 5;
        audioSource.pitch = Random.Range(0.98f, 1.02f);
        audioSource.transform.position = transform.position;
        audioSource.clip = audioClip;
        audioSource.Play();
        Destroy(audioSource.gameObject, audioClip.length);
    }
}
