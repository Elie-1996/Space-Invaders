using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed;
    public float rotationSpeed;
    public GameObject cameras;
    public float cameraSwitchRate;

    private float camSwitchTime;

    public GameObject rocket1;
    public Transform rocket1Shot;
    public GameObject rocket2;
    public Transform rocket2Shot;
    public float fireRate;
    private float nextFire;
    private void Start()
    {
        camSwitchTime = Time.time;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SetActiveCameras();
    }

    private void Update()
    {
        SwitchActiveCamera();
    }

    private void SwitchActiveCamera()
    {
        if (Input.GetKey(KeyCode.Tab) == false) return;
        if (Time.time <= camSwitchTime) return;

        camSwitchTime = Time.time + cameraSwitchRate;
        PlayerGameObject.Tags.cameras.Next();
        SetActiveCameras();
    }

    private void Update()
    {
        if(Input.GetButton("Fire1") && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            Instantiate(rocket1, rocket1Shot.position, rocket1Shot.rotation);
        }
        if (Input.GetButton("Fire2") && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            Instantiate(rocket2, rocket2Shot.position, rocket2Shot.rotation);
        }
    }
    private void FixedUpdate()
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        if (rigidbody == null)
        {
            Debug.LogError(gameObject.name + " (" + typeof(Movement).Name + "): No Rigidbody component was found!");
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

    private void SetActiveCameras()
    {
        if (cameras == null) { Debug.Log(typeof(Movement).Name + ": Start() Function was initiated with null"); return; }
        foreach (Transform child in cameras.transform)
        {
            child.gameObject.SetActive(false);

            if (child.gameObject.tag == PlayerGameObject.Tags.cameras.GetValue())
            {
                child.gameObject.SetActive(true);
            }
        }
    }
}
