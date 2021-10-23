using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeChanger : MonoBehaviour
{
    public void ChangeMode()
    {
        StaticGameController.Instance.ChangeMode();
    }
}
