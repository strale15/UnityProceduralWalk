using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPointer : MonoBehaviour
{
    public LayerMask mask;
    public Transform marker;
    public GameObject feetMark1;
    public GameObject feetMark2;

    public LinkedList<Vector3> goalsArray = new LinkedList<Vector3>();
    public bool isPaused = true;
    public bool leftFeet = false;

    private RaycastHit hitInfo;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if(Physics.Raycast(ray, out hitInfo, 30f, mask))
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                Vector3 stepGoal = hitInfo.point;
                stepGoal.y += 0.1154f;
                stepGoal.z -= 0.1f;

                //Dodaj u niz za hodanje
                goalsArray.AddLast(stepGoal);

                //Nacrtaj stopu
                if(leftFeet)
                {
                    Instantiate(feetMark1, hitInfo.point, Quaternion.Euler(90, 0, 0));
                    leftFeet = false;
                } else
                {
                    Instantiate(feetMark2, hitInfo.point, Quaternion.Euler(90, 0, 0));
                    leftFeet = true;
                }

            }
            
            marker.position = hitInfo.point;
            marker.localScale = new Vector3(0.124949999f, 0.124949999f, 0.124949999f);
            
        } else
        {
            marker.localScale = new Vector3(0, 0, 0);
        }

        if(Input.GetKeyDown(KeyCode.P))
        {
            isPaused = !isPaused;
        }
        

        Debug.DrawRay(transform.position, transform.forward * 30f, Color.red);
        
    }

    private void FixedUpdate()
    {
    }
}
