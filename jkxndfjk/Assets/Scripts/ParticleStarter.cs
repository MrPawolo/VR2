using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleStarter : MonoBehaviour
{
    ParticleSystem particles;
    void Awake()
    {
        particles = GetComponent<ParticleSystem>();
        
    }

    private void OnEnable()
    {
        StaticGameController.Instance.onLevelStart -= Play;
        StaticGameController.Instance.onLevelStart += Play;
        StaticGameController.Instance.onLevelFinished -= Stop;
        StaticGameController.Instance.onLevelFinished += Stop;
    }
    private void OnDisable()
    {
        StaticGameController.Instance.onLevelStart -= Play;
        StaticGameController.Instance.onLevelFinished -= Stop;
    }

    void Play()
    {
        if (particles)
        {
            particles.Play();
        }
    }
    void Stop()
    {
        if (particles)
        {
            particles.Stop();
        }
    }

}
