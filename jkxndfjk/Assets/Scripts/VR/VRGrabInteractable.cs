using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VRGrabInteractable : MonoBehaviour
{
    [SerializeField]bool hoverable = true;
    public bool Hoverable { get { return hoverable; } set { hoverable = value; } }

    [SerializeField] float hoverWeight = 1;
    public float HoverWeight { get { return hoverWeight; } }



    List<VRHandInteractor> handInteractors = new List<VRHandInteractor>();
    public List<VRHandInteractor> HandInteractors { get { return handInteractors; } }
    Rigidbody myRb;


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

        
        //EnableAllHovers(false);
    }
    public virtual void OnDetach(VRHandInteractor handInteractor)
    {
        onDetach?.Invoke();
        
        //EnableAllHovers(true);
        if (handInteractors.Count == 0)
        {
            onObjectDropped?.Invoke();
        }
    }
}
