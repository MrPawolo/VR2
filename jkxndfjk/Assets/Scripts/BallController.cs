using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class BallController : MonoBehaviour, IJoyVal
{
    [Header("OnDeath")]
    public GameObject deadParticles;
    public AudioClip deadSound;
    [Space(30)]
    public LayerMask layerMask;
    [Range(-1,1)] public float xVal;
    [Range(-1,1)] public float yVal;
    public float torque = 10f;
    public float maxVel = 10f;
    public AnimationCurve magToVel;
    public float forceStopThreshold = 0.05f;
    public float startMoveThreshold = 0.1f;
    public Transform refTrans;
    bool canMove = true;
    public bool CanMove {  get { return canMove; } set { canMove = value; } }

    [Header("Scale")]
    public float scaleTime = 0.5f;
    public float smallSize = 0.09f;
    public float smallMass = 1;
    public float smallVel = 10;
    public float smallTorque = 10;
    public float bigSize = 0.29f;
    public float bigMass = 5;
    public float bigVel = 4f;
    public float bigTorque = 30f;
     float scaleProggres = 0;
    public bool scaleUp = true;

    public UnityEvent onScaleUp;
    public UnityEvent onScaleDown;

    IEnumerator scaleEnumerator;
    Rigidbody myRb;
    SphereCollider sphereCollider;
    private void Awake()
    {
        myRb = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        Move();
    }
    public void SpawnAfterDeath()
    {
        ParticleSystem gameObject = Instantiate(deadParticles, transform.position, transform.rotation).GetComponent<ParticleSystem>();
        gameObject.transform.SetParent(null);
        Destroy(gameObject.gameObject, gameObject.main.duration);
        if (deadSound)
        {
            AudioSource.PlayClipAtPoint(deadSound, transform.position);
        }
    }
    private void Move()
    {
        if (!canMove) return;
        float magSpeed = new Vector2(xVal, yVal).magnitude;

        if (Mathf.Abs(magSpeed) > startMoveThreshold)
        {
            myRb.constraints = RigidbodyConstraints.None;
            Vector3 velNormalized = new Vector3(xVal, 0, yVal).normalized;
            Vector3 dir = new Vector3(0, refTrans.rotation.eulerAngles.y, 0);

            Vector3 rot = Vector3.Cross(refTrans.up, Quaternion.Euler(dir) * velNormalized) * torque;
            myRb.angularVelocity = rot;
            myRb.maxAngularVelocity = maxVel * magToVel.Evaluate(magSpeed);
        }
        else if (Mathf.Abs(magSpeed) < forceStopThreshold)
        {
            myRb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    public void JoyVal(Vector2 vector)
    {
        xVal = vector.x;
        yVal = vector.y;
    }

    [ContextMenu("Scale")]
    public void ScalePlayer()
    {
        if (!canMove) return;
        scaleUp = !scaleUp;
        if (scaleEnumerator != null)
        {
            StopCoroutine(scaleEnumerator);
        }
        scaleEnumerator = ProcessScale(scaleUp);
        StartCoroutine(scaleEnumerator);
    }
    public void StopScaling()
    {
        if (scaleEnumerator == null) return;
        StopCoroutine(scaleEnumerator);
        scaleEnumerator = null;
    }

    public void ResetPlayer()
    {
        StopScaling();
        ForceStopPlayer();
        myRb.mass = Mathf.Lerp(smallMass, bigMass, 0);
        transform.localScale = Mathf.Lerp(smallSize, bigSize, 0) * Vector3.one;
        torque = Mathf.Lerp(smallTorque, bigTorque, 0);
        maxVel = Mathf.Lerp(smallVel, bigVel, 0);
    }

    void ForceStopPlayer()
    {
        canMove = false;
        xVal = 0;
        yVal = 0;
        myRb.velocity = Vector3.zero;
        myRb.angularVelocity = Vector3.zero;
    }
    public void MoveTo(Vector3 wsPos)
    {
        ForceStopPlayer();
        transform.position = wsPos;
        ForceStopPlayer();
    }

    IEnumerator ProcessScale(bool scaleUp)
    {
        float targetScale = scaleUp ? 1 : 0;

        if (scaleUp)
        {
            onScaleUp?.Invoke();
        }
        else
        {
            onScaleDown?.Invoke();
        }

        while (scaleUp ? scaleProggres < targetScale : scaleProggres > targetScale)
        {
            yield return new WaitForFixedUpdate();
            if (scaleUp)
            {
                scaleProggres += Time.fixedDeltaTime / scaleTime;
            }
            else
            {
                scaleProggres -= Time.fixedDeltaTime / scaleTime;
            }
            
            float scale = Mathf.Lerp(smallSize, bigSize, Mathf.SmoothStep(0, 1, scaleProggres));

            if (scaleUp && !CanScaleUp(scale)) StopScaling();



            myRb.mass = Mathf.Lerp(smallMass, bigMass, Mathf.SmoothStep(0, 1, scaleProggres));
            transform.localScale = scale * Vector3.one;
            torque = Mathf.Lerp(smallTorque, bigTorque, Mathf.SmoothStep(0, 1, scaleProggres));
            maxVel = Mathf.Lerp(smallVel, bigVel, Mathf.SmoothStep(0, 1, scaleProggres));


        }

        
        scaleProggres = scaleUp ? 1 : 0;

        myRb.mass = Mathf.Lerp(smallMass, bigMass, scaleProggres);
        transform.localScale = Mathf.Lerp(smallSize, bigSize, scaleProggres) * Vector3.one;
        torque = Mathf.Lerp(smallTorque, bigTorque, scaleProggres);
        maxVel = Mathf.Lerp(smallVel, bigVel, scaleProggres);

        StopScaling();
    }
    bool CanScaleUp(float predictedScale)
    {
        Vector3[] directions =
        {
            Vector3.up, //0
            Vector3.right, //1
            Vector3.down, //2
            Vector3.left, //3
            Vector3.forward, //4
            Vector3.back //5
        };
        bool[] resoults = new bool[directions.Length];

        string resoult = "";

        for(int i = 0; i < directions.Length; i++)
        {
            //Debug.DrawRay(transform.position, directions[i] * (sphereCollider.radius * predictedScale + 0.02f));
            if(Physics.Raycast(transform.position, directions[i], sphereCollider.radius * predictedScale + 0.01f, layerMask, QueryTriggerInteraction.Ignore))
            {
                resoults[i] = true;

                resoult += ": 1 ";
            }
            else
            {
                resoults[i] = false;

                resoult += ": 0 ";
            }

        }



        if (( 
            (resoults[1] && resoults[3]) || //right left
            (resoults[0] && resoults[2]) || //up down
            (resoults[4] && resoults[5])    //forward back
            ))
        {
            return false;
        }
        
        return true;
    }
}
