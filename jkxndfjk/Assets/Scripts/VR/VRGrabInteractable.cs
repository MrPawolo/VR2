using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VRGrabInteractable : MonoBehaviour
{
    [SerializeField]bool hoverable = true;
    public float distanceDetach = 0.2f;
    public bool Hoverable { get { return hoverable; } set { hoverable = value; } }

    [SerializeField] float hoverWeight = 1;
    public float HoverWeight { get { return hoverWeight; } }



    List<VRHandInteractor> handInteractors = new List<VRHandInteractor>();
    public List<VRHandInteractor> HandInteractors { get { return handInteractors; } }
    Rigidbody myRb;


    Transform attachedTrans;
    #region UnityEvents
    public UnityEvent<VRGrabInteractable> onAttachBegin;
    public UnityEvent onAttachEnd;
    public UnityEvent onDetach;
    public UnityEvent onHoverEnter;
    public UnityEvent onHoverExit;
    public UnityEvent onTriggerTrue;
    public UnityEvent onTriggerFalse;
    public UnityEvent onObjectDropped;
    #endregion

    public virtual void ProcessFixedUpdate(float _deltaTime)
    {
        if (distanceDetach == 0) return;
        if(handInteractors.Count > 0)
        {
            Vector3 controllerPos = handInteractors[0].Controller.transform.position;
            if (Vector3.Distance(controllerPos, attachedTrans.position) > distanceDetach)
            {
                handInteractors[0].TryToDetach();
            }
        }
    }
    public void AddHandInteractor(VRHandInteractor _interactor)
    {
        if (!handInteractors.Contains(_interactor))
        {
            handInteractors.Add(_interactor);
        }
    }
    public void RemoveHandInteractor(VRHandInteractor _interactor)
    {
        if (handInteractors.Contains(_interactor))
        {
            handInteractors.Remove(_interactor);
        }
    }
    public virtual void OnHoverEnter(VRHandInteractor handInteractor)
    {
        //GetAttachTransform(handInteractor);
        onHoverEnter?.Invoke();
    }
    public virtual void OnHoverExit(VRHandInteractor handInteractor)
    {
        //attachTransform?.DistableAllHovers();
        onHoverExit?.Invoke();
    }
    public virtual void OnAttachBegin(VRHandInteractor handInteractor)
    {
        onAttachBegin?.Invoke(this);
    }
    public virtual void OnAttachEnd(VRHandInteractor handInteractor)
    {
        myRb = GetComponent<Rigidbody>();
        onAttachEnd?.Invoke();

        //attachTransform?.DistableAllHovers();
        attachedTrans = new GameObject().transform;
        attachedTrans.position = handInteractor.transform.position;
        attachedTrans.rotation = handInteractor.transform.rotation;
        attachedTrans.SetParent(this.transform);
        attachedTrans.name = "attachedTrans";

        
        //EnableAllHovers(false);
    }
    public virtual void OnDetach(VRHandInteractor handInteractor)
    {
        onDetach?.Invoke();
        if (attachedTrans)
        {
            Destroy(attachedTrans.gameObject);
        }
        //EnableAllHovers(true);
        if (handInteractors.Count == 0)
        {
            onObjectDropped?.Invoke();
        }
    }
}
