using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForeArmRotationFix : MonoBehaviour
{
    public Transform leftForeArm;
    public Transform rightForeArm;
    public Transform leftArmTarget;
    public Transform rightArmTarget;

    void Start()
    {
        
    }

    void Update()
    {
        Vector3 leftArmRoation = leftArmTarget.eulerAngles;
        Vector3 rightArmRoation = rightArmTarget.eulerAngles;
        Vector3 leftForearmRotation = leftForeArm.eulerAngles;
        Vector3 rightForearmRotation = rightForeArm.eulerAngles;

        leftArmTarget.localRotation = Quaternion.Euler(leftArmRoation.x, leftArmRoation.y ,leftForearmRotation.z);
        rightArmTarget.localRotation = Quaternion.Euler(rightArmRoation.x, rightArmRoation.y ,rightForearmRotation.z);
    }
}
