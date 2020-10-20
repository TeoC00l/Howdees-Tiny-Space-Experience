//@Author: Teodor Tysklind / FutureGames / Teodor.Tysklind@FutureGames.nu

using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Explosion))]
public class Asteroid : Knockable
{
    private int noOfActiveCrystals = default;
    private Crystal[] crystals;
    private DebrisSpawner debrisSpawner;
    private Explosion explosion;
    private Rigidbody body;
    
    [Header("Crystal Properties")]
    private float crystalSize = 1f;
    
    [Tooltip("Chance of a crystal spawning on each of the asteroid crystal slots. 0 = none, 1 = always")]
    [Range(0f, 1f)][SerializeField] private float crystalSpawnChance = 0.5f;

    [Header("Velocity Properties")]
    [Tooltip("Restimulates velocity when velocity falls below this magnitude")]
    [SerializeField] private float restimulateVelocityThreshold = 0.2f;
    
    [Tooltip("Restimulates velocity by this magnitude")]
    [SerializeField] private float restimulationVelocity = 0.3f;
    
    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        explosion = GetComponent<Explosion>();
        crystals = GetComponentsInChildren<Crystal>();

        Assert.IsNotNull(crystals);
    }

    private void Update()
    {
        RestimulateVelocity();
    }

    private void OnEnable()
    {
        noOfActiveCrystals = 0;
        
        foreach (Crystal crystal in crystals)
        {
            crystal.gameObject.SetActive(false);
        }
        
        foreach (Crystal crystal in crystals)
        {
            if (Random.Range(0f, 1f) < crystalSpawnChance)
            {
                crystal.gameObject.SetActive(true);
                noOfActiveCrystals++;
            }
        }
    }

    public void RemoveCrystal(GameObject crystal)
    {
        noOfActiveCrystals--;
        
        crystal.SetActive(false);

        if (noOfActiveCrystals == 0)
        {
            explosion.ExplodeAfterDelay();
        }
    }

    public void SetAsteroidSize(float diameter, float mass)
    {
        transform.localScale = new Vector3(diameter, diameter, diameter);
        body.mass = mass;

        foreach (Crystal crystal in crystals)
        {
            float asteroidSize = transform.localScale.x;
            float scale = crystalSize / asteroidSize;
            crystal.transform.localScale = new Vector3(scale, scale, scale);
            
            crystal.SetPosition();
        }
    }

    private void RestimulateVelocity()
    {
        if (body.velocity.magnitude > restimulateVelocityThreshold)
        {
            return;
        }
        
        body.velocity = body.velocity.normalized * restimulationVelocity;
    }

    public override void Knock(Vector3 knockVelocity)
    {
        gameObject.GetComponent<Rigidbody>().velocity += knockVelocity;
    }
}
