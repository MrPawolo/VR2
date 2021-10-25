using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Pose
{
    public string name;
    public Vector3 pos;
    public Vector3 rot;
}
public class PosHolder : MonoBehaviour
{
    public Pose[] poses;
    public int loadPoseIndex;
    void Start()
    {
        if(poses.Length > loadPoseIndex)
        {
            transform.localPosition = poses[loadPoseIndex].pos;
            transform.localRotation = Quaternion.Euler(poses[loadPoseIndex].pos);
        }
    }

}
