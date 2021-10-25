using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Songs : MonoBehaviour
{

  AudioSource audioSource;
  public AudioClip[] songs;
  int currentIndex = 0;

  bool isplaying = false;
  // Start is called before the first frame update
  void Start()
  {
    audioSource = GetComponent<AudioSource>();
  }

  // Update is called once per frame
  void Update()
  {


  }

  public void Play()
  {
    Debug.Log("hit");
    isplaying = !isplaying;
    if (isplaying)
    {
      audioSource.Stop();
      audioSource.clip = songs[currentIndex];
      audioSource.Play();
    }
    else { audioSource.Stop(); }
  }

  public void SkipForward()
  {
    if (currentIndex < songs.Length - 1)
    {
      audioSource.Stop();
      currentIndex++;
      audioSource.clip = songs[currentIndex];
      audioSource.Play();
    }
  }

  public void SkipBackward()
  {
    if (currentIndex > 0)
    {
      audioSource.Stop();
      currentIndex--;
      audioSource.clip = songs[currentIndex];
      audioSource.Play();
    }
  }
}
