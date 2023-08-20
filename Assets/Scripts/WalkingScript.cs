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
    private Vector3 leftFootEndPosition;
    private Vector3 rightFootEndPosition;
    private Vector3 rightFootGoal;
    private Vector3 hipsStartPosition;
    private float legLength;
    private float feetDistance;
    private const float modellegHeight = 0.7160985f;
    private float lerp = 0;

    [SerializeField]
    private float hipHeight;

    [SerializeField]
    private float stepSpeed = 0.2f;

    [SerializeField]
    private float stepHegiht = 0.2f;

    private Vector3 hipPosition;
    void Start()
    {
        leftFootStartPosition = leftFoot.position;
        rightFootStartPosition = rightFoot.position;
        hipsStartPosition = hips.position;


        //rightFootGoal = new Vector3(0.123000003f,0.131999999f,1.205f);
        rightFootGoal = new Vector3(0.123000003f, 0.612999976f, 0.779999971f);

        legLength = Vector3.Distance(leftFootBase.position, hipsBase.position);
        feetDistance = Vector3.Distance(rightFootBase.position, leftFootBase.position)/2;

        hipHeight = legLength * legLength - feetDistance * feetDistance;
        Vector3 feetMiddlepoint = Vector3.Lerp(leftFootBase.position, rightFootBase.position, 0.5f); //Ovo nece raditi ako se stopala na razictim visinama

        hipPosition = new Vector3(feetMiddlepoint.x, modellegHeight - hipHeight, feetMiddlepoint.z);
        //hips.position = hipPosition;
    }

    void Update()
    {
        leftFootEndPosition = leftFootStartPosition;
        rightFootEndPosition = rightFootStartPosition;

        leftFoot.position = leftFootEndPosition;
        //rightFoot.position = rightFootEndPosition;

        hips.position = Vector3.Lerp(hipsStartPosition, CalculateHipsPosition(leftFoot.position, rightFootGoal), lerp);
        Vector3 rightFoorPos;
        rightFoorPos = Vector3.Lerp(rightFootStartPosition, rightFootGoal, lerp);
        rightFoorPos.y += Mathf.Sin(lerp * Mathf.PI) * stepHegiht;
        rightFoot.position = rightFoorPos;

        lerp += stepSpeed * Time.deltaTime;
        if (lerp > 1f) { lerp = 1f; }
    }

    private Vector3 CalculateHipsPosition(Vector3 leftFootPosition, Vector3 rightFootPoition)
    {
        float feetDist = Vector3.Distance(rightFootPoition, leftFootPosition)/2;
        float hipHeight = legLength * legLength - feetDist * feetDist;
        Vector3 feetMiddlepoint = Vector3.Lerp(leftFootPosition, rightFootPoition, 0.5f); //Ovo nece raditi ako se stopala na razictim visinama

        //Debug.Log(hipHeight);
        return new Vector3(feetMiddlepoint.x, hipHeight - modellegHeight, feetMiddlepoint.z);
        
    } 
}
