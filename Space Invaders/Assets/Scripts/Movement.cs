using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{
    public float moveSpeed;
    public float rotationSpeed;
    public GameObject cameras;
    public float cameraSwitchRate;
    public Text rocket2Text;
    private float camSwitchTime;
    public RawImage Astro;
    public Text welcome;
    public Text welcomeMessage;
    private bool showWelcomeMessage;

    public GameObject rocket1;
    public Transform rocket1Shot;
    public GameObject rocket2;
    public Transform rocket2Shot;
    public float fireRate;
    private float nextFire;
    private Stopwatch stopwatch;
    private bool canShootRocket2;
    private void Start()
    {
        camSwitchTime = Time.time;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SetActiveCameras();
        stopwatch = Stopwatch.StartNew();
        rocket2Text.color = new Color(1, 0, 0);
        rocket2Text.text = "Master Rocket is AVAILABLE NOW, USE IT";
        canShootRocket2 = true;
        Astro.enabled = true;
        welcome.text = "Welcome to BE in space";
        welcomeMessage.text = "Hello and welcome to BE in space\n your task is to kill and get some score bitch \n right click for master rocket\n HIT ENTER TO BEGIN";
        showWelcomeMessage = true;
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
        if (showWelcomeMessage)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Astro.enabled = false;
                welcome.text = "";
                welcomeMessage.text = "";
                showWelcomeMessage = false;
            }
        }
        else
        {
            AudioSource audioData;
            SwitchActiveCamera();
            if (Input.GetButton("Fire1") && Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;
                Instantiate(rocket1, rocket1Shot.position, rocket1Shot.rotation);
                audioData = GetComponent<AudioSource>();
                audioData.Play(0);
            }
            if (stopwatch.ElapsedMilliseconds < 7 * 1000 && canShootRocket2)
            {
                if (Input.GetButton("Fire2") && Time.time > nextFire)
                {
                    nextFire = Time.time + fireRate;
                    Instantiate(rocket2, rocket2Shot.position, rocket2Shot.rotation);
                    audioData = GetComponent<AudioSource>();
                    audioData.Play(0);
                }
            }
            else if (stopwatch.ElapsedMilliseconds > 10 * 1000 && !canShootRocket2)
            {
                stopwatch = Stopwatch.StartNew();
                rocket2Text.color = new Color(1, 0, 0);
                rocket2Text.text = "Master Rocket is AVAILABLE NOW, USE IT";
                canShootRocket2 = true;
            }
            else
            {
                if (canShootRocket2) { stopwatch = Stopwatch.StartNew(); }
                rocket2Text.color = new Color(0.67f, 0.67f, 0.19f);
                rocket2Text.text = "Master Rocket is not available now, wait to reload rockets!";
                canShootRocket2 = false;
            }
        }
    }
    private void FixedUpdate()
    {
        if (showWelcomeMessage)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Astro.enabled = false;
                welcome.text = "";
                welcomeMessage.text = "";
                showWelcomeMessage = false;
            }
        }
        else
        {
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            if (rigidbody == null)
            {
                UnityEngine.Debug.LogError(gameObject.name + " (" + typeof(Movement).Name + "): No Rigidbody component was found!");
                return;
            }

            HandleMovement(rigidbody);
            HandleRotation(rigidbody);
            rigidbody.freezeRotation = true;
        }
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
        if (cameras == null) { UnityEngine.Debug.LogError(typeof(Movement).Name + ": Start() Function was initiated with null"); return; }
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
