using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class VelToSound : MonoBehaviour
{
    AudioSource audioSource;
    Rigidbody rb;
    public AnimationCurve pitchToVel;
    public AnimationCurve volToVel;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        audioSource.velocityUpdateMode = AudioVelocityUpdateMode.Fixed;
    }

    private void FixedUpdate()
    {
        Vector3 vel = rb.velocity;
        float velXY = new Vector2(vel.x, vel.z).magnitude;

        audioSource.pitch = pitchToVel.Evaluate(velXY);
        audioSource.volume = volToVel.Evaluate(velXY);
    }
}
