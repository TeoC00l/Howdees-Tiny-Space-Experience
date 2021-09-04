//@Author: Teodor Tysklind / FutureGames / Teodor.Tysklind@FutureGames.nu

using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class AsteroidThrower : MonoBehaviour
{
    [Header("Spawn points")]
    [SerializeField] private float boundsSizeX;
    [SerializeField] private float boundsSizeY;
    [SerializeField] private float resolution = 5f;
    
    [Header("Settings")] 
    [SerializeField] private float magnitude = 5f;
    [SerializeField] private int interval = 30;
    private int intervalInFrames;
    
    private int noOfPointsX;
    private Vector3[] positions;
    private float timer;
    private AsteroidManager asteroidManager;
    private Vector3 centerPosition;
    
    private void OnValidate()
    {
        centerPosition = transform.position;
        
        noOfPointsX = (int) (boundsSizeX / resolution);
        int noOfPointsY = (int) (boundsSizeY / resolution);
        positions = new Vector3[noOfPointsX*2 + noOfPointsY*2];
        
        positions[0] = new Vector3(-boundsSizeX/2, -boundsSizeY/2) + centerPosition;
        positions[noOfPointsX] = new Vector3(boundsSizeX/2, -boundsSizeY/2)+ centerPosition;
        positions[noOfPointsX*2] = new Vector3(-boundsSizeX/2, boundsSizeY/2)+ centerPosition;
        positions[noOfPointsX*2+noOfPointsY] = new Vector3(boundsSizeX/2, boundsSizeY/2)+ centerPosition;
      
        for (int i = 1; i < noOfPointsX; i++)
        {
            positions[i] = new Vector3(-boundsSizeX/2 + i * resolution, boundsSizeY/2, 0)+ centerPosition;
            positions[i+noOfPointsX] = new Vector3(-boundsSizeX/2 + i * resolution, -boundsSizeY/2, 0)+ centerPosition;
        }

        for (int i = 1; i < noOfPointsY; i++)
        {
            positions[i+noOfPointsX*2] = new Vector3(-boundsSizeX/2 , -boundsSizeY/2+  i * resolution, 0)+ transform.position;
            positions[i+noOfPointsX*2 + noOfPointsY] = new Vector3(boundsSizeX/2 , -boundsSizeY/2+  i * resolution, 0)+ centerPosition;
        }
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Vector3 position1 = new Vector3(-boundsSizeX/2, -boundsSizeY/2, 0)+ centerPosition;
        Vector3 position2 = new Vector3(boundsSizeX/2, -boundsSizeY/2, 0)+ centerPosition;
        Vector3 position3 = new Vector3(-boundsSizeX/2, boundsSizeY/2, 0)+ centerPosition;
        Vector3 position4 = new Vector3(boundsSizeX/2, boundsSizeY/2, 0)+ centerPosition;
        
        Gizmos.color = Color.yellow;

        foreach (Vector3 position in positions)
        {
            Gizmos.DrawSphere(position, 3f);
        }
        
        Handles.color = Color.yellow;
        Handles.DrawLine(position1, position2);
        Handles.DrawLine(position3, position4);
        Handles.DrawLine(position1, position3);
        Handles.DrawLine(position2, position4);
    }
#endif

    private void Awake()
    {
        intervalInFrames = (int) (interval / Time.deltaTime);
        timer = intervalInFrames;
        asteroidManager = GetComponent<AsteroidManager>();
    }

    private void Update()
    {
        timer--;

        if (timer == 0)
        {
            ThrowAsteroid();
            timer += intervalInFrames;
        }
    }

    private void ThrowAsteroid()
    {
        Vector3 position = positions[Random.Range(0, positions.Length)];
        Vector3 velocity = (Vector3.zero - position).normalized * magnitude;
        
        asteroidManager.SpawnAsteroid(position, velocity);
    }
}
