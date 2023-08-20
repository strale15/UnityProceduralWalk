using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingScript : MonoBehaviour
{
    public Transform leftFoot;
    public Transform rightFoot;
    public Transform hips;

    public Transform leftFootBase;
    public Transform rightFootBase;
    public Transform hipsBase;

    private Vector3 leftFootStartPosition;
    private Vector3 rightFootStartPosition;
    private Vector3 hipsStartPosition;

    private Vector3[] goalsArray;
    private float legLength;
    private const float modellegHeight = 0.7160985f;
    private float lerp = 0;
    private Vector3 leftFootGoal = new Vector3();
    private Vector3 rightFootGoal = new Vector3();

    private bool leftFootTurn = false;
    private int index = 0;

    public float stepSpeed = 0.2f;
    public float stepHegiht = 0.2f;

    
    void Start()
    {
        leftFootStartPosition = leftFoot.position;
        rightFootStartPosition = rightFoot.position;
        hipsStartPosition = hips.position;

        legLength = Vector3.Distance(leftFootBase.position, hipsBase.position);

        //Fill array with goals (starting with right foot)
        goalsArray = new Vector3[4];
        goalsArray[0] = new Vector3(0.238000005f,   0.12f, 0.736000001f);
        goalsArray[1] = new Vector3(-0.103f,        0.12f,1.09099996f);
        goalsArray[2] = new Vector3(0.238000005f,   0.12f, 1.78600001f);
        goalsArray[3] = new Vector3(-0.078000005f,  0.12f, 1.78600001f);
    }

    void Update()
    {    
        if(index >= goalsArray.Length && lerp == 0f)
        {
            return;
        }

        if(leftFootTurn) //Leva noga se pomera
        {
            if(lerp == 0f)
            {
                leftFootGoal = goalsArray[index];
            }
            rightFoot.position = rightFootStartPosition;

            hips.position = Vector3.Lerp(hipsStartPosition, CalculateHipsPosition(leftFootGoal, rightFootStartPosition), lerp);

            lerp += stepSpeed * Time.deltaTime;

            Vector3 currentFoorPos = Vector3.Lerp(leftFootStartPosition, leftFootGoal, lerp);
            currentFoorPos.y += Mathf.Sin(lerp * Mathf.PI) * stepHegiht;
            leftFoot.position = currentFoorPos;


            if (lerp > 1f) 
            {
                leftFootTurn = !leftFootTurn;
                leftFootStartPosition = currentFoorPos;
                hipsStartPosition = hips.position;
                lerp = 0f;
                index++;
            }

        } else //Desna noga se pomera
        {
            if(lerp == 0f)
            {
                rightFootGoal = goalsArray[index];
            }

            leftFoot.position = leftFootStartPosition;

            hips.position = Vector3.Lerp(hipsStartPosition, CalculateHipsPosition(rightFootGoal, leftFootStartPosition), lerp);

            lerp += stepSpeed * Time.deltaTime;

            Vector3 currentFoorPos = Vector3.Lerp(rightFootStartPosition, rightFootGoal, lerp);
            currentFoorPos.y += Mathf.Sin(lerp * Mathf.PI) * stepHegiht;
            rightFoot.position = currentFoorPos;


            if (lerp > 1f) 
            {
                leftFootTurn = !leftFootTurn;
                rightFootStartPosition = currentFoorPos;
                hipsStartPosition = hips.position;
                lerp = 0f;
                index++;
            }
        }

    }

    private Vector3 CalculateHipsPosition(Vector3 leftFootPosition, Vector3 rightFootPoition)
    {
        if(leftFootPosition.y < rightFootPoition.y)
        {
            rightFootPoition.y = leftFootPosition.y;
        } else
        {
            leftFootPosition.y = rightFootPoition.y;
        }

        float feetDist = Vector3.Distance(rightFootPoition, leftFootPosition)/2;
        float hipHeight = legLength * legLength - feetDist * feetDist;

        Vector3 feetMiddlepoint = Vector3.Lerp(leftFootPosition, rightFootPoition, 0.5f); 

        return new Vector3(feetMiddlepoint.x, hipHeight - modellegHeight, feetMiddlepoint.z);
        
    } 
}
