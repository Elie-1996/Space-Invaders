using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public GameObject gameBackground;
    public GameObject Planets;
    public GameObject AsteroidPrefab;
    public GameObject EnemyPrefab;
    public Transform playerTransform;
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
    private int score;
    private int maxAllowedLevels;
    private bool shouldAdvanceLevel = false;
    private int level;
    private bool escape;

    private bool extraRocket;
    private bool speedGift;
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

    }


    // Start is called before the first frame update
    void Start()
    {
        if (Planets == null || gameBackground == null || AsteroidPrefab == null || playerTransform == null) throw new MissingReferenceException();
        loadGUI();
        score = 0;
        updateScore();
        gameOver = false;
        restart = false;
        _gameOverText.GetComponent<Text>().text = "";
        _RestartText.GetComponent<Text>().text = "";

        maxAllowedLevels = Planets.transform.childCount;
        level = 1;
        shouldAdvanceLevel = false;
        InitiatePlanetLocations();
        StartCoroutine (LevelSystem());
        //StartCoroutine(SpawnAsteroids());
        escape = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    IEnumerator LevelSystem()
    {
        while (level < maxAllowedLevels)
        {
            shouldAdvanceLevel = false;
            SpawnPlanets();
            StartCoroutine (SpawnLevel(level));
            ++level;
            yield return new WaitUntil(()=> shouldAdvanceLevel == true);
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
        foreach (Transform child in Planets.transform)
        {
            if (currentLevel == 0) break;
            child.gameObject.SetActive(true);
            currentLevel--;
        }
    }

    void InitiatePlanetLocations()
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
    IEnumerator SpawnAsteroids()
    {
        float radius = Utils.getBackgroundRadius(gameBackground);
        float distance = 12.0f;
        Vector3 startSpawn = Utils.getRandomDirection() * radius;
        Vector3 direction = ((playerTransform.position + new Vector3(35.0f, 35.0f, 35.0f))- startSpawn).normalized;
        Utils.setAsteroidDirection(direction);
        
        // spawn the first 800
        for (int i = 0; i < 750; i+=5)
        {
            Vector3 inc = new Vector3(direction.x * i, direction.y * i, direction.z * i);
            if (Random.value <= 0.5)
                Instantiate(AsteroidPrefab, startSpawn + inc, Quaternion.identity);

            Instantiate(AsteroidPrefab, startSpawn + inc + Random.insideUnitSphere * distance, Quaternion.identity);
            Instantiate(AsteroidPrefab, startSpawn + inc + Random.insideUnitSphere * distance, Quaternion.identity);
            Instantiate(AsteroidPrefab, startSpawn + inc + Random.insideUnitSphere * distance, Quaternion.identity);

            if (Random.value <= 0.3)
                Instantiate(AsteroidPrefab, startSpawn + inc + Random.insideUnitSphere * distance, Quaternion.identity);
        }

        // spawn endless Asteroids from startSpawn
        while (true)
        {
            if (Random.value <= 0.5)
                Instantiate(AsteroidPrefab, startSpawn, Quaternion.identity);

            Instantiate(AsteroidPrefab, startSpawn + Random.insideUnitSphere * distance, Quaternion.identity);
            Instantiate(AsteroidPrefab, startSpawn + Random.insideUnitSphere * distance, Quaternion.identity);
            Instantiate(AsteroidPrefab, startSpawn + Random.insideUnitSphere * distance, Quaternion.identity);


            if (Random.value <= 0.3)
                Instantiate(AsteroidPrefab, startSpawn + Random.insideUnitSphere * distance, Quaternion.identity);

            yield return new WaitForSeconds(asteroidSpawnWaitSeconds);
        }
    }

    void updateScore()
    {
        _scoreText.GetComponent<Text>().text = "Score: " + score;
    }

    public void addScore (int newScore)
    {
        score += newScore;
        updateScore();
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
