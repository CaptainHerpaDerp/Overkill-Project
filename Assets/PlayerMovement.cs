using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float movementSpeed;

    [Header("Ground Checking")]
    public float playerHeight;
    public LayerMask groundMask;

    public Transform orientation;

    float horizontalInput, verticalInput;

    Vector3 moveDirection;


    void Update()
    {
        GetInput();

        LimitSpeed();

        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        MovePlayer(moveDirection);
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    private void MovePlayer(Vector3 direction)
    {
        transform.position += direction.normalized * movementSpeed * Time.deltaTime;
    }

    private void LimitSpeed()
    {
        Vector3 flatVelocity = new Vector3(moveDirection.x, 0, moveDirection.z);

        if (flatVelocity.magnitude > movementSpeed)
        {
            moveDirection = flatVelocity.normalized * movementSpeed;
        }
    }
}
