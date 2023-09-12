using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootSolver : MonoBehaviour
{
    public float footSpacing;
    public Transform body;
    public LayerMask groundLayer;
    public bool hit;
    public float corection = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        footSpacing = transform.localPosition.x;
        hit = false;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(body.position + (body.right * footSpacing), Vector3.down);
        Debug.DrawRay(ray.origin, ray.direction, Color.red);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 15, groundLayer.value))
        {
            transform.position = hitInfo.point + (Vector3.up * corection);
            hit = true;
        }
        else hit = false;
    }
}
