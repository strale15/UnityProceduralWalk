using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingScript : MonoBehaviour
{
    public AnimationCurve HipsCurve;
    public AnimationCurve FeetCurve;

    public Transform leftFoot;
    public Transform rightFoot;
    public Transform hips;

    public Transform leftArmTarget;
    public Transform rightArmTarget;
    public Transform leftZbica;
    public Transform rightZbica;
    public Transform leftShoulder;
    public Transform rightShoulder;

    public Transform leftFootBase;
    public Transform rightFootBase;
    public Transform hipsBase;

    private LaserPointer laserPointerScript;

    private Vector3 leftFootStartPosition;
    private Vector3 rightFootStartPosition;
    private Vector3 hipsStartPosition;
    private Vector3 leftArmStartPosition;
    private Vector3 rightArmStartPosition;
    private Quaternion leftShoulderStartRotation;
    private Quaternion rightShoulderStartRotation;

    private float legLength;
    private const float modellegHeight = 0.7160985f;
    private float lerp = 0;
    private Vector3 leftFootGoal = new Vector3();
    private Vector3 rightFootGoal = new Vector3();
    private Vector3 calculatedHipsPos = new Vector3();
    private Vector3 leftArmGoal = new Vector3();
    private Vector3 rightArmGoal = new Vector3();
    private Quaternion rightShoulderGoalRotation = new Quaternion();
    private Quaternion leftShoulderGoalRotation = new Quaternion();
    private Quaternion currentRightShoulderRotation = new Quaternion();
    private Quaternion currentLeftShoulderRotation = new Quaternion();
    private Quaternion leftFootGoalRotation = new Quaternion();
    private Quaternion rightFootGoalRotation = new Quaternion();
    private Quaternion leftFootStartRot = new Quaternion();
    private Quaternion rightFootStartRot = new Quaternion();

    private Quaternion leftFootStartRotLerp = new Quaternion();
    private Quaternion rightFootStartRotLerp = new Quaternion();
    private float handHipDistance = 0;

    private int index = 0;

    public float stepSpeed = 0.2f;
    public float stepHeight = 0.2f;
    private float calculatedStepHeight = 0f;

    
    void Start()
    {
        laserPointerScript = GameObject.FindObjectOfType<LaserPointer>();

        SetupStartPositions();

        legLength = Vector3.Distance(leftFootBase.position, hipsBase.position);
        handHipDistance = Mathf.Abs(hipsBase.position.x - rightArmTarget.position.x);
    }

    public void SetupStartPositions()
    {
        leftFootStartPosition = leftFoot.position;
        rightFootStartPosition = rightFoot.position;
        hipsStartPosition = hips.position;

        leftArmStartPosition = leftArmTarget.position;
        rightArmStartPosition = rightArmTarget.position;

        leftShoulderStartRotation = Quaternion.Euler(leftShoulder.localRotation.eulerAngles);
        rightShoulderStartRotation = Quaternion.Euler(rightShoulder.localRotation.eulerAngles);

        leftFootStartRot = leftFoot.rotation;
        rightFootStartRot = rightFoot.rotation;
        leftFootStartRotLerp = leftFoot.rotation;
        rightFootStartRotLerp = rightFoot.rotation;
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

                //Racunanje goal-a za ruke
                rightArmGoal = new Vector3(calculatedHipsPos.x + handHipDistance, calculatedHipsPos.y + 1f, calculatedHipsPos.z + 0.1f);
                leftArmGoal = new Vector3(calculatedHipsPos.x - handHipDistance, calculatedHipsPos.y + 1f , calculatedHipsPos.z - 0.07f);

                //Pomeranje ramena
                rightShoulderGoalRotation = Quaternion.Euler(
                    rightShoulderStartRotation.eulerAngles.x, rightShoulderStartRotation.eulerAngles.y, rightShoulderStartRotation.eulerAngles.z + 25f);

                leftShoulderGoalRotation = Quaternion.Euler(
                    leftShoulderStartRotation.eulerAngles.x, leftShoulderStartRotation.eulerAngles.y, leftShoulderStartRotation.eulerAngles.z + 25f);

                currentRightShoulderRotation = rightShoulder.localRotation;
                currentLeftShoulderRotation = leftShoulder.localRotation;

                //Racunanje goal rotacije za stopalo
                leftFootGoalRotation = Quaternion.FromToRotation(Vector3.up, laserPointerScript.footAngleArray.First.Value) * leftFootStartRot;
                laserPointerScript.footAngleArray.RemoveFirst();

                leftFoot.rotation = leftFootGoalRotation;
            }
            rightFoot.position = rightFootStartPosition;

            lerp += stepSpeed * Time.deltaTime;

            //Hips lerp
            hips.position = Vector3.Lerp(hipsStartPosition, calculatedHipsPos, HipsCurve.Evaluate(lerp));

            //Pomeranje desne ruke u napred dok se leva noga pomera
            rightArmTarget.position = Vector3.Lerp(rightArmStartPosition, rightArmGoal, lerp);
            leftArmTarget.position = Vector3.Lerp(leftArmStartPosition, leftArmGoal, lerp);

            //Slerp ramena
            leftShoulder.localRotation = Quaternion.Slerp(currentLeftShoulderRotation, leftShoulderGoalRotation, lerp);
            rightShoulder.localRotation = Quaternion.Slerp(currentRightShoulderRotation, rightShoulderGoalRotation, lerp);

            //Slerp stopala
            leftFoot.rotation = Quaternion.Slerp(leftFootStartRotLerp, leftFootGoalRotation, lerp);

            Vector3 currentFootPos = Vector3.Lerp(leftFootStartPosition, leftFootGoal, FeetCurve.Evaluate(lerp));
            currentFootPos.y += Mathf.Sin(FeetCurve.Evaluate(lerp) * Mathf.PI) * calculatedStepHeight;
            leftFoot.position = currentFootPos;


            if (lerp > 1f) 
            {
                laserPointerScript.leftFootTurn = !laserPointerScript.leftFootTurn;
                leftFootStartPosition = currentFootPos;
                hipsStartPosition = hips.position;

                rightArmStartPosition = rightArmTarget.position;
                leftArmStartPosition = leftArmTarget.position;

                leftFootStartRotLerp = leftFoot.rotation;

                laserPointerScript.FinishStep();

                lerp = 0f;
            }

        } else //Desna noga se pomera
        {
            if(lerp == 0f)
            {
                rightFootGoal = laserPointerScript.goalsArray.First.Value;
                laserPointerScript.goalsArray.RemoveFirst();
                calculatedHipsPos = CalculateHipsPosition(rightFootGoal, leftFootStartPosition);
                calculatedStepHeight = CalculateStepHeight(rightFootGoal, rightFootStartPosition, leftFootStartPosition);

                //Racunanje goal-a za ruke
                leftArmGoal = new Vector3(calculatedHipsPos.x - handHipDistance, calculatedHipsPos.y + 1f, calculatedHipsPos.z + 0.1f);
                rightArmGoal = new Vector3(calculatedHipsPos.x + handHipDistance,calculatedHipsPos.y + 1f, calculatedHipsPos.z - 0.07f);

                //Pomeranje ramena
                rightShoulderGoalRotation = Quaternion.Euler(
                    rightShoulderStartRotation.eulerAngles.x, rightShoulderStartRotation.eulerAngles.y, rightShoulderStartRotation.eulerAngles.z - 25f);

                leftShoulderGoalRotation = Quaternion.Euler(
                    leftShoulderStartRotation.eulerAngles.x, leftShoulderStartRotation.eulerAngles.y, leftShoulderStartRotation.eulerAngles.z - 25f);

                currentRightShoulderRotation = rightShoulder.localRotation;
                currentLeftShoulderRotation = leftShoulder.localRotation;

                //Racunanje goal rotacije za stopalo
                rightFootGoalRotation = Quaternion.FromToRotation(Vector3.up, laserPointerScript.footAngleArray.First.Value) * rightFootStartRot;
                laserPointerScript.footAngleArray.RemoveFirst();

                rightFoot.rotation = rightFootGoalRotation;
            }

            leftFoot.position = leftFootStartPosition;

            lerp += stepSpeed * Time.deltaTime;

            //Hips lerp
            hips.position = Vector3.Lerp(hipsStartPosition, calculatedHipsPos, HipsCurve.Evaluate(lerp));

            //Pomeranje desne ruke u napred dok se leva noga pomera
            rightArmTarget.position = Vector3.Lerp(rightArmStartPosition, rightArmGoal, lerp);
            leftArmTarget.position = Vector3.Lerp(leftArmStartPosition, leftArmGoal, lerp);

            //Slerp ramena
            leftShoulder.localRotation = Quaternion.Slerp(currentLeftShoulderRotation, leftShoulderGoalRotation, lerp);
            rightShoulder.localRotation = Quaternion.Slerp(currentRightShoulderRotation, rightShoulderGoalRotation, lerp);

            //Slerp stopala
            rightFoot.rotation = Quaternion.Slerp(rightFootStartRotLerp, rightFootGoalRotation, lerp);

            Vector3 currentFoorPos = Vector3.Lerp(rightFootStartPosition, rightFootGoal, FeetCurve.Evaluate(lerp));
            currentFoorPos.y += Mathf.Sin(FeetCurve.Evaluate(lerp) * Mathf.PI) * calculatedStepHeight;
            rightFoot.position = currentFoorPos;


            if (lerp > 1f) 
            {
                laserPointerScript.leftFootTurn = !laserPointerScript.leftFootTurn;
                rightFootStartPosition = currentFoorPos;
                hipsStartPosition = hips.position;

                rightArmStartPosition = rightArmTarget.position;
                leftArmStartPosition = leftArmTarget.position;

                rightFootStartRotLerp = rightFoot.rotation;

                laserPointerScript.FinishStep();

                lerp = 0f;
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
        otherFeetYDiff = 0f; //OVO JE TEMP

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
