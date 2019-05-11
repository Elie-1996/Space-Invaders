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
            Debug.LogError(gameObject.name + " (Movement.cs): No Rigidbody component was found!");
            return;
        }
        float timePassed = Time.deltaTime;

        HandleMovement(rigidbody, timePassed);
        HandleRotation(rigidbody, timePassed);
    }

    private void HandleMovement(Rigidbody rigidbody, float time)
    {
        float realSpeed = moveSpeed * Utils.speedScale;
        float verticalDirection = Input.GetAxis("Vertical");
        float horizontalDirection = Input.GetAxis("Horizontal");
        float distance = time * realSpeed;

        Vector3 moveAmount = distance * (verticalDirection * rigidbody.transform.forward + horizontalDirection * rigidbody.transform.right);

        rigidbody.velocity = moveAmount;

    }

    private void HandleRotation(Rigidbody rigidbody, float time)
    {
        float upRotationInput = Input.GetAxis("Mouse Y");
        float roundRotationInput = Input.GetAxis("Mouse X");

        Vector3 upRotation = rigidbody.transform.right;
        Vector3 rightRotation = rigidbody.transform.up;
        Vector3 rotateAmount = roundRotationInput * roundRotationSpeed * rightRotation + upRotationInput * upRotationSpeed * upRotation;
        rotateAmount *= time * Utils.speedScale;
        rigidbody.transform.Rotate(rotateAmount);
    }
}
