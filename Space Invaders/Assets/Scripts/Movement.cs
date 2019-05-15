using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed;
    public float rotationSpeed;

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
            Debug.LogError(gameObject.name + " (Movement.cs): No Rigidbody component was found!");
            return;
        }

        HandleMovement(rigidbody);
        HandleRotation(rigidbody);
        rigidbody.freezeRotation = true;
    }

    private void HandleMovement(Rigidbody rigidbody)
    {
        float verticalDirection = Input.GetAxis("Vertical");
        float horizontalDirection = Input.GetAxis("Horizontal");

        Vector3 moveAmount = moveSpeed * (verticalDirection * rigidbody.transform.forward + horizontalDirection * rigidbody.transform.right);

        rigidbody.velocity = moveAmount;

    }

    private void HandleRotation(Rigidbody rigidbody)
    {
        float upRotationInput = Input.GetAxis("Mouse Y");
        float roundRotationInput = Input.GetAxis("Mouse X");
        
        rigidbody.transform.Rotate(upRotationInput * rotationSpeed, roundRotationInput * rotationSpeed, 0.0f);
    }
}
