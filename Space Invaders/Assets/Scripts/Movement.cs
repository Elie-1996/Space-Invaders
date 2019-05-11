using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed;
    public float upRotationSpeed;
    public float roundRotationSpeed;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        if (rigidbody == null)
        {
            Debug.LogError("Movement.cs: No Rigidbody component was found!");
            return;
        }

        HandleMovement(rigidbody);
        HandleRotation(rigidbody);
    }

    private void HandleMovement(Rigidbody rigidbody)
    {
        float realSpeed = moveSpeed * Utils.speedScale;
        float verticalDirection = Input.GetAxis("Vertical");
        float horizontalDirection = Input.GetAxis("Horizontal");
        float timePassed = Time.deltaTime;
        float distance = timePassed * realSpeed;

        Vector3 moveAmount = distance * (verticalDirection * rigidbody.transform.forward + horizontalDirection * rigidbody.transform.right);

        rigidbody.velocity = moveAmount;

    }

    private void HandleRotation(Rigidbody rigidbody)
    {
        float upRotationInput = Input.GetAxis("Mouse X");
        float roundRotationInput = Input.GetAxis("Mouse Y");

        Vector3 rightRotation = rigidbody.transform.right;
        Vector3 upRotation = rigidbody.transform.up;
        Vector3 rotateAmount = roundRotationInput * roundRotationSpeed * rightRotation + upRotationInput * upRotationSpeed * upRotation;
        rigidbody.transform.Rotate(rotateAmount);
    }
}
