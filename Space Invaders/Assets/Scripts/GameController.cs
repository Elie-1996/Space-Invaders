using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject gameBackground;
    public GameObject Planets;
    public GameObject AsteroidPrefab;
    public GameObject EnemyPrefab;
    public Transform playerTransform;
    public float asteroidSpawnWaitSeconds;
    public float enemyIntervalSpawnWaitSeconds;

    private int maxAllowedLevels;
    private bool shouldAdvanceLevel = false;
    private int level;

    // Start is called before the first frame update
    void Start()
    {
        if (Planets == null || gameBackground == null || AsteroidPrefab == null || playerTransform == null) throw new MissingReferenceException();
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

    IEnumerator SpawnAsteroids()
    {
        float radius = Utils.getBackgroundRadius(gameBackground);
        Vector3 startSpawn = Utils.getRandomDirection() * radius;
        Vector3 startSpawnVariant1 = startSpawn + new Vector3(1.0f, 3.0f, 2.0f) * 2;
        Vector3 startSpawnVariant2 = startSpawn - new Vector3(3.0f, 2.0f, 1.0f) * 2;
        Vector3 startSpawnVariant3 = startSpawn + new Vector3(2.0f, 2.0f, 1.0f) * 2;
        Vector3 startSpawnVariant4 = startSpawn - new Vector3(2.0f, 2.0f, 1.0f) * 2;
        Vector3 startSpawnVariant5 = startSpawn - new Vector3(1.8f, 2.4f, 1.9f) * 2;
        Vector3 startSpawnVariant6 = startSpawn - new Vector3(3.2f, 5.2f, 4.3f) * 2;
        Vector3 startSpawnVariant7 = startSpawn - new Vector3(2.3f, 2.9f, 4.1f) * 2;
        Vector3 startSpawnVariant8 = startSpawn - new Vector3(0.0f, 1.0f, 3.0f) * 2;
        Vector3 startSpawnVariant9 = startSpawn - new Vector3(0.5f, 2.0f, 3.0f) * 2;
        Vector3 direction = ((playerTransform.position + new Vector3(15.0f, 15.0f, 15.0f))- startSpawn).normalized;
        Utils.setAsteroidDirection(direction);
        
        // spawn the first 1000
        for (int i = 0; i < 800; i+=5)
        {
            Vector3 inc = new Vector3(direction.x * i, direction.y * i, direction.z * i);
            if (Random.value <= 0.5)
                Instantiate(AsteroidPrefab, startSpawn + inc, Quaternion.identity);
            if (Random.value <= 0.5)
                Instantiate(AsteroidPrefab, startSpawnVariant1 + inc, Quaternion.identity);
            else
                Instantiate(AsteroidPrefab, startSpawnVariant3 + inc, Quaternion.identity);
            if (Random.value <= 0.5)
                Instantiate(AsteroidPrefab, startSpawnVariant2 + inc, Quaternion.identity);
            else
                Instantiate(AsteroidPrefab, startSpawnVariant4 + inc, Quaternion.identity);
            if (Random.value <= 0.5)
                Instantiate(AsteroidPrefab, startSpawnVariant5 + inc, Quaternion.identity);
            else
                Instantiate(AsteroidPrefab, startSpawnVariant6 + inc, Quaternion.identity);
            if (Random.value <= 0.5)
                Instantiate(AsteroidPrefab, startSpawnVariant7 + inc, Quaternion.identity);
            else
                Instantiate(AsteroidPrefab, startSpawnVariant8 + inc, Quaternion.identity);
            if (Random.value <= 0.5)
                Instantiate(AsteroidPrefab, startSpawnVariant9 + inc, Quaternion.identity);
        }

        // spawn endless Asteroids from startSpawn
        while (true)
        {
            if (Random.value <= 0.5)
                Instantiate(AsteroidPrefab, startSpawn, Quaternion.identity);
            if (Random.value <= 0.5)
                Instantiate(AsteroidPrefab, startSpawnVariant1, Quaternion.identity);
            else
                Instantiate(AsteroidPrefab, startSpawnVariant3, Quaternion.identity);
            if (Random.value <= 0.5)
                Instantiate(AsteroidPrefab, startSpawnVariant2, Quaternion.identity);
            else
                Instantiate(AsteroidPrefab, startSpawnVariant4, Quaternion.identity);
            if (Random.value <= 0.5)
                Instantiate(AsteroidPrefab, startSpawnVariant5, Quaternion.identity);
            else
                Instantiate(AsteroidPrefab, startSpawnVariant6, Quaternion.identity);
            if (Random.value <= 0.5)
                Instantiate(AsteroidPrefab, startSpawnVariant7, Quaternion.identity);
            else
                Instantiate(AsteroidPrefab, startSpawnVariant8, Quaternion.identity);
            if (Random.value <= 0.5)
                Instantiate(AsteroidPrefab, startSpawnVariant9, Quaternion.identity);
            yield return new WaitForSeconds(asteroidSpawnWaitSeconds);
        }
    }

}
