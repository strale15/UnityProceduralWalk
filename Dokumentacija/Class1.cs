using System;

public class Class1
{
	public Class1()
	{
        private void CalculateSpineRotation(Vector3 stepGoal, Vector3 currentStep, Vector3 otherFootPos, bool isLeftLeg)
        {
            float zeroHeight = 0.05f;
            float bigStep = 0.9f;
            float xRotationLower = 0f;
            float xRotationMiddle = 0f;
            float yRotationUpper = 10f;
            float footHeightDiff = stepGoal.y - currentStep.y;
            float otherFootHeightDiff = stepGoal.y - otherFootPos.y;

            if (otherFootHeightDiff > zeroHeight)
            {
                //Korak navise
                xRotationLower = lowerSpineForwardAngle;
                xRotationMiddle = middleSpineForwardAngle;

            } else if (otherFootHeightDiff < -zeroHeight)
            {
                //Korak nanize
                xRotationLower = 0f;
                xRotationMiddle = 0f;

            } else if (footHeightDiff <= zeroHeight && footHeightDiff >= -zeroHeight)
            {
                if (Vector3.Distance(stepGoal, otherFootPos) >= bigStep)
                {
                    //Digacak iskorak
                    xRotationLower = -lowerSpineForwardAngle*1.2f;
                    xRotationMiddle = -middleSpineForwardAngle;
                } else
                {
                    //Normalan korak
                    xRotationLower = 0f;
                    xRotationMiddle = 0f;
                }
            }

            if (isLeftLeg)
            {
                yRotationUpper *= -1;
            }

            lowerSpineGoalRot = Quaternion.Euler(
                lowerSpineDefaultRot.x + xRotationLower, 
                lowerSpineDefaultRot.y,
                lowerSpineDefaultRot.z);

            middleSpineGoalRot = Quaternion.Euler(
                middleSpineDefaultRot.x + xRotationMiddle,
                middleSpineDefaultRot.y,
                middleSpineDefaultRot.z);

            upperSpineGoalRot = Quaternion.Euler(
                upperSpineDefaultRot.x,
                upperSpineDefaultRot.y + yRotationUpper, 
                upperSpineDefaultRot.z);
        }
	}
}
