using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ShellExplosion_Net : NetworkBehaviour {

    public LayerMask m_TankMask;                        // Used to filter what the explosion affects, this should be set to "Players".
    public LayerMask m_TreeMask;                        // Used to filter what the explosion affects, this should be set to "Tree".
    public LayerMask m_VehicleMask;                     // Used to filter what the explosion affects, this should be set to "Vehicle".
    public ParticleSystem m_ExplosionParticles;         // Reference to the particles that will play on explosion.
    public AudioSource m_ExplosionAudio;                // Reference to the audio that will play on explosion.
    public float m_MaxDamage = 100f;                    // The amount of damage done if the explosion is centred on a tank.
    public float m_ExplosionForce = 1000f;              // The amount of force added to a tank at the centre of the explosion.
    public float m_MaxLifeTime = 2f;                    // The time in seconds before the shell is removed.
    public float m_ExplosionRadius = 5f;                // The maximum distance away from the explosion tanks can be and are still affected.

    private float m_AgeTime = 0f;

    protected void Start ()
    {
        
    }

    [ServerCallback]
    protected virtual void Update()
    {
        GetComponent<Rigidbody>().angularVelocity = GetComponent<Rigidbody>().velocity;

        m_AgeTime += Time.deltaTime;
        if (m_AgeTime >= m_MaxLifeTime)
        {
            // If it isn't destroyed by then, destroy the shell after it's lifetime.
            NetworkServer.Destroy (gameObject);
        }
    }

    protected virtual void OnTriggerEnter (Collider other)
    {
        if (!isServer)
        {
            return;
        }

        // Collect all the colliders in a sphere from the shell's current position to a radius of the explosion radius.
        Collider[] players = Physics.OverlapSphere (transform.position, m_ExplosionRadius, m_TankMask);
        // Go through all the colliders...
        for (int i = 0; i < players.Length; i++)
        {
            // ... and find their rigidbody.
            Rigidbody targetRigidbody = players[i].GetComponent<Rigidbody> ();

            // If they don't have a rigidbody, go on to the next collider.
            if (!targetRigidbody)
                continue;

            // Add an explosion force.
            targetRigidbody.AddExplosionForce (m_ExplosionForce, transform.position, m_ExplosionRadius);

            // Find the TankHealth script associated with the rigidbody.
            TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth> ();

            // If there is no TankHealth script attached to the gameobject, go on to the next collider.
            if (!targetHealth)
                continue;

            // Calculate the amount of damage the target should take based on it's distance from the shell.
            float damage = CalculateDamage (targetRigidbody.position);


            // Deal this damage to the tank.
            targetHealth.TakeDamage (damage);
        }

        Collider[] trees = Physics.OverlapSphere (transform.position, 2f, m_TreeMask);

        for (int i = 0; i < trees.Length; i++)
        {
            Transform target = trees[i].GetComponent<Transform>();

            if (!target)
                continue;

            TreeDestroy treeDestroy = target.GetComponent<TreeDestroy>();

            if (!treeDestroy)
                continue;

            float damage = CalculateDamage (target.position);

            treeDestroy.TakeDamage(damage);
        }

        Collider[] vehicle = Physics.OverlapSphere (transform.position, m_ExplosionRadius, m_VehicleMask);

        for (int i = 0; i < vehicle.Length; i++)
        {
            // ... and find their rigidbody.
            Rigidbody targetRigidbody = vehicle[i].GetComponent<Rigidbody> ();

            // If they don't have a rigidbody, go on to the next collider.
            if (!targetRigidbody)
                continue;

            // Add an explosion force.
            targetRigidbody.AddExplosionForce (m_ExplosionForce, transform.position, m_ExplosionRadius);

        }



        ParticleSystem explosionParticles = Instantiate(m_ExplosionParticles, transform.position, transform.rotation) as ParticleSystem;

        // Unparent the particles from the shell.
        explosionParticles.transform.parent = null;

        // Play the particle system.
        explosionParticles.Play();

        // Play the explosion sound effect.
        m_ExplosionAudio.Play();

        // Once the particles have finished, destroy the gameobject they are on.
        Destroy (explosionParticles.gameObject, explosionParticles.duration);

        // Destroy the shell.
        Destroy (gameObject);
    }


    protected float CalculateDamage (Vector3 targetPosition)
    {
        // Create a vector from the shell to the target.
        Vector3 explosionToTarget = targetPosition - transform.position;

        // Calculate the distance from the shell to the target.
        float explosionDistance = explosionToTarget.magnitude;

        // Calculate the proportion of the maximum distance (the explosionRadius) the target is away.
        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;

        // Calculate damage as this proportion of the maximum possible damage.
        float damage = relativeDistance * m_MaxDamage;

        // Make sure that the minimum damage is always 0.
        damage = Mathf.Max (0f, damage);

        return damage;
    }
}
