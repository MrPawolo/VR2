using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyStick : MonoBehaviour
{
    public bool debug = false;
    public bool flipX = false;
    public bool flipY = false;
    public bool swapXY = false;
    public float maxTravel = 0.1f;
    Vector3 startPos;
    public GameObject playerBall;
    IJoyVal joyVal;
    public Vector2 joy;
    void Start()
    {
        startPos = transform.localPosition;
        if (playerBall)
        {
            joyVal = playerBall.GetComponent<IJoyVal>();
        }
    }

    void Update()
    {
        if (debug)
        {
            float hor = Input.GetAxis("Horizontal");
            float ver = Input.GetAxis("Vertical");

            Vector2 outVec = new Vector2(flipX ? hor : -hor,
                flipY ? ver : -ver);
            joy = outVec;
            if (swapXY)
            {
                outVec = new Vector2(outVec.y, outVec.x);
            }


            joyVal.JoyVal(outVec / maxTravel);
            return;
        }
        if (joyVal != null)
        {
            Vector3 actPos = transform.localPosition;
            Vector3 posDiff = startPos - actPos;

            Vector2 outVec = new Vector2(flipX?  -posDiff.x : posDiff.x,
                flipY? -posDiff.z : posDiff.z);
            joy = outVec;
            if (swapXY)
            {
                outVec = new Vector2(outVec.y, outVec.x);
            }


            joyVal.JoyVal(outVec / maxTravel);
        }

    }
}
