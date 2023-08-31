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

    private LaserPointer laserPointerScript;

    private Vector3 leftFootStartPosition;
    private Vector3 rightFootStartPosition;
    private Vector3 hipsStartPosition;

    private float legLength;
    private const float modellegHeight = 0.7160985f;
    private float lerp = 0;
    private Vector3 leftFootGoal = new Vector3();
    private Vector3 rightFootGoal = new Vector3();
    private Vector3 calculatedHipsPos = new Vector3();

    private int index = 0;

    public float stepSpeed = 0.2f;
    public float stepHeight = 0.2f;
    private float calculatedStepHeight = 0f;

    
    void Start()
    {
        laserPointerScript = GameObject.FindObjectOfType<LaserPointer>();

        SetupStartPositions();

        legLength = Vector3.Distance(leftFootBase.position, hipsBase.position);
    }

    public void SetupStartPositions()
    {
        leftFootStartPosition = leftFoot.position;
        rightFootStartPosition = rightFoot.position;
        hipsStartPosition = hips.position;
    }

    void Update()
    {
        if (laserPointerScript.isPaused) return;

        if((laserPointerScript.goalsArray.Count <= 0 && lerp == 0f) || (laserPointerScript.isPaused && lerp == 0f))
        {
            return;
        }

        if(laserPointerScript.leftFootTurn) //Leva noga se pomera
        {
            if(lerp == 0f)
            {
                leftFootGoal = laserPointerScript.goalsArray.First.Value;
                laserPointerScript.goalsArray.RemoveFirst();
                calculatedHipsPos = CalculateHipsPosition(leftFootGoal, rightFootStartPosition);
                calculatedStepHeight = CalculateStepHeight(leftFootGoal, leftFootStartPosition, rightFootStartPosition);
            }
            rightFoot.position = rightFootStartPosition;

            lerp += stepSpeed * Time.deltaTime;

            hips.position = Vector3.Lerp(hipsStartPosition, calculatedHipsPos, lerp);

            Vector3 currentFoorPos = Vector3.Lerp(leftFootStartPosition, leftFootGoal, lerp);
            currentFoorPos.y += Mathf.Sin(lerp * Mathf.PI) * calculatedStepHeight;
            leftFoot.position = currentFoorPos;


            if (lerp > 1f) 
            {
                laserPointerScript.leftFootTurn = !laserPointerScript.leftFootTurn;
                leftFootStartPosition = currentFoorPos;
                hipsStartPosition = hips.position;
                lerp = 0f;
                index++;
            }

        } else //Desna noga se pomera
        {
            if(lerp == 0f)
            {
                rightFootGoal = laserPointerScript.goalsArray.First.Value;
                laserPointerScript.goalsArray.RemoveFirst();
                calculatedHipsPos = CalculateHipsPosition(rightFootGoal, leftFootStartPosition);
                calculatedStepHeight = CalculateStepHeight(rightFootGoal, rightFootStartPosition, leftFootStartPosition);
            }

            leftFoot.position = leftFootStartPosition;

            lerp += stepSpeed * Time.deltaTime;

            hips.position = Vector3.Lerp(hipsStartPosition, calculatedHipsPos, lerp);

            Vector3 currentFoorPos = Vector3.Lerp(rightFootStartPosition, rightFootGoal, lerp);
            currentFoorPos.y += Mathf.Sin(lerp * Mathf.PI) * calculatedStepHeight;
            rightFoot.position = currentFoorPos;


            if (lerp > 1f) 
            {
                laserPointerScript.leftFootTurn = !laserPointerScript.leftFootTurn;
                rightFootStartPosition = currentFoorPos;
                hipsStartPosition = hips.position;
                lerp = 0f;
                index++;
            }
        }

    }

    private Vector3 CalculateHipsPosition(Vector3 leftFootPosition, Vector3 rightFootPoition)
    {
        float feetY = 0f;
        if(leftFootPosition.y < rightFootPoition.y)
        {
            rightFootPoition.y = leftFootPosition.y;
            feetY = leftFootPosition.y;
        } else
        {
            leftFootPosition.y = rightFootPoition.y;
            feetY = rightFootPoition.y;
        }

        float feetDist = Vector3.Distance(rightFootPoition, leftFootPosition)/2;
        float hipHeight = legLength * legLength - feetDist * feetDist;

        //Debug.Log("Feet dist: " + feetDist + " LegLength: " + legLength + " HipHeight: " + hipHeight + "FeetY: " + feetY);

        Vector3 feetMiddlepoint = Vector3.Lerp(leftFootPosition, rightFootPoition, 0.5f); 

        return new Vector3(feetMiddlepoint.x, hipHeight - modellegHeight + (feetY - 0.1154f), feetMiddlepoint.z);
        
    } 
    
    private float CalculateStepHeight(Vector3 goalPosition, Vector3 oldPostion, Vector3 otherFootPostion)
    {
        float correction = 0.2f;
        float finalDiff = 0f;
        float feetYDiff = goalPosition.y - oldPostion.y;
        float otherFeetYDiff = otherFootPostion.y - oldPostion.y;

        Debug.Log("Diff " + feetYDiff + " Other: " + otherFeetYDiff);

        if(otherFeetYDiff > feetYDiff)
        {
            finalDiff = otherFeetYDiff + correction/2.2f;
        } else
        {
            finalDiff = feetYDiff - correction;
        }

        if(finalDiff < 0f)
        {
            finalDiff = 0.1f;
        }
        return stepHeight + finalDiff;
    }
}
