using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float flySpeed = 5f;
    public CharacterController characterController;

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        move.y = 0f;

        characterController.Move(move * Time.deltaTime * moveSpeed);

        bool spacePressed = Input.GetKey(KeyCode.Space);
        bool shiftPRessed = Input.GetKey(KeyCode.LeftShift);

        if(spacePressed)
        {
            characterController.Move(Vector3.up * Time.deltaTime * flySpeed);
        }
        if(shiftPRessed)
        {
            characterController.Move(-Vector3.up * Time.deltaTime * flySpeed);
        }
    }
}
