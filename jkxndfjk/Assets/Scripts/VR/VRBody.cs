using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRBody : MonoBehaviour
{
    public Transform head;
    public Transform io;
    public float spineOffset = 0.2f;
    public VRController rotateController;
    public VRController moveController;
    public float walkVel;
    public float startWalkThreshold = 0.1f;
    [Range(0,1)]public float walkJoyThreshold = 0.3f;
    public CapsuleCollider bodyCollider;
    public SphereCollider headCol;

    Rigidbody myRb;
    Vector3 virtualPos;
    private void Start()
    {
        myRb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        Movement();
        CenterIO();
        CenterColliders();
        HandleRotation();
    }

    void Movement()
    {
        Vector2 joyStiockVal = moveController.JoyStickVal;
        float controllerSqrMag = joyStiockVal.magnitude;
        if (controllerSqrMag < walkJoyThreshold) return;

        Vector3 vel = Move(joyStiockVal).normalized * controllerSqrMag * walkVel;

        if (vel.sqrMagnitude < startWalkThreshold * startWalkThreshold) return;
        myRb.velocity = vel;
    }
    private Vector3 Move(Vector2 joystickVal)
    {
        Vector3 velocity = new Vector3(joystickVal.x, 0, joystickVal.y);
        Vector3 direction;

        direction = head.eulerAngles;

        Vector3 walkDirection = new Vector3(0, direction.y, 0);
        velocity = Quaternion.Euler(walkDirection) * velocity;

        return new Vector3(velocity.x, 0, velocity.z);
    }
    void CenterIO() //Camera and controller's parent
    {
        io.transform.position = transform.position + virtualPos;
    }
    void CenterColliders()
    {
        CenterHeadCollider();
        CenterBody();
    }
    void CenterHeadCollider()
    {
        headCol.center = transform.InverseTransformPoint(head.position);
    }
    void CenterBody()
    {
        Vector3 headPos = transform.InverseTransformPoint(head.position);

        float headHeight = Mathf.Clamp(headPos.y, 0.5f, 2f);
        bodyCollider.height = headHeight;

        Vector3 newCenter = Vector3.zero;

        newCenter.y = transform.position.y;
        newCenter.y = bodyCollider.height / 2;
        newCenter.y += 0.08f;

        Vector2 pos = SpinePos();
        newCenter.x = pos.x;
        newCenter.z = pos.y;
        bodyCollider.center = newCenter;
    }
    private Vector2 SpinePos()
    {
        Vector3 right = Vector3.ProjectOnPlane(head.right, Vector3.up).normalized;
        Vector3 back = Vector3.Cross(right, Vector3.up);
        Vector3 spinePos = transform.InverseTransformDirection(back) * -0.2f;
        spinePos = transform.InverseTransformPoint(head.position) + spinePos;

        return new Vector2(spinePos.x, spinePos.z);
    }
    void HandleRotation()
    {
        TurnProcess();
        if (turning)
        {
            SmoothSnapTurn();
        }
    }

    public enum TurnType { Snap, Smooth };
    [SerializeField] TurnType turnType = TurnType.Smooth;

    [SerializeField] float turnThreshodldVal = 0.7f;
    [SerializeField] int turnAngle = 45;
    [SerializeField] float turnSpeed = 10;

    private bool turned = false;
    private bool turning = false;
    private float desiredTurn = 0f;
    private float actTurn = 0f;


    private void TurnProcess()
    {
        float joyStickXVal = rotateController.JoyStickVal.x;
        if (Mathf.Abs(joyStickXVal) > turnThreshodldVal && !turned)
        {
            if (joyStickXVal > 0)
            {
                if (turnType == TurnType.Snap)
                {
                    SnapTurn(turnAngle);
                }
                else
                {
                    if (!turning)
                    {
                        turning = true;
                        desiredTurn = turnAngle;
                    }
                }
            }
            else
            {
                if (turnType == TurnType.Snap)
                {
                    SnapTurn(-turnAngle);
                }
                else
                {
                    if (!turning)
                    {
                        turning = true;
                        desiredTurn = -turnAngle;
                    }
                }
            }
            turned = true;
        }
        else if (Mathf.Abs(joyStickXVal) < turnThreshodldVal)
        {
            turned = false;
        }
    }
    private void SnapTurn(int _turnAmount)
    {
        transform.RotateAround(head.transform.position, Vector3.up, _turnAmount);
    }
    public void RotateRig(float _amount)
    {
        Vector3 prevPos = io.localPosition;
        io.transform.RotateAround(head.transform.position, Vector3.up, _amount);
        virtualPos = virtualPos + (io.localPosition - prevPos);
        actTurn += _amount;
    }
    private void SmoothSnapTurn()
    {
        float _turnAmount = (desiredTurn / Mathf.Abs(desiredTurn)) * turnSpeed * Time.fixedDeltaTime;
        RotateRig(_turnAmount);

        if (Mathf.Abs(actTurn) >= Mathf.Abs(desiredTurn))
        {
            turning = false;
            actTurn = 0;
            desiredTurn = 0;
        }
    }
}
