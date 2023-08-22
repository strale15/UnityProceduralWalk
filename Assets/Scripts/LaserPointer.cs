using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPointer : MonoBehaviour
{
    public LayerMask mask;
    public Transform marker;

    private RaycastHit hitInfo;
    private bool rayHit;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        rayHit = false;
        if(Physics.Raycast(ray, out hitInfo, 30f, mask))
        {
            if(Input.GetKey(KeyCode.E))
            {
                Debug.Log(hitInfo.point);
            }
            
            marker.position = hitInfo.point;
            marker.localScale = new Vector3(0.124949999f, 0.124949999f, 0.124949999f);
            
        } else
        {
            marker.localScale = new Vector3(0, 0, 0);
        }
        

        Debug.DrawRay(transform.position, transform.forward * 30f, Color.red);
        
    }

    private void FixedUpdate()
    {
    }
}
