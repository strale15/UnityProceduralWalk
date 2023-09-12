using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingScript : MonoBehaviour
{
    [Header("Misc")]
    public AnimationCurve HipsCurve;
    public AnimationCurve FeetCurve;

    [Header("IK Targets")]
    public Transform leftFoot;
    public Transform rightFoot;
    public Transform leftArmTarget;
    public Transform rightArmTarget;

    [Header("Bones")]
    public Transform hips;
    public Transform lowerSpine;
    public Transform middleSpine;
    public Transform upperSpine;

    public Transform leftShoulder;
    public Transform rightShoulder;

    public Transform leftFootBase;
    public Transform rightFootBase;
    public Transform hipsBase;

    [Header("Parametars")]
    public float stepSpeed = 0.2f;
    public float stepHeight = 0.2f;
    public float shoulderAngle = 13f;
    public float handOffsetBack = 0.975f;
    public float handOffsetForward = 0.965f;
    public float lowerSpineForwardAngle = 9f;
    public float middleSpineForwardAngle = 6f;
    public float lowerSpineSidewaysAngle = 9f;
    public float middleSpineSidewaysAngle = 6f;

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
    private float calculatedStepHeight = 0f;
    private float handHipDistance = 0;

    private Vector3 leftFootGoal = new Vector3();
    private Vector3 rightFootGoal = new Vector3();
    private Vector3 calculatedHipsPos = new Vector3();
    private Vector3 leftArmGoal = new Vector3();
    private Vector3 rightArmGoal = new Vector3();
    private Vector3 coef = new Vector3();

    private Quaternion rightShoulderGoalRotation = new Quaternion();
    private Quaternion leftShoulderGoalRotation = new Quaternion();
    private Quaternion currentRightShoulderRotation = new Quaternion();
    private Quaternion currentLeftShoulderRotation = new Quaternion();
    private Quaternion leftFootGoalRotation = new Quaternion();
    private Quaternion rightFootGoalRotation = new Quaternion();
    private Quaternion leftFootStartRot = new Quaternion();
    private Quaternion rightFootStartRot = new Quaternion();

    //Spine
    private Vector3 lowerSpineDefaultRot = new Vector3();
    private Quaternion lowerSpineGoalRot = new Quaternion();
    private Quaternion lowerSpineCurrentRot = new Quaternion();

    private Vector3 middleSpineDefaultRot = new Vector3();
    private Quaternion middleSpineGoalRot = new Quaternion();
    private Quaternion middleSpineCurrentRot = new Quaternion();

    private Vector3 upperSpineDefaultRot = new Vector3();
    private Quaternion upperSpineGoalRot = new Quaternion();
    private Quaternion upperSpineCurrentRot = new Quaternion();
    //-----

    private Quaternion leftFootStartRotLerp = new Quaternion();
    private Quaternion rightFootStartRotLerp = new Quaternion();

    
    void Start()
    {
        laserPointerScript = GameObject.FindObjectOfType<LaserPointer>();

        SetupStartPositions();

        legLength = Vector3.Distance(leftFootBase.position, hipsBase.position);
        handHipDistance = Mathf.Abs(hipsBase.position.x - rightArmTarget.position.x);
    }

    public void SetupStartPositions()
    {
        //Hips
        hipsStartPosition = hips.position;

        //Feet position
        leftFootStartPosition = leftFoot.position;
        rightFootStartPosition = rightFoot.position;

        //Arms position
        leftArmStartPosition = leftArmTarget.position;
        rightArmStartPosition = rightArmTarget.position;

        //Shoulder position
        leftShoulderStartRotation = Quaternion.Euler(leftShoulder.localRotation.eulerAngles);
        rightShoulderStartRotation = Quaternion.Euler(rightShoulder.localRotation.eulerAngles);

        //Spine
        lowerSpineDefaultRot = lowerSpine.eulerAngles;
        lowerSpineCurrentRot = lowerSpine.rotation;

        middleSpineDefaultRot = middleSpine.eulerAngles;
        middleSpineCurrentRot = middleSpine.rotation;

        upperSpineDefaultRot = upperSpine.eulerAngles;
        upperSpineCurrentRot = upperSpine.rotation;

        //Foot rotation
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

                coef = FitCurve(leftFootStartPosition, leftFootGoal);

                //Racunanje goal-a za ruke
                rightArmGoal = new Vector3(calculatedHipsPos.x + handHipDistance, calculatedHipsPos.y + handOffsetForward, calculatedHipsPos.z + 0.1f);
                leftArmGoal = new Vector3(calculatedHipsPos.x - handHipDistance, calculatedHipsPos.y + handOffsetBack , calculatedHipsPos.z - 0.07f);

                //Pomeranje ramena
                rightShoulderGoalRotation = Quaternion.Euler(
                    rightShoulderStartRotation.eulerAngles.x, rightShoulderStartRotation.eulerAngles.y, rightShoulderStartRotation.eulerAngles.z + shoulderAngle);

                leftShoulderGoalRotation = Quaternion.Euler(
                    leftShoulderStartRotation.eulerAngles.x, leftShoulderStartRotation.eulerAngles.y, leftShoulderStartRotation.eulerAngles.z + shoulderAngle);

                currentRightShoulderRotation = rightShoulder.localRotation;
                currentLeftShoulderRotation = leftShoulder.localRotation;

                //Racunanje goal rotacije za stopalo
                leftFootGoalRotation = Quaternion.FromToRotation(Vector3.up, laserPointerScript.footAngleArray.First.Value) * leftFootStartRot;
                laserPointerScript.footAngleArray.RemoveFirst();

                leftFoot.rotation = leftFootGoalRotation;

                //Racunanje za kicmu
                CalculateSpineRotation(leftFootGoal, leftFootStartPosition, rightFootStartPosition, true);
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

            //Slerp kicme
            lowerSpine.rotation = Quaternion.Slerp(lowerSpineCurrentRot, lowerSpineGoalRot, lerp);
            middleSpine.rotation = Quaternion.Slerp(middleSpineCurrentRot, middleSpineGoalRot, lerp);

            Vector3 rot = upperSpine.eulerAngles;
            upperSpine.rotation = Quaternion.Slerp(upperSpineCurrentRot, upperSpineGoalRot, lerp);
            upperSpine.rotation = Quaternion.Euler(rot.x, upperSpine.eulerAngles.y, rot.z);

            //Lerp noge
            Vector3 currentFootPos = Vector3.Lerp(leftFootStartPosition, leftFootGoal, FeetCurve.Evaluate(lerp));
            currentFootPos.y = Quadratic(coef, FeetCurve.Evaluate(lerp));
            leftFoot.position = currentFootPos;


            if (lerp > 1f) 
            {
                laserPointerScript.leftFootTurn = !laserPointerScript.leftFootTurn;
                leftFootStartPosition = currentFootPos;
                hipsStartPosition = hips.position;

                rightArmStartPosition = rightArmTarget.position;
                leftArmStartPosition = leftArmTarget.position;

                leftFootStartRotLerp = leftFoot.rotation;

                lowerSpineCurrentRot = lowerSpine.rotation;
                middleSpineCurrentRot = middleSpine.rotation;
                upperSpineCurrentRot = upperSpine.rotation;

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

                coef = FitCurve(rightFootStartPosition, rightFootGoal);
                //Debug.Log("StarH: " + rightFootStartPosition.y + " EndH: " + rightFootGoal.y + " coef: " + coef + " CalculatedH: " + calculatedStepHeight);

                //Racunanje goal-a za ruke
                leftArmGoal = new Vector3(calculatedHipsPos.x - handHipDistance, calculatedHipsPos.y + handOffsetForward, calculatedHipsPos.z + 0.1f);
                rightArmGoal = new Vector3(calculatedHipsPos.x + handHipDistance,calculatedHipsPos.y + handOffsetBack, calculatedHipsPos.z - 0.07f);

                //Pomeranje ramena
                rightShoulderGoalRotation = Quaternion.Euler(
                    rightShoulderStartRotation.eulerAngles.x, rightShoulderStartRotation.eulerAngles.y, rightShoulderStartRotation.eulerAngles.z - shoulderAngle);

                leftShoulderGoalRotation = Quaternion.Euler(
                    leftShoulderStartRotation.eulerAngles.x, leftShoulderStartRotation.eulerAngles.y, leftShoulderStartRotation.eulerAngles.z - shoulderAngle);

                currentRightShoulderRotation = rightShoulder.localRotation;
                currentLeftShoulderRotation = leftShoulder.localRotation;

                //Racunanje goal rotacije za stopalo
                rightFootGoalRotation = Quaternion.FromToRotation(Vector3.up, laserPointerScript.footAngleArray.First.Value) * rightFootStartRot;
                laserPointerScript.footAngleArray.RemoveFirst();

                rightFoot.rotation = rightFootGoalRotation;

                //Racunanje za kicmu
                CalculateSpineRotation(rightFootGoal, rightFootStartPosition, leftFootStartPosition, false);
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

            //Slerp kicme
            lowerSpine.rotation = Quaternion.Slerp(lowerSpineCurrentRot, lowerSpineGoalRot, lerp);
            middleSpine.rotation = Quaternion.Slerp(middleSpineCurrentRot, middleSpineGoalRot, lerp);

            Vector3 rot = upperSpine.eulerAngles;
            upperSpine.rotation = Quaternion.Slerp(upperSpineCurrentRot, upperSpineGoalRot, lerp);
            upperSpine.rotation = Quaternion.Euler(rot.x, upperSpine.eulerAngles.y, rot.z);

            //Lerp noge
            Vector3 currentFootPos = Vector3.Lerp(rightFootStartPosition, rightFootGoal, FeetCurve.Evaluate(lerp));
            currentFootPos.y = Quadratic(coef, FeetCurve.Evaluate(lerp));
            rightFoot.position = currentFootPos;

            if (lerp > 1f) 
            {
                laserPointerScript.leftFootTurn = !laserPointerScript.leftFootTurn;
                rightFootStartPosition = currentFootPos;
                hipsStartPosition = hips.position;

                rightArmStartPosition = rightArmTarget.position;
                leftArmStartPosition = leftArmTarget.position;

                rightFootStartRotLerp = rightFoot.rotation;

                lowerSpineCurrentRot = lowerSpine.rotation;
                middleSpineCurrentRot = middleSpine.rotation;
                upperSpineCurrentRot = upperSpine.rotation;

                laserPointerScript.FinishStep();

                lerp = 0f;
            }
        }
    }

    private void CalculateSpineRotation(Vector3 stepGoal, Vector3 currentStep, Vector3 otherFootPos, bool isLeftLeg)
    {
        float zeroHeight = 0.05f;
        float bigStep = 0.9f;

        float xRotationLower = 0f;
        float zRotationLower = 0f;

        float xRotationMiddle = 0f;
        float zRotationMiddle = 0f;

        float yRotationUpper = 0f;
        //1 Za korak ka gore - noga ide gore dok druga ostaje dole - rotira se unapred za ugao 

        //2 Korak ka gore - druga noga je vec gore - rotira se unazad za ugao

        //3 Obican korak - mala razdaljina ista visina - nista

        //4 Sirok korak - velika razdaljina ista visina - rotira se unazad za ugao

        //5 Korak ka dole - druga noga ostaje gore prva se spusta - rotira se unapred za ugao i sa strane malo

        //6 Korak ka dole - noga se spusta druga je vec dole - rotira se unazad za ugao i sa strane se vraca uspravno

        //-----------------------------
        float footHeightDiff = stepGoal.y - currentStep.y;
        float otherFootHeightDiff = stepGoal.y - otherFootPos.y;

        if (otherFootHeightDiff > zeroHeight)
        {
            //Climbing up
            xRotationLower = lowerSpineForwardAngle;
            xRotationMiddle = middleSpineForwardAngle;

        } else if (otherFootHeightDiff < -zeroHeight)
        {
            //Climbing down
            xRotationLower = 0f;
            xRotationMiddle = 0f;

        } else if (footHeightDiff <= zeroHeight && footHeightDiff >= -zeroHeight)
        {
            //Normal
            if (Vector3.Distance(stepGoal, otherFootPos) >= bigStep)
            {
                //Big step
                xRotationLower = -lowerSpineForwardAngle*1.2f;
                xRotationMiddle = -middleSpineForwardAngle;
            }
            else
            {
                //Normal step
                xRotationLower = 0f;
                xRotationMiddle = 0f;
            }
        }

        //Rotacija u ramenima
        yRotationUpper = 10f;
        if (isLeftLeg)
        {
            yRotationUpper *= -1;
        }


        lowerSpineGoalRot = Quaternion.Euler(lowerSpineDefaultRot.x + xRotationLower, lowerSpineDefaultRot.y, lowerSpineDefaultRot.z);
        middleSpineGoalRot = Quaternion.Euler(middleSpineDefaultRot.x + xRotationMiddle, middleSpineDefaultRot.y, middleSpineDefaultRot.z);
        upperSpineGoalRot = Quaternion.Euler(upperSpineDefaultRot.x, upperSpineDefaultRot.y + yRotationUpper, upperSpineDefaultRot.z);

    }

    private Vector3 CalculateHipsPosition(Vector3 leftFootPosition, Vector3 rightFootPosition)
    {
        float feetY = 0f;
        if(leftFootPosition.y < rightFootPosition.y)
        {
            rightFootPosition.y = leftFootPosition.y;
            feetY = leftFootPosition.y;
        } else
        {
            leftFootPosition.y = rightFootPosition.y;
            feetY = rightFootPosition.y;
        }

        float feetDist = Vector3.Distance(rightFootPosition, leftFootPosition)/2;
        float hipHeight = legLength * legLength - feetDist * feetDist;

        Vector3 feetMiddlepoint = Vector3.Lerp(leftFootPosition, rightFootPosition, 0.5f); 

        return new Vector3(feetMiddlepoint.x, hipHeight - modellegHeight + (feetY - 0.1154f), feetMiddlepoint.z);
        
    } 
    
    private float CalculateStepHeight(Vector3 goalPosition, Vector3 oldPostion, Vector3 otherFootPostion)
    {
        //float diff = Mathf.Abs(goalPosition.y - oldPostion.y);
        float h = Mathf.Max(goalPosition.y, oldPostion.y) + stepHeight;
        return h;
    }

    private float Quadratic(Vector3 coef, float x)
    {
        return (coef.x * x * x) + (coef.y * x) + coef.z;
    }

    private Vector3 FitCurve(Vector3 startPosition, Vector3 goalPosition)
    {
        float y1 = startPosition.y;
        float y2 = calculatedStepHeight;
        float y3 = goalPosition.y;

        float c = y1;
        float a = -4 * y2 + 2 * y3 + 2 * c;
        float b = y3 - c - a;

        return new Vector3(a, b, c);
    }

    private float Distance2D(Vector3 point)
    {
        Vector3 point2D = new Vector3(point.x, 0, point.z);

        return Vector3.Distance(point2D, new Vector3());
    }
}
