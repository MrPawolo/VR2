using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VRController : MonoBehaviour
{
    [SerializeField] ControllerSettingsSO controllerSettings;
    [SerializeField] VRControllerLinkBase controllerLink;
    [SerializeField] VRHandInteractor handInteractor;
    [Range(0, 1)] public float triggerButtonThreshold = 0.75f;
    [Range(0, 1)] public float gripButtonThreshold = 0.75f;


    public VRHandInteractor HandInteractor { get { return handInteractor; } }
    #region Inputs
    //If you need more information about your inpouts you can add them here and in UpdateInputs()
    //Visit https://docs.unity3d.com/2019.4/Documentation/Manual/xr_input.html to get more info
    public Vector2 JoyStickVal { get; private set; }
    public float TriggerVal { get; private set; }
    public bool TriggerButton { get; private set; }
    public float GripVal { get; private set; }
    public bool GripButton { get; private set; }
    public bool FirstButton { get; private set; }
    public bool FirstButtonTouch { get; private set; }
    public bool SecondButton { get; private set; }
    public bool SecondButtonTouch { get; private set; }
    public bool JoyStickButton { get; private set; }
    public bool JoyStickButtonTouch { get; private set; }
    #endregion

    #region Helpers
    private bool lastTriggerButtonState = false;
    private bool lastGripButtonState = false;
    private bool lastFirstButtonState = false;
    private bool lastSecondButtonState = false;
    private bool lastJoystickButtonState = false;
    #endregion

    #region Unity Events
    public UnityEvent onTriggerButtonTrue;
    public UnityEvent onTriggerButtonFalse;

    public UnityEvent onGripButtonTrue;
    public UnityEvent onGripButtonFalse;

    public UnityEvent onFirstButtonTrue;
    public UnityEvent onFirstButtonFalse;

    public UnityEvent onFirstButtonTouch;

    public UnityEvent onSecondButtonTrue;
    public UnityEvent onSecondButtonFalse;

    public UnityEvent onSecondButtonTouch;

    public UnityEvent onJoystickButtonTrue;
    public UnityEvent onJoystickButtonFasle;

    public UnityEvent onJoyStickButtonTouch;
    #endregion
    void Start()
    {
        
    }

    private void Update()
    {
        UpdateControllerInputs();
        UpdateEvents();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.localPosition = UpdateControllerPos();
        transform.localRotation = UpdateControllerRot();
    }
    public Vector3 UpdateControllerPos()
    {
        return controllerLink.GetControllerLocalPostion();
    }
    public Quaternion UpdateControllerRot()
    {
        return controllerLink.GetControllerLocalRotation();
    }
    private void UpdateControllerInputs()
    {
        if (controllerLink)
        {
            JoyStickVal = controllerLink.GetJoyStickVal();
            JoyStickButton = controllerLink.GetJoyStickClick();
            JoyStickButtonTouch = controllerLink.GetJoyStickTouch();
            TriggerVal = controllerLink.GetTriggerVal();
            TriggerButton = TriggerVal > triggerButtonThreshold;
            GripVal = controllerLink.GetGripVal();
            GripButton = GripVal > gripButtonThreshold;
            FirstButton = controllerLink.GetFirstButton();
            FirstButtonTouch = controllerLink.GetFirstButtonTouch();
            SecondButton = controllerLink.GetSecondButton();
            SecondButtonTouch = controllerLink.GetSecondButtonTouch();
        }
    }
    private void UpdateEvents()
    {
        if (TriggerButton && !lastTriggerButtonState) { onTriggerButtonTrue.Invoke(); lastTriggerButtonState = true; }// HandInteractor?.OnTriggerPressed(); }
        else if (!TriggerButton && lastTriggerButtonState) { onTriggerButtonFalse.Invoke(); lastTriggerButtonState = false; }// HandInteractor?.OnTriggerReleased(); }

        if (GripButton && !lastGripButtonState) { onGripButtonTrue.Invoke(); lastGripButtonState = true; }// HandInteractor?.OnGripPressed(); }
        else if (!GripButton && lastGripButtonState) { onGripButtonFalse.Invoke(); lastGripButtonState = false; }// HandInteractor?.OnGripReleased(); }


        if (FirstButton && !lastFirstButtonState) { onFirstButtonTrue.Invoke(); lastFirstButtonState = true; }
        else if (!FirstButton && lastFirstButtonState) { onFirstButtonFalse.Invoke(); lastFirstButtonState = false; }

        if (FirstButtonTouch) onFirstButtonTouch.Invoke();

        if (SecondButton && !lastSecondButtonState) { onSecondButtonTrue.Invoke(); lastSecondButtonState = true; }
        else if (!SecondButton && lastSecondButtonState) { onSecondButtonFalse.Invoke(); lastSecondButtonState = false; }

        if (SecondButtonTouch) onSecondButtonTouch.Invoke();

        if (JoyStickButton && !lastJoystickButtonState) { onJoystickButtonTrue.Invoke(); lastJoystickButtonState = true; }
        else if (!JoyStickButton && lastJoystickButtonState) { onJoystickButtonFasle.Invoke(); lastJoystickButtonState = false; }

        if (JoyStickButtonTouch) onJoyStickButtonTouch.Invoke();
    }

    public void SendHapticImpulse(float duration, float amplitude)
    {
        controllerLink.SendHapticImpulse(duration, amplitude);
    }
    public void OnHoverEnter()
    {
        if (controllerSettings.HapticOnHoverEnter)
        {
            SendHapticImpulse(controllerSettings.HoverEnterHapticDuration, controllerSettings.HoverEnterHapticAmount);
        }
    }
    public void OnHoverExit()
    {
        if (controllerSettings.HapticOnHoverExit)
        {
            SendHapticImpulse(controllerSettings.HoverExitHapticDuration, controllerSettings.HoverExitHapticAmount);
        }
    }
    public void OnAttach()
    {
        if (controllerSettings.HapticOnAttach)
        {
            SendHapticImpulse(controllerSettings.AttachHapticDuration, controllerSettings.AttachHapticAmount);
        }
    }
    public void OnDetach()
    {
        if (controllerSettings.HapticOnDetach)
        {
            SendHapticImpulse(controllerSettings.DetachHapticDuration, controllerSettings.DetachHapticAmount);
        }
    }
}
