using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

enum Mode {
    StepGiver,
    MovePlayer
}
public class LaserPointer : MonoBehaviour
{
    public LayerMask mask;
    public Transform marker;
    public GameObject feetMark1;
    public GameObject feetMark2;
    public GameObject playerPrefab;

    public LinkedList<Vector3> goalsArray = new LinkedList<Vector3>();

    private LinkedList<GameObject> feetMarkers = new LinkedList<GameObject>();

    public bool isPaused = true;
    public bool leftFootTurn = false;
    private bool leftFootDrawTurn = false;

    private RaycastHit hitInfo;
    private Mode mode;
    private GameObject playerObject;
    private WalkingScript walkingScript;
    void Start()
    {
        playerObject = Instantiate(playerPrefab, new Vector3(0, 0, -2), Quaternion.identity);
        walkingScript = GameObject.FindObjectOfType<WalkingScript>();
        mode = Mode.StepGiver;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if(Physics.Raycast(ray, out hitInfo, 30f, mask))
        {
            if(mode == Mode.StepGiver)
            {
                if(Input.GetKeyDown(KeyCode.E))
                {
                    Vector3 stepGoal = hitInfo.point;
                    stepGoal.y += 0.1154f;
                    stepGoal.z -= 0.1f;

                    //Dodaj u niz za hodanje
                    goalsArray.AddLast(stepGoal);

                    //Nacrtaj stopu
                    if(leftFootDrawTurn)
                    {
                        GameObject feetObj = Instantiate(feetMark1, hitInfo.point, Quaternion.Euler(90, 0, 0));
                        feetMarkers.AddLast(feetObj);
                        leftFootDrawTurn = false;
                    } else
                    {
                        GameObject feetObj = Instantiate(feetMark2, hitInfo.point, Quaternion.Euler(90, 0, 0));
                        feetMarkers.AddLast(feetObj);
                        leftFootDrawTurn = true;
                    }

                }
            
                marker.position = hitInfo.point;
                marker.localScale = new Vector3(0.124949999f, 0.124949999f, 0.124949999f);
            }

            else if(mode == Mode.MovePlayer)
            {
                if(Input.GetKeyDown(KeyCode.E))
                {
                    mode = Mode.StepGiver;
                    walkingScript.SetupStartPositions();
                    ClearSteps();
                }
                else
                {
                    playerObject.transform.position = hitInfo.point;
                }
            }
            
            
        } else
        {
            marker.localScale = new Vector3(0, 0, 0);
        }

        if(Input.GetKeyDown(KeyCode.P))
        {
            isPaused = !isPaused;
        }

        if(Input.GetKeyDown(KeyCode.C))
        {
            ClearSteps();
        }

        if(Input.GetKeyDown(KeyCode.M))
        {
            if(mode != Mode.MovePlayer)
            {
                ClearSteps();
                Destroy(playerObject);
                playerObject = Instantiate(playerPrefab, new Vector3(), Quaternion.identity);
                walkingScript = GameObject.FindObjectOfType<WalkingScript>();

                marker.localScale = new Vector3(0, 0, 0);

                mode = Mode.MovePlayer;
            }
            else if(mode != Mode.StepGiver)
            {
                ClearSteps();
                mode = Mode.StepGiver;
            }
        }
        

        Debug.DrawRay(transform.position, transform.forward * 30f, Color.red);
        
    }

    private void FixedUpdate()
    {
    }

    private void ClearSteps()
    {
        foreach(GameObject go in feetMarkers) 
        {
            Destroy(go);
        }
        feetMarkers.Clear();
        goalsArray.Clear();
        leftFootTurn = false;
        leftFootDrawTurn = false;
        
    }
}
