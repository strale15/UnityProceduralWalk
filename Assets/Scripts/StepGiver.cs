using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepGiver : MonoBehaviour
{
    public float sensX = 400f;
    public float sensY = 400f;
    public float moveSpeed = 8f;
    public float flySpeed = 5f;
    public CharacterController characterController;

    private float xRotation;
    private float yRotation;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Camera look
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensX;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);

        //Camera move
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
