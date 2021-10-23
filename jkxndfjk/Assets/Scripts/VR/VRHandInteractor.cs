using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum Hand
{
    Left,
    Right
}
public class VRHandInteractor : MonoBehaviour
{
    [SerializeField] protected LayerMask interactableLayerMask;
    [SerializeField] private Hand hand = Hand.Left;
    [SerializeField] VRController controller;
    //[SerializeField] private VRController controller;
    [SerializeField] private Collider[] handColliders;
    [SerializeField] float interiaTensor = 0.1f;

    public Hand GetHand { get { return hand; } }
    public VRController Controller { get { return controller; } }

    public UnityEvent onTriggerButtonTrue;
    public UnityEvent onTriggerButtonFalse;

    public UnityEvent onGripButtonTrue;
    public UnityEvent onGripButtonFalse;

    public UnityEvent onFirstButtonTrue;
    public UnityEvent onFirstButtonFalse;

    public UnityEvent onSecondButtonTrue;
    public UnityEvent onSecondButtonFalse;

    public UnityEvent onJoystickButtonTrue;
    public UnityEvent onJoystickButtonFasle;


    public UnityEvent onHoverEnter;
    public UnityEvent onHoverExit;
    public UnityEvent onAttach;
    public UnityEvent onDetach;

    protected ConfigurableJoint myJoint;

    public List<VRGrabInteractable> gripHoveredObjects = new List<VRGrabInteractable>();
    private Rigidbody myRb;
    private VRGrabInteractable tempGripInteractable;
    public VRGrabInteractable GrabInteractable { get; set; }

    private void Awake()
    {
        myRb = GetComponent<Rigidbody>();
        myRb.solverIterations = 30;
        myRb.inertiaTensor = interiaTensor * Vector3.one;
    }
    void Start()
    {
        if (controller)
        {
            controller.onGripButtonTrue.AddListener(OnGripPressed);
            controller.onGripButtonFalse.AddListener(OnGripReleased);
        }
    }
    private void OnDisable()
    {
        controller.onGripButtonTrue.RemoveListener(OnGripPressed);
        controller.onGripButtonFalse.RemoveListener(OnGripReleased);
    }
    public void TryRemoveGripHoveredObject(VRGrabInteractable interactable)
    {
        if (gripHoveredObjects.Contains(interactable))
        {
            gripHoveredObjects.Remove(interactable);
        }
    }

    private void FixedUpdate()
    {
        tempGripInteractable = Calculate(tempGripInteractable, gripHoveredObjects);
        if (GrabInteractable)
        {
            GrabInteractable.ProcessFixedUpdate(Time.fixedDeltaTime);
        }
    }


    VRGrabInteractable Calculate(VRGrabInteractable _tempInteractable, List<VRGrabInteractable> _hoveredObjects)
    {
        if (_hoveredObjects.Count == 0)
        {
            Controller.OnHoverExit();
            return null;
        }
        else
        {
            float weightReference = Mathf.Infinity;
            VRGrabInteractable previousInteractable = _tempInteractable;
            VRGrabInteractable hoveredInteractable = null;

            foreach (VRGrabInteractable interactable in _hoveredObjects)
            {
                if (!interactable.Hoverable || interactable.HandInteractors.Count > 0)
                {
                    break;
                }

                float calculated = interactable.HoverWeight * (interactable.transform.position - transform.position).magnitude;

                if (calculated < weightReference)
                {
                    weightReference = calculated;
                    hoveredInteractable = interactable;
                }
                else
                {
                    if (interactable == previousInteractable)
                    {
                        interactable.OnHoverExit(this); //TODO: daje sygna³ mimo ¿e nie by³ wczeœnieje podswietlony
                        OnHoverExit(interactable);
                    }
                }
            }
            _tempInteractable = hoveredInteractable;
            //_tempInteractable.GetAttachTransform(this);
            if (_tempInteractable != previousInteractable && _tempInteractable != null)
            {
                Controller.OnHoverEnter();
                _tempInteractable.OnHoverEnter(this);
                OnHoverEnter(_tempInteractable);
            }
            return _tempInteractable;
        }
    }
    public void OnHoverEnter(VRGrabInteractable _interactableBase)
    {
        onHoverEnter.Invoke();
    }
    public  void OnHoverExit(VRGrabInteractable _interactableBase)
    {
        onHoverExit.Invoke();
    }
    


    public void OnAttach(VRGrabInteractable _interactableBase)
    {
        onAttach?.Invoke();
    }
    public  void OnDetach(VRGrabInteractable _interactableBase)
    {
        onDetach.Invoke();
    }
    
    public  void TryToAttach(VRGrabInteractable _interactable)
    {
        if (_interactable == null) return;


        Attach(_interactable);
        tempGripInteractable.OnHoverExit(this);
        tempGripInteractable = null;
        gripHoveredObjects.Clear();
    }
    void Attach(VRGrabInteractable _interactalbe)
    {
        _interactalbe.OnAttachBegin(this);
        EnableAllColliders(false);

        //create joint
        ConfigureJoint();

        _interactalbe.AddHandInteractor(this);
        _interactalbe.OnAttachEnd(this);
        controller.OnAttach();
        OnAttach(_interactalbe);
    }
    private void ConfigureJoint()
    {
        Rigidbody rb = GrabInteractable.GetComponent<Rigidbody>();
        myJoint = this.gameObject.AddComponent<ConfigurableJoint>();
        myJoint.connectedBody = rb;

        myJoint.connectedMassScale = 1f;
        myJoint.massScale = 1f;
        myJoint.axis = new Vector3(0, 0, 0);
        myJoint.secondaryAxis = new Vector3(0, 1.5f, 1.5f);

        
        myJoint.angularXMotion = ConfigurableJointMotion.Locked;
        myJoint.angularYMotion = ConfigurableJointMotion.Locked;
        myJoint.angularZMotion = ConfigurableJointMotion.Locked;
        myJoint.xMotion = ConfigurableJointMotion.Locked;
        myJoint.yMotion = ConfigurableJointMotion.Locked;
        myJoint.zMotion = ConfigurableJointMotion.Locked;

        //myJoint.breakForce = 800f;
        //myJoint.breakTorque = 35f;
        //myJoint.projectionMode = JointProjectionMode.None;
    }
    public  void TryToDetach()
    {
        if (!GrabInteractable) return;
        if (!myJoint) return;
        Destroy(myJoint);
        GrabInteractable.RemoveHandInteractor(this);
        GrabInteractable.OnDetach(this);
        controller.OnDetach();
        OnDetach(GrabInteractable);

        EnableAllColliders(true);
        GrabInteractable = null;
    }
    public void OnGripPressed()
    {
        onGripButtonTrue.Invoke();
        if (tempGripInteractable != null)
        {
            GrabInteractable = tempGripInteractable;
            TryToAttach(GrabInteractable);
        }
        
    }
    public void OnGripReleased()
    {
        onGripButtonFalse.Invoke();
        TryToDetach();
    }
    public  void OnTriggerEntered(Collider _other)
    {
        if (GrabInteractable)
        {
            return;
        }
        if (_other.gameObject.TryGetComponent<VRGrabInteractable>(out VRGrabInteractable interactable))
        {
            if (!interactable.Hoverable || interactable.HandInteractors.Count > 0)
            {
                return;
            }
            if (gripHoveredObjects.Count >= 5)
            {
                return;
            }
            else
            {
                bool theSame = false;
                foreach (VRGrabInteractable temopInteractable in gripHoveredObjects) //check if there is no doubles
                {
                    if (temopInteractable == interactable)
                    {
                        theSame = true;
                    }
                }
                if (!theSame)
                {
                    
                    gripHoveredObjects.Add(interactable);
                }
                else { return; }

            }
        }
    }
    public  void OnTriggerExited(Collider _other)
    {
        if (GrabInteractable)
        {
            return;
        }
        if (_other.gameObject.TryGetComponent<VRGrabInteractable>(out VRGrabInteractable _interactable))
        {
            _interactable.OnHoverExit(this);
            
            gripHoveredObjects.Remove(_interactable);
            tempGripInteractable = null;
            
        }
    }
    private void OnTriggerEnter(Collider _other)
    {
        if (((1 << _other.gameObject.layer) & interactableLayerMask) != 0)
        {
            OnTriggerEntered(_other);
        }
    }
    private void OnTriggerExit(Collider _other)
    {
        if (((1 << _other.gameObject.layer) & interactableLayerMask) != 0)
        {
            OnTriggerExited(_other);
        }
    }

    void EnableAllColliders(bool _state)
    {
        foreach (Collider collider in handColliders)
        {
            if (!collider.isTrigger)
            {
                collider.enabled = _state;
            }
        }
    }
}

