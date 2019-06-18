﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameController : NetworkBehaviour
{
    public GameObject gameBackground;
    public GameObject Planets;
    public GameObject AsteroidPrefab;
    public GameObject EnemyPrefab;
    public float asteroidSpawnWaitSeconds;
    public float enemyIntervalSpawnWaitSeconds;
    public Canvas canvas;
    public Text scoreTextPrefab;
    public Text gameOverTextPrefab;
    public Text RestartTextPrefab;

    private GameObject _scoreText;
    private GameObject _gameOverText;
    private GameObject _RestartText;

    private GameObject circule;
    private bool gameOver;
    private bool restart;

    [SyncVar(hook = "updateScoreGUI")]
    private int score;

    private int maxAllowedLevels;
    private bool shouldAdvanceLevel;
    private int level;
    private bool escape;

    private bool extraRocket;
    private bool speedGift;

    private GameObject AsteroidsHolder;

    [SyncVar]
    private Vector3 _AsteroidDirection;

    public Vector3 AsteroidDirection { get { return _AsteroidDirection; } }
    private Vector3 startSpawn;

    void loadGUI()
    {
        GameObject canvasObject = Instantiate(canvas).gameObject;
        RectTransform rTransform = canvasObject.GetComponent<RectTransform>();

        _scoreText = Instantiate(scoreTextPrefab.gameObject);
        _scoreText.transform.SetParent(rTransform, false);

        _gameOverText = Instantiate(gameOverTextPrefab.gameObject);
        _gameOverText.transform.SetParent(rTransform, false);

        _RestartText = Instantiate(RestartTextPrefab.gameObject);
        _RestartText.transform.SetParent(rTransform, false);

        if (isServer) score = 0;
    }


    // Start is called before the first frame update
    void Start()
    {
        if (Planets == null || gameBackground == null || AsteroidPrefab == null) throw new MissingReferenceException();
        // specific to the LOCAL PLAYER
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        loadGUI();

        // specific to the LOCAL PLAYER (For Now)
        updateScoreGUI(score);
        gameOver = false;
        restart = false;
        escape = true;
        _gameOverText.GetComponent<Text>().text = "";
        _RestartText.GetComponent<Text>().text = "";
        if (isServer == false)
        {
            AsteroidsHolder = new GameObject("Asteroid Holder");
            CollectAsteroids();
        }

        // everything below is specific to the SERVER
        if (isServer == false) return;
        AsteroidsHolder = new GameObject("Asteroid Holder");

        SpawnAsteroids();


        // okay, let's initiate these planets locations as the SERVER
        InitiatePlanetLocationsOnServerAndUpdateForClients();
        maxAllowedLevels = Planets.transform.childCount;
        level = 1;
        shouldAdvanceLevel = false;
        StartCoroutine (LevelSystem());
    }

    private void SpawnAsteroids()
    {
        (_AsteroidDirection, startSpawn) = decideAsteroidDirection();
        StartCoroutine(SpawnAsteroidsHelper());
    }

    private void CollectAsteroids()
    {
        GameObject[] spawnedAsteroids = GameObject.FindGameObjectsWithTag(Utils.TagAsteroid);
        foreach (GameObject o in spawnedAsteroids)
        {
            o.transform.parent = AsteroidsHolder.transform;
        }
    }

    private (Vector3, Vector3) decideAsteroidDirection()
    {
        float radius = Utils.getBackgroundRadius(gameBackground);
        Vector3 startSpawn = Utils.getRandomDirection() * radius;
        while (startSpawn.x == 35.0f && startSpawn.y == 35.0f && startSpawn.z == 35.0f)
            startSpawn = Utils.getRandomDirection() * radius;
        return (
            (new Vector3(35.0f, 35.0f, 35.0f) - startSpawn).normalized,
             startSpawn);
    }

    private void InitiatePlanetLocationsOnServerAndUpdateForClients()
    {
        float radius = Utils.getGameBoundaryRadius(gameBackground) + 25.0f;
        foreach (Transform child in Planets.transform)
        {
            GameObject planet = child.gameObject;
            // generate random location
            Vector3 direction = Utils.getRandomDirection();

            // assign random location
            planet.transform.position = direction * radius;
            planet.SetActive(false);
            // let's update ALL of the clients, to change their planet locations according to the SERVER.
            RpcUpdatePlanetLocationsOnClient(planet.GetComponent<NetworkIdentity>(), planet.transform.position, planet.activeSelf);
        }
    }

    [ClientRpc]
    private void RpcUpdatePlanetLocationsOnClient(NetworkIdentity planetIdentity, Vector3 planetPosition, bool planetIsActive)
    {
        if (planetIdentity == null) throw new MissingComponentException("Fatal Error: No NetworkIdentity provided for the planet");
        planetIdentity.gameObject.transform.position = planetPosition;
        planetIdentity.gameObject.SetActive(planetIsActive);

    }

    IEnumerator LevelSystem()
    {
        if (isServer == false) throw new Utils.AttemptedUnauthorizedAccessLevelSystemException("hasAuthority = " + hasAuthority + ", isLocalPlayer = " + isLocalPlayer + ", isServer = " + isServer + ".");
        while (level < maxAllowedLevels)
        {
            shouldAdvanceLevel = false;
            SpawnPlanets();
            StartCoroutine (SpawnLevel(level));
            yield return new WaitUntil(()=> shouldAdvanceLevel == true);
            ++level;
        }
        // Win Game! => Define Behaviour!
    }

    IEnumerator SpawnLevel(int level)
    {
        int enemisAlive = 0;
        // spawn some enemies
        foreach (Transform child in Planets.transform)
        {
            if (child.gameObject.activeSelf == false) continue;
            int enemiesToAdd = level * 3;
            enemisAlive += enemiesToAdd;
            StartCoroutine (SpawnEnemiesFromPlanet(child, enemiesToAdd));
        }
        yield return new WaitUntil(() => enemisAlive == 0);
        shouldAdvanceLevel = true;
    }
    
    IEnumerator SpawnEnemiesFromPlanet(Transform planet, int amount)
    {
        for (int i = 0; i < amount; ++i)
        {
            //TODO: Avoiding to actually instantiate until movement is decided
            //Instantiate(EnemyPrefab, planet.position, Quaternion.identity);
            yield return new WaitForSeconds(enemyIntervalSpawnWaitSeconds);
        }
    }

    void SpawnPlanets()
    {
        int currentLevel = level;
        if (currentLevel <= 0 || currentLevel > maxAllowedLevels) return;
        foreach (Transform planet in Planets.transform)
        {
            if (currentLevel == 0) break;
            planet.gameObject.SetActive(true);
            currentLevel--;
            RpcUpdatePlanetLocationsOnClient(planet.gameObject.GetComponent<NetworkIdentity>(), planet.gameObject.transform.position, planet.gameObject.activeSelf);
        }
    }

    private void Update()
    {
        if (restart)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
        if (escape)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                escape = false;
            }
        }
        else {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                escape = true;
            }
        }
    }

    IEnumerator SpawnAsteroidsHelper()
    {
        float distance = 12.0f;
        float radius = Utils.getBackgroundRadius(gameBackground);
        Vector3 direction = _AsteroidDirection;

        // spawn the first 800
        for (int i = 0; i < 750; i += 5)
        {
            Vector3 inc = new Vector3(direction.x * i, direction.y * i, direction.z * i);
            if (Random.value <= 0.5)
                CmdInstantiateAsteroidOnServerThenUpdateClient(startSpawn + inc, Quaternion.identity);

            CmdInstantiateAsteroidOnServerThenUpdateClient(startSpawn + inc + Random.insideUnitSphere * distance, Quaternion.identity);
            CmdInstantiateAsteroidOnServerThenUpdateClient(startSpawn + inc + Random.insideUnitSphere * distance, Quaternion.identity);
            CmdInstantiateAsteroidOnServerThenUpdateClient(startSpawn + inc + Random.insideUnitSphere * distance, Quaternion.identity);

            if (Random.value <= 0.3)
                CmdInstantiateAsteroidOnServerThenUpdateClient(startSpawn + inc + Random.insideUnitSphere * distance, Quaternion.identity);
        }

        // spawn endless Asteroids from startSpawn
        while (true)
        {
            if (Random.value <= 0.5)
                CmdInstantiateAsteroidOnServerThenUpdateClient(startSpawn, Quaternion.identity);

            CmdInstantiateAsteroidOnServerThenUpdateClient(startSpawn + Random.insideUnitSphere * distance, Quaternion.identity);
            CmdInstantiateAsteroidOnServerThenUpdateClient(startSpawn + Random.insideUnitSphere * distance, Quaternion.identity);
            CmdInstantiateAsteroidOnServerThenUpdateClient(startSpawn + Random.insideUnitSphere * distance, Quaternion.identity);


            if (Random.value <= 0.3)
                CmdInstantiateAsteroidOnServerThenUpdateClient(startSpawn + Random.insideUnitSphere * distance, Quaternion.identity);

            RpcUpdateAsteroidHolder();
            yield return new WaitForSeconds(asteroidSpawnWaitSeconds);
        }
    }

    [ClientRpc]
    private void RpcUpdateAsteroidHolder()
    {
        AsteroidsHolder.name = "Asteroid Holder (" + AsteroidsHolder.transform.childCount + ")";
    }

    [Command]
    private void CmdInstantiateAsteroidOnServerThenUpdateClient(Vector3 startingPosition, Quaternion startingRotation)
    {
        if (isServer == false) throw new System.Exception("Unauthorized Access to instantiating Asteroids");
        GameObject asteroid = Instantiate(AsteroidPrefab, startingPosition, startingRotation);
        asteroid.transform.parent = AsteroidsHolder.transform;
        NetworkServer.Spawn(asteroid);
        RpcInstantiateAsteroid(asteroid.GetComponent<NetworkIdentity>(), asteroid.transform.position, asteroid.transform.rotation);
    }

    [ClientRpc]
    private void RpcInstantiateAsteroid(NetworkIdentity asteroidIdentity, Vector3 startingPosition, Quaternion startingRotation)
    {
        if (asteroidIdentity == null) throw new MissingComponentException("Error: Asteroid did not have NetworkIdentity");
        asteroidIdentity.gameObject.transform.parent = AsteroidsHolder.transform;
        asteroidIdentity.gameObject.transform.position = startingPosition;
        asteroidIdentity.gameObject.transform.rotation = startingRotation;
    }


    //please dont rename this function
    void updateScoreGUI(int newScore)
    {
        score = newScore;
        _scoreText.GetComponent<Text>().text = "Combined Score: " + score;
    }

    public void addScore (int newScore)
    {
        if (!isServer)
            CmdSetScore(newScore);
        else
            score += newScore;
    }

    [Command]
    private void CmdSetScore(int newScore)
    {
        score += newScore;
    }

    public void GameOverFunction(){
        _gameOverText.GetComponent<Text>().text = "Game Over!";
        _RestartText.GetComponent<Text>().text = "Press 'R' for restart";
        restart = true;
        gameOver = true;
        AudioListener audioListener = GetComponent<AudioListener>();
        audioListener.enabled = true;
        AudioSource[] allAudioSources;
        allAudioSources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
        foreach (AudioSource audioS in allAudioSources)
        {
            audioS.Stop();
        }
        AudioSource audioData = GetComponent<AudioSource>();
        audioData.Play();
    }
    public void setExtraRocket(bool status) { extraRocket = status; }
    public bool getExtraRocketStatus() { return extraRocket; }
    public bool getSpeedGift() { return speedGift; }
    public void setSpeedGift(bool status) { speedGift = status; }
}
