using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Burst.CompilerServices;
using UnityEditor.Sprites;
using UnityEngine;
using UnityEngine.Timeline;

public enum Mode {
    StepGiver,
    MovePlayer
}
public class LaserPointer : MonoBehaviour
{
    public LayerMask mask;
    public Transform leftMarker;
    public Transform rightMarker;
    public GameObject leftFootMarker;
    public GameObject rightFootMarker;
    public GameObject playerPrefab;

    public LinkedList<Vector3> goalsArray = new LinkedList<Vector3>();
    public LinkedList<Vector3> footAngleArray = new LinkedList<Vector3>();

    private LinkedList<GameObject> feetMarkers = new LinkedList<GameObject>();

    public bool isPaused = true;
    public bool leftFootTurn = false;
    public bool leftFootDrawTurn = false;

    private RaycastHit hitInfo;
    public Mode mode;
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
                    stepGoal += hitInfo.normal * 0.1154f;

                    //Dodaj u niz za hodanje
                    goalsArray.AddLast(stepGoal);
                    footAngleArray.AddLast(hitInfo.normal);

                    //Nacrtaj stopu
                    if(leftFootDrawTurn)
                    {
                        GameObject feetObj = Instantiate(leftFootMarker, hitInfo.point, Quaternion.FromToRotation(Vector3.up, hitInfo.normal));
                        feetMarkers.AddLast(feetObj);
                        leftFootDrawTurn = false;
                    } else
                    {
                        GameObject feetObj = Instantiate(rightFootMarker, hitInfo.point, Quaternion.FromToRotation(Vector3.up, hitInfo.normal));
                        feetMarkers.AddLast(feetObj);
                        leftFootDrawTurn = true;
                    }

                }

                if(leftFootDrawTurn)
                {
                    leftMarker.position = hitInfo.point;
                    leftMarker.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
                    leftMarker.localScale = new Vector3(1, 1, 1);
                }
                else
                {
                    rightMarker.position = hitInfo.point;
                    rightMarker.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
                    rightMarker.localScale = new Vector3(1, 1, 1);
                }
            
                
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
            leftMarker.localScale = new Vector3(0, 0, 0);
            rightMarker.localScale = new Vector3(0, 0, 0);
        }

        if(Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.R))
        {
            isPaused = !isPaused;
        }

        if(Input.GetKeyDown(KeyCode.C))
        {
            ClearSteps();
        }

        if(Input.GetKeyDown(KeyCode.M))
        {
            isPaused = true;
            if(mode != Mode.MovePlayer)
            {
                ClearSteps();
                Destroy(playerObject);
                playerObject = Instantiate(playerPrefab, new Vector3(), Quaternion.identity);
                walkingScript = GameObject.FindObjectOfType<WalkingScript>();

                leftMarker.localScale = new Vector3(0, 0, 0);
                rightMarker.localScale = new Vector3(0, 0, 0);

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

    public void FinishStep()
    {
        Destroy(feetMarkers.First.Value);
        feetMarkers.RemoveFirst();
    }
}
