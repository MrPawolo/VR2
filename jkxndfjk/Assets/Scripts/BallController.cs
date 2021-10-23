using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BallController : MonoBehaviour, IJoyVal
{
    [Range(-1,1)] public float xVal;
    [Range(-1,1)] public float yVal;
    public float torque = 10f;
    public float maxVel = 10f;
    public AnimationCurve magToVel;
    public float forceStopThreshold = 0.05f;
    public float startMoveThreshold = 0.1f;
    public Transform refTrans;

    Rigidbody myRb;
    private void Start()
    {
        myRb = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        float magSpeed = new Vector2(xVal, yVal).magnitude;
         
        if(Mathf.Abs(magSpeed) > startMoveThreshold)
        {
            myRb.constraints = RigidbodyConstraints.None;
            Vector3 velNormalized = new Vector3(xVal, 0, yVal).normalized;

            Vector3 dir = new Vector3(0, refTrans.rotation.eulerAngles.y, 0);

            Vector3 rot = Vector3.Cross(refTrans.up, Quaternion.Euler(dir) * velNormalized ) * torque;
            myRb.angularVelocity = rot;
            myRb.maxAngularVelocity = maxVel * magToVel.Evaluate(magSpeed);
        }
        else if(Mathf.Abs(magSpeed) < forceStopThreshold)
        {
            myRb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    public void JoyVal(Vector2 vector)
    {
        xVal = vector.x;
        yVal = vector.y;
    }
}
