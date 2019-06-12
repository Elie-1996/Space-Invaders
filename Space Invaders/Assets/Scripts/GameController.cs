using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject gameBackground;
    public GameObject Planets;
    public GameObject AsteroidPrefab;
    public GameObject EnemyPrefab;
    public Transform playerTransform;
    public float asteroidSpawnWaitSeconds;
    public float enemyIntervalSpawnWaitSeconds;
    public Text scoreText;
    public Text gameOverText;
    public Text RestartText;

    private GameObject circule;
    private bool gameOver;
    private bool restart;
    private int score;
    private int maxAllowedLevels;
    private bool shouldAdvanceLevel = false;
    private int level;
    // Start is called before the first frame update
    void Start()
    {
        if (Planets == null || gameBackground == null || AsteroidPrefab == null || playerTransform == null) throw new MissingReferenceException();
        score = 0;
        updateScore();
        gameOver = false;
        restart = false;
        gameOverText.text = "";
        RestartText.text = "";

        maxAllowedLevels = Planets.transform.childCount;
        level = 1;
        shouldAdvanceLevel = false;
        InitiatePlanetLocations();
        StartCoroutine (LevelSystem());
        StartCoroutine (SpawnAsteroids());

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
                Application.LoadLevel(Application.loadedLevel);
            }
        }
    }
    IEnumerator SpawnAsteroids()
    {
        float radius = Utils.getBackgroundRadius(gameBackground);
        float distance = 12.0f;
        Vector3 startSpawn = Utils.getRandomDirection() * radius;
        Vector3 direction = ((playerTransform.position + new Vector3(15.0f, 15.0f, 15.0f))- startSpawn).normalized;
        Utils.setAsteroidDirection(direction);
        
        // spawn the first 1000
        for (int i = 0; i < 800; i+=5)
        {
            Vector3 inc = new Vector3(direction.x * i, direction.y * i, direction.z * i);
            if (Random.value <= 0.5)
                Instantiate(AsteroidPrefab, startSpawn + inc, Quaternion.identity);

            Instantiate(AsteroidPrefab, startSpawn + inc + Random.insideUnitSphere * distance, Quaternion.identity);
            Instantiate(AsteroidPrefab, startSpawn + inc + Random.insideUnitSphere * distance, Quaternion.identity);
            Instantiate(AsteroidPrefab, startSpawn + inc + Random.insideUnitSphere * distance, Quaternion.identity);
            Instantiate(AsteroidPrefab, startSpawn + inc + Random.insideUnitSphere * distance, Quaternion.identity);
            Instantiate(AsteroidPrefab, startSpawn + inc + Random.insideUnitSphere * distance, Quaternion.identity);
            Instantiate(AsteroidPrefab, startSpawn + inc + Random.insideUnitSphere * distance, Quaternion.identity);
            Instantiate(AsteroidPrefab, startSpawn + inc + Random.insideUnitSphere * distance, Quaternion.identity);
            Instantiate(AsteroidPrefab, startSpawn + inc + Random.insideUnitSphere * distance, Quaternion.identity);

           
            if (Random.value <= 0.5)
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
            Instantiate(AsteroidPrefab, startSpawn + Random.insideUnitSphere * distance, Quaternion.identity);
            Instantiate(AsteroidPrefab, startSpawn + Random.insideUnitSphere * distance, Quaternion.identity);
            Instantiate(AsteroidPrefab, startSpawn + Random.insideUnitSphere * distance, Quaternion.identity);
            Instantiate(AsteroidPrefab, startSpawn + Random.insideUnitSphere * distance, Quaternion.identity);
            Instantiate(AsteroidPrefab, startSpawn + Random.insideUnitSphere * distance, Quaternion.identity);


            if (Random.value <= 0.5)
                Instantiate(AsteroidPrefab, startSpawn + Random.insideUnitSphere * distance, Quaternion.identity);

            yield return new WaitForSeconds(asteroidSpawnWaitSeconds);
            if (gameOver)
            {
                RestartText.text = "Press 'R' for restart";
                restart = true;
                break;
            }
        }
    }

    void updateScore()
    {
        scoreText.text = "Score: " + score;
    }

    public void addScore (int newScore)
    {
        score += newScore;
        updateScore();
    }

    public void GameOverFunction(){
        gameOverText.text = "Game Over!";
        gameOver = true;
        AudioListener audioListener = GetComponent<AudioListener>();
        audioListener.enabled = true;
        AudioSource[] allAudioSources;
        allAudioSources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
        foreach (AudioSource audioS in allAudioSources)
        {
            audioS.Stop();
        }
        //GameObject gameConrollerObject = GameObject.FindWithTag(Utils.TagBackground);
        //if (gameConrollerObject != null)
        //{
        //    circule = gameConrollerObject.GetComponent<GameObject>();
        //    AudioSource audio = circule.GetComponent<AudioSource>();
        //    audio.Pause();
        //}
        AudioSource audioData = GetComponent<AudioSource>();
        audioData.Play();
    }
}
