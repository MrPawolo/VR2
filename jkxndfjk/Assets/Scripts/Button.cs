using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ConfigurableJoint))]
public class Button : MonoBehaviour
{
    public float percent;
    public Vector3 startPos;
    ConfigurableJoint configurableJoint;
    [Range(0,1)] public float pushForcePercent = 0.6f;
    [Range(0,1)] public float travelPercentActivate = 0.7f;
    [Range(0,1)] public float travelPercentDeactivate = 0.5f;
    public float travel = 0.02f;
    public float positionSpring = 1000f;
    public float positionDamper = 20;
    public float maxForce = 40f;
    public UnityEvent onButtonDown;
    public UnityEvent onButtonUp;
    bool lastState = false;

    [Header("Audio")]
    public float volume = 1;
    public AudioClip pressClip;
    public AudioClip relClip;

    

    void Start()
    {
        Rigidbody myRb = GetComponent<Rigidbody>();
        myRb.useGravity = false;
        myRb.mass = 0.2f;

        startPos = transform.localPosition;
        configurableJoint = GetComponent<ConfigurableJoint>();
        configurableJoint.anchor = new Vector3(0, -transform.localPosition.y, 0);
        SoftJointLimit softJointLimit = new SoftJointLimit();
        softJointLimit.limit = travel;
        configurableJoint.linearLimit = softJointLimit;
        configurableJoint.targetPosition = new Vector3(0, -1, 0);
        JointDrive jointDrive = new JointDrive {
            positionSpring = positionSpring,
            positionDamper = positionDamper,
            maximumForce = maxForce
        };
        configurableJoint.yDrive = jointDrive;
        configurableJoint.xMotion = ConfigurableJointMotion.Locked;
        configurableJoint.yMotion = ConfigurableJointMotion.Limited;
        configurableJoint.zMotion = ConfigurableJointMotion.Locked;
        configurableJoint.angularXMotion = ConfigurableJointMotion.Locked;
        configurableJoint.angularYMotion = ConfigurableJointMotion.Locked;
        configurableJoint.angularZMotion = ConfigurableJointMotion.Locked;
        transform.localPosition += Vector3.up * travel;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float travelPercent = 1 - ((transform.localPosition.y - startPos.y) / travel);
        percent = travelPercent;
        if (travelPercent > travelPercentActivate && lastState == false)
        {
            if (lastState == false)
            {
                PlayClipRandomPitch(volume, pressClip);
                onButtonDown?.Invoke();
                lastState = true;
            }
        }
        else if(travelPercent < travelPercentDeactivate && lastState == true)
        {
            PlayClipRandomPitch(volume, relClip);
            onButtonUp?.Invoke();
            lastState = false;
        }
        if(travelPercent > pushForcePercent)
        {
            JointDrive jointDrive = new JointDrive
            {
                positionSpring =  positionSpring,
                positionDamper = positionDamper,
                maximumForce = pushForcePercent * maxForce
            };
            configurableJoint.yDrive = jointDrive;
        }
        else
        {
            JointDrive jointDrive = new JointDrive
            {
                positionSpring = positionSpring,
                positionDamper = positionDamper,
                maximumForce = maxForce
            };
            configurableJoint.yDrive = jointDrive;
        }
    }
   
    void PlayClipRandomPitch(float vol, AudioClip clip)
    {
        if (!clip) return;
        AudioSource audioSource = new GameObject().AddComponent<AudioSource>();
        audioSource.transform.position = transform.position;
        audioSource.volume = vol;
        audioSource.clip = clip;
        audioSource.pitch = Random.Range(0.98f, 1.02f);
        audioSource.spatialBlend = 1;
        audioSource.Play();
        Destroy(audioSource.gameObject, clip.length);
    }
}
