//@Author: Teodor Tysklind / FutureGames / Teodor.Tysklind@FutureGames.nu

using UnityEngine;
using Random = UnityEngine.Random;

public class AsteroidManager : MonoBehaviour
{
    private PoolManager poolManager;
    
    [SerializeField] private GameObject asteroidPrefab;
    [SerializeField] private int asteroidPoolSize = 100;
    
    [Tooltip("Parent for all of the gameobject (transforms) that indicate asteroid spawn positions")]
    [SerializeField] private GameObject spawnPositionsParent;
    
    [Header("Settings for Asteroids spawning on start")]
    
    [Tooltip("Asteroids will spawn with a random velocity of a magnitude between these values")]
    [SerializeField] private float minMagnitude = 0.2f;
    [Tooltip("Asteroids will spawn with a random velocity of a magnitude between these values")]
    [SerializeField] private float maxMagnitude = 2f;

    [Tooltip("Asteroids randomized scale, minSize through maxSize")] 
    [SerializeField] private float minSize = 1f;
    [Tooltip("Asteroids randomized scale, minSize through maxSize")]
    [SerializeField] private float maxSize = 4f;
    [Tooltip("Mass of an asteroid the size in units times this multiplier")]
    [SerializeField] private float massToSizeMultiplier = 2f;

    [SerializeField] private float startTorqueMagnitude = 5f;
    
    private float size;

    private void Awake()
    {
        poolManager = PoolManager.Instance;
        poolManager.CreatePool(asteroidPrefab, asteroidPoolSize);

        SpawnAsteroidsOnStart();
    }

    private void SpawnAsteroidsOnStart()
    {
        Transform[] spawnPositions = spawnPositionsParent.GetComponentsInChildren<Transform>();
        
        foreach (Transform spawnTransform in spawnPositions)
        {
            GameObject asteroid = poolManager.ReuseObject(asteroidPrefab, spawnTransform.position, Quaternion.identity);

            Rigidbody body =  asteroid.GetComponent<Rigidbody>();
            body.velocity = randomizeVelocity();
            body.AddTorque(transform.forward * startTorqueMagnitude);

            size = Random.Range(minSize, maxSize);

            float mass = size * massToSizeMultiplier;

            asteroid.GetComponent<Asteroid>().SetAsteroidSize(size, mass);
        }
    }

    private Vector2 randomizeVelocity()
    {
        Vector2 direction = Random.insideUnitCircle.normalized;
        Vector2 velocity = direction * Random.Range(minMagnitude, maxMagnitude);

        return velocity;
    }

    public void SpawnAsteroid(Vector3 spawnPosition, Vector3 velocity)
    {
        GameObject asteroid = poolManager.ReuseObject(asteroidPrefab, spawnPosition, Quaternion.identity);
        Rigidbody body =  asteroid.GetComponent<Rigidbody>();
        
        body.velocity = randomizeVelocity();
        body.AddTorque(transform.forward * startTorqueMagnitude);
        size = Random.Range(minSize, maxSize);
        float mass = size * massToSizeMultiplier;

        asteroid.GetComponent<Asteroid>().SetAsteroidSize(size, mass);

        body.velocity = velocity;
    }
}
