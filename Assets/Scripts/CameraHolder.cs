using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHolder : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform camPos;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = camPos.position;
    }
}
