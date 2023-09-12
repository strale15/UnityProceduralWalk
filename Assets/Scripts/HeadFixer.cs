using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadFixer : MonoBehaviour
{
    public Transform head;
    void Start()
    {
        
    }
    void Update()
    {
        head.rotation = Quaternion.LookRotation(Vector3.forward);
    }
}
