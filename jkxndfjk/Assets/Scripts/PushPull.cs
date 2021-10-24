using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PushPull : MonoBehaviour
{
    public Transform start;
    public Transform end;
    float dist;


    public bool push = true;
    public bool distanceRelative = true;
    public float pushForce = 1;
    public UnityEvent onPushChange;
    public UnityEvent onPullChange;

    private void Start()
    {
        dist = end.localPosition.z - start.localPosition.z;
        
    }
    private void OnEnable()
    {
        StaticGameController.Instance.onStateChange += OnChange;
        CheckState();
    }
    private void OnDisable()
    {
        StaticGameController.Instance.onStateChange -= OnChange;
    }

    [ContextMenu("asdfgag")]
    void OnChange()
    {
        push = !push;
        CheckState();
    }

    
    void CheckState()
    {
        if (push)
        {
            onPushChange?.Invoke();
            pushForce = pushForce < 0 ? -pushForce : pushForce; //if less than 0 then inverse
        }
        else
        {
            onPullChange?.Invoke();
            pushForce = pushForce > 0 ? -pushForce : pushForce; //if more than 0 then inverse
        }
    }
    private void OnValidate()
    {
        CheckState();
    }

    private void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb)
        {
            //Vector3 projected = Vector3.Project(other.transform.position, end.position - start.position);
            //debuCube.transform.position =  projected;
            if (distanceRelative)
            {
                Vector3 otherInLocal = transform.InverseTransformPoint(other.transform.position);
                float relDist = 1 - ((otherInLocal.z - start.position.z) / dist);
                rb.AddForce(transform.forward * pushForce * relDist, ForceMode.Acceleration);
            }
            else
            {
                rb.AddForce(transform.forward * pushForce, ForceMode.Acceleration);
            }

            
        }
    }
}
