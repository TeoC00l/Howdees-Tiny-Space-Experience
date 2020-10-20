//@Author: Teodor Tysklind / FutureGames / Teodor.Tysklind@FutureGames.nu

using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(DebrisSpawner))]
public class Explosion : MonoBehaviour
{
    [Header("Properties")]
    
    [SerializeField] private GameObject ExplosionAnimationObject;
    [SerializeField] private float explosionMagnitude = default;
    
    [Tooltip("Explosion size = asteroid size + explosion offset")]
    [SerializeField] private float explosionOffset = default;

    [Tooltip("Time from destruction of asteroid until explosion")]
    [SerializeField] private float explosionDelay = default;
    
    [Tooltip("Explosion animation size = asteroid size times this value")]
    [SerializeField] private float explosionAnimationSizeMultiplier = default;

    private Transform asteroidTransform;
    private DebrisSpawner debrisSpawner;
    
    private float asteroidDiameter = default;

    private void Start()
    {
        asteroidTransform = transform;
        asteroidDiameter = gameObject.transform.localScale.x;
        Debug.Log(asteroidDiameter);
        debrisSpawner = GetComponent<DebrisSpawner>();
        
        Assert.IsNotNull(ExplosionAnimationObject);
    }
    
    private void Explode()
    {
        KnockAdjacent();

        float explosionAnimationDiameter = asteroidDiameter * explosionAnimationSizeMultiplier;
        Vector3 explosionAnimationSize = new Vector3(explosionAnimationDiameter,explosionAnimationDiameter,explosionAnimationDiameter);

        GameObject explosionAnimationObject = Instantiate(ExplosionAnimationObject, transform.position, Quaternion.identity);
        explosionAnimationObject.transform.localScale = explosionAnimationSize;
        
        SoundManager.Instance.PlaySound("AsteroidExplosion", asteroidTransform.position);
        
        debrisSpawner.SpawnDebris();
        gameObject.SetActive(false);
    }

    private void KnockAdjacent()
    {
        Collider[] collidersHit = Physics.OverlapSphere(transform.position, asteroidDiameter/2 + explosionOffset);
        
        foreach (Collider hitCollider in collidersHit)
        {
            Knockable knockable = hitCollider.gameObject.GetComponent<Knockable>();

            if (knockable == null)
            {
                continue;
            }

            Vector3 knockDirection = hitCollider.transform.position - asteroidTransform.position;
            hitCollider.gameObject.GetComponent<Knockable>().Knock(knockDirection * explosionMagnitude);
        }
    }

    public void ExplodeAfterDelay()
    {
        Invoke(nameof(Explode), explosionDelay);
    }

}
