using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Movement : NetworkBehaviour
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
    private float elapsedTime;
    private bool canShootRocket2;
    private int masterRocketsCount;
    

    public override void OnStartAuthority()
    {
        Start();
    }

    private void Start()
    {
        if (hasAuthority == false) return;
        loadUI();
        elapsedTime = 0.0f;
        camSwitchTime = Time.time;
        SetActiveCameras();
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

    private void Update()
    {
        if (hasAuthority == false) return;
        if (showWelcomeMessage)
        {
            WelcomeMessage();
        }
        else
        {
            HandleSwitchingActiveCamera();
            HandleShooting();
        }
    }

    private void FixedUpdate()
    {
        if (hasAuthority == false) return;
        if (showWelcomeMessage == true) return;
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        if (rigidbody == null)
        {
            Debug.LogError(gameObject.name + " (" + typeof(Movement).Name + "): No Rigidbody component was found!");
            return;
        }

        rigidbody.freezeRotation = true;
        HandleMovement(rigidbody);
        HandleRotation(rigidbody);
    }

    private void HandleMovement(Rigidbody rigidbody)
    {
        float verticalDirection = Input.GetAxis("Vertical");
        float horizontalDirection = Input.GetAxis("Horizontal");

        Vector3 moveAmount = moveSpeed * (verticalDirection * rigidbody.transform.forward + horizontalDirection * rigidbody.transform.right);
        rigidbody.velocity = moveAmount;

        // ask the server to handle this unit's movement as well.
        CmdHandleMovement(moveAmount, transform.position);
    }

    [Command]
    private void CmdHandleMovement(Vector3 newMoveAmount, Vector3 newPosition)
    {
        HandleMovementHelper(newMoveAmount, newPosition);
        RpcHandleMovement(newMoveAmount, newPosition);
    }

    [ClientRpc]
    private void RpcHandleMovement(Vector3 newMoveAmount, Vector3 newPosition)
    {
        if (hasAuthority == true) return;
        HandleMovementHelper(newMoveAmount, newPosition);
    }

    private void HandleMovementHelper(Vector3 newMoveAmount, Vector3 newPosition)
    {
        transform.position = newPosition;

        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = newMoveAmount;
    }


    private void HandleRotation(Rigidbody rigidbody)
    {
        float upRotationInput = Input.GetAxis("Mouse Y");
        float roundRotationInput = Input.GetAxis("Mouse X");

        float xRotation = upRotationInput * rotationSpeed;
        float yRotation = roundRotationInput * rotationSpeed;
        rigidbody.transform.Rotate(xRotation, yRotation, 0.0f);
        
        CmdHandleRotation(rigidbody.transform.rotation);
    }

    [Command]
    private void CmdHandleRotation(Quaternion newRotation)
    {
        HandleRotationHelper(newRotation);
        RpcHandleRotation(newRotation);
    }

    [ClientRpc]
    private void RpcHandleRotation(Quaternion newRotation)
    {
        HandleRotationHelper(newRotation);
    }

    private void HandleRotationHelper(Quaternion newRotation)
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.rotation = newRotation;
    }


    private void HandleShooting()
    {
        AudioSource audioData;
        elapsedTime += Time.deltaTime;
        if (Input.GetButton("Fire1") && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            CmdSpawnShot(rocket1, rocket1Shot.position, rocket1Shot.rotation);
            audioData = GetComponent<AudioSource>();
            audioData.Play(0);
        }
        if (elapsedTime < 7 && canShootRocket2 && masterRocketsCount != 0)
        {
            if (Input.GetButton("Fire2") && Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;
                CmdSpawnShot(rocket2, rocket2Shot.position, rocket2Shot.rotation);
                audioData = GetComponent<AudioSource>();
                audioData.Play(0);
                turnOffRocketsImage();
                masterRocketsCount--;
            }
        }
        else if (elapsedTime > 10 && !canShootRocket2)
        {
            elapsedTime = 0.0f;
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
            if (canShootRocket2) { elapsedTime = 0.0f; }
            rocket2Text.color = new Color(0.67f, 0.67f, 0.19f);
            rocket2Text.text = "";
            if (_masterRocket1 == null) { Debug.Log("_masterRocket1 is null"); }
            if (_masterRocket1.GetComponent<RawImage>() == null) { Debug.Log("_masterRocket1.RawImage is null"); }
            _masterRocket1.GetComponent<RawImage>().enabled = false;
            _masterRocket2.GetComponent<RawImage>().enabled = false;
            _masterRocket3.GetComponent<RawImage>().enabled = false;
            canShootRocket2 = false;
        }
    }

    [Command]
    private void CmdSpawnShot(GameObject shotObject, Vector3 startingPostion, Quaternion startingRotation)
    {
        GameObject shot = Instantiate(shotObject, startingPostion, startingRotation);
        NetworkServer.Spawn(shot);
    }

    private void SetActiveCameras()
    {
        if (cameras == null) { Debug.LogError(typeof(Movement).Name + ": Start() Function was initiated with null"); return; }
        foreach (Transform child in cameras.transform)
        {
            child.gameObject.SetActive(false);

            if (child.gameObject.tag == PlayerGameObject.Tags.cameras.GetValue())
            {
                child.gameObject.SetActive(true);
            }
        }
    }

    private void HandleSwitchingActiveCamera()
    {
        if (Input.GetKey(KeyCode.Tab) == false) return;
        if (Time.time <= camSwitchTime) return;

        camSwitchTime = Time.time + cameraSwitchRate;
        PlayerGameObject.Tags.cameras.Next();
        SetActiveCameras();
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

    private void loadUI()
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

    private void WelcomeMessage()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            _Astro.GetComponent<RawImage>().enabled = false;
            _welcome.GetComponent<Text>().text = "";
            _welcomeMessage.GetComponent<Text>().text = "";
            showWelcomeMessage = false;
        }
    }
}
