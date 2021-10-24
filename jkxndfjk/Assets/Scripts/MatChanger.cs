using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatChanger : MonoBehaviour
{
    public Material red;
    public Material blue;
    public int i = 1;
    public void ChangeToRed(bool state)
    {
        Material[] mats = GetComponent<MeshRenderer>().materials;
        if (state)
        {
            mats[i] = red;
            Debug.Log("1");
        }
        else
        {
            mats[i] = blue;
            Debug.Log("2");
        }
        GetComponent<MeshRenderer>().materials = mats;
    }
}
