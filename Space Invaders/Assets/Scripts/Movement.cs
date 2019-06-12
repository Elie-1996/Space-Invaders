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
    public Canvas canvas;
    public RawImage AstroPrefab;
    public RawImage masterRocket1Prefab;
    public RawImage masterRocket2Prefab;
    public RawImage masterRocket3Prefab;
    public Text welcomePrefab;
    public Text welcomeMessagePrefab;

    private GameObject _Astro;
    private GameObject _masterRocket1;
    private GameObject _masterRocket2;
    private GameObject _masterRocket3;
    private GameObject _welcome;
    private GameObject _welcomeMessage;
    private bool showWelcomeMessage;

    public GameObject rocket1;
    public Transform rocket1Shot;
    public GameObject rocket2;
    public Transform rocket2Shot;
    public float fireRate;
    private float nextFire;
    private Stopwatch stopwatch;
    private bool canShootRocket2;
    private int masterRocketsCount;

    void loadUI()
    {
        GameObject canvasObject = Instantiate(canvas).gameObject;
        RectTransform rTransform = canvasObject.GetComponent<RectTransform>();
        _Astro = Instantiate(AstroPrefab.gameObject);
        _Astro.transform.SetParent(rTransform, false);

        _masterRocket1 = Instantiate(masterRocket1Prefab.gameObject);
        _masterRocket1.transform.SetParent(rTransform, false);

        _masterRocket2 = Instantiate(masterRocket2Prefab.gameObject);
        _masterRocket2.transform.SetParent(rTransform, false);

        _masterRocket3 = Instantiate(masterRocket3Prefab.gameObject);
        _masterRocket3.transform.SetParent(rTransform, false);

        _welcome = Instantiate(welcomePrefab.gameObject);
        _welcome.transform.SetParent(rTransform, false);

        _welcomeMessage = Instantiate(welcomeMessagePrefab.gameObject);
        _welcomeMessage.transform.SetParent(rTransform, false);
    }

    private void Start()
    {
        loadUI();
        camSwitchTime = Time.time;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SetActiveCameras();
        stopwatch = Stopwatch.StartNew();
        rocket2Text.color = new Color(1, 0, 0);
        rocket2Text.text = "";
        masterRocket1Prefab.enabled = true;
        masterRocket2Prefab.enabled = true;
        masterRocket3Prefab.enabled = true;
        masterRocketsCount = 3;
        canShootRocket2 = true;
        _Astro.GetComponent<RawImage>().enabled = true;
        _welcome.GetComponent<Text>().text = "Welcome to BE in space";
        _welcomeMessage.GetComponent<Text>().text = "Hello and welcome to BE in space\n your task is to kill and get some score bitch \n right click for master rocket\n HIT ENTER TO BEGIN";
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
                _Astro.GetComponent<RawImage>().enabled = false;
                _welcome.GetComponent<Text>().text = "";
                _welcomeMessage.GetComponent<Text>().text = "";
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
            if (stopwatch.ElapsedMilliseconds < 7 * 1000 && canShootRocket2 && masterRocketsCount !=0)
            {
                if (Input.GetButton("Fire2") && Time.time > nextFire)
                {
                    nextFire = Time.time + fireRate;
                    Instantiate(rocket2, rocket2Shot.position, rocket2Shot.rotation);
                    audioData = GetComponent<AudioSource>();
                    audioData.Play(0);
                    turnOffRocketsImage();
                    masterRocketsCount--;
                }
            }
            else if (stopwatch.ElapsedMilliseconds > 10 * 1000 && !canShootRocket2)
            {
                stopwatch = Stopwatch.StartNew();
                rocket2Text.color = new Color(1, 0, 0);
                rocket2Text.text = "";
                _masterRocket1.GetComponent<RawImage>().enabled = true;
                _masterRocket2.GetComponent<RawImage>().enabled = true;
                _masterRocket3.GetComponent<RawImage>().enabled = true;
                masterRocketsCount = 3;
                canShootRocket2 = true;
            }
            else
            {
                if (canShootRocket2) { stopwatch = Stopwatch.StartNew(); }
                rocket2Text.color = new Color(0.67f, 0.67f, 0.19f);
                rocket2Text.text = "";
                _masterRocket1.GetComponent<RawImage>().enabled = false;
                _masterRocket2.GetComponent<RawImage>().enabled = false;
                _masterRocket3.GetComponent<RawImage>().enabled = false;
                canShootRocket2 = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!showWelcomeMessage)
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
    private void turnOffRocketsImage()
    {
        if (masterRocketsCount == 3)
        {
            _masterRocket3.GetComponent<RawImage>().enabled = false;
            return;
        }
        else if (masterRocketsCount == 2)
        {
            _masterRocket2.GetComponent<RawImage>().enabled = false;
            return;
        }
        else
            _masterRocket1.GetComponent<RawImage>().enabled = false;
    }
}
