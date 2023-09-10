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

    public Transform lumbarSpine;

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
    private Vector3 coef = new Vector3();
    private Quaternion rightShoulderGoalRotation = new Quaternion();
    private Quaternion leftShoulderGoalRotation = new Quaternion();
    private Quaternion currentRightShoulderRotation = new Quaternion();
    private Quaternion currentLeftShoulderRotation = new Quaternion();
    private Quaternion leftFootGoalRotation = new Quaternion();
    private Quaternion rightFootGoalRotation = new Quaternion();
    private Quaternion leftFootStartRot = new Quaternion();
    private Quaternion rightFootStartRot = new Quaternion();
    private Vector3 lumbarSpineDefaultRot = new Vector3();
    private Quaternion lumbarSpineGoalRot = new Quaternion();
    private Quaternion lumbarSpineCurrentRot = new Quaternion();

    private Quaternion leftFootStartRotLerp = new Quaternion();
    private Quaternion rightFootStartRotLerp = new Quaternion();
    private float handHipDistance = 0;

    public float stepSpeed = 0.2f;
    public float stepHeight = 0.2f;
    private float calculatedStepHeight = 0f;
    public float shoulderAngle = 13f;

    
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

        lumbarSpineDefaultRot = lumbarSpine.eulerAngles;
        lumbarSpineCurrentRot = lumbarSpine.rotation;

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
                rightArmGoal = new Vector3(calculatedHipsPos.x + handHipDistance, calculatedHipsPos.y + 1f, calculatedHipsPos.z + 0.1f);
                leftArmGoal = new Vector3(calculatedHipsPos.x - handHipDistance, calculatedHipsPos.y + 1f , calculatedHipsPos.z - 0.07f);

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
                CalculateSpineRotation(leftFootGoal, rightFootStartPosition, true);
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
            lumbarSpine.rotation = Quaternion.Slerp(lumbarSpineCurrentRot, lumbarSpineGoalRot, lerp);

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

                lumbarSpineCurrentRot = lumbarSpine.rotation;

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
                leftArmGoal = new Vector3(calculatedHipsPos.x - handHipDistance, calculatedHipsPos.y + 1f, calculatedHipsPos.z + 0.1f);
                rightArmGoal = new Vector3(calculatedHipsPos.x + handHipDistance,calculatedHipsPos.y + 1f, calculatedHipsPos.z - 0.07f);

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
                CalculateSpineRotation(rightFootGoal, leftFootStartPosition, false);
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
            lumbarSpine.rotation = Quaternion.Slerp(lumbarSpineCurrentRot, lumbarSpineGoalRot, lerp);

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

                lumbarSpineCurrentRot = lumbarSpine.rotation;

                laserPointerScript.FinishStep();

                lerp = 0f;
            }
        }
    }

    private void CalculateSpineRotation(Vector3 stepGoal, Vector3 currentStep, bool isLeftLeg)
    {
        float yRotation = 15f;

        if (isLeftLeg)
        {
            yRotation *= -1f;
        }

        if ((stepGoal.y - currentStep.y) < 0)
        {
            //zRotation = 0f;
        }

        //zRotation *= (stepGoal.y - currentStep.y) / 0.6f;

        lumbarSpineGoalRot = Quaternion.Euler(lumbarSpineDefaultRot.x, lumbarSpineDefaultRot.y + yRotation, lumbarSpineDefaultRot.z);

        //Debug.Log(stepGoal.y + " Current " + currentStep.y + " ?:" + lumbarSpineDefaultRot.x);
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

        //Debug.Log("Feet dist: " + feetDist + " LegLength: " + legLength + " HipHeight: " + hipHeight + "FeetY: " + feetY);

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
