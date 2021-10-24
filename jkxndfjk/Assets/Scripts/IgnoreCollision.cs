using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreCollision : MonoBehaviour
{
    public GameObject[] layer1;
    public GameObject[] layer2;
    void Awake()
    {
        for(int i = 0; i < layer1.Length; i++)
        {
            Collider[] col1 = layer1[i].GetComponents<Collider>();
            for(int j = 0; j < col1.Length; j++)
            {
                for(int k = 0; k < layer2.Length; k++)
                {
                    Collider[] col2 = layer2[k].GetComponents<Collider>();
                    for(int l = 0; l < col2.Length; l++)
                    {
                        Physics.IgnoreCollision(col1[j], col2[l]);
                    }
                }
            }
        }
    }

}
