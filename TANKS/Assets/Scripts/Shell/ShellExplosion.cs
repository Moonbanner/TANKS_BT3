using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class ShellExplosion : NetworkBehaviour
{
    public LayerMask m_TankMask;
    public GameObject m_ShellExplosion;
    //public ParticleSystem m_ExplosionParticles;       //changed the Shell as parent - Explosion as child setup so the network can spawn/despawn the explosions
    //public AudioSource m_ExplosionAudio;              
    public float m_MaxDamage = 100f;                  
    public float m_ExplosionForce = 1000f;            
    public float m_MaxLifeTime = 2f;                  
    public float m_ExplosionRadius = 5f;              


    private void Start()
    {
        Destroy(gameObject, m_MaxLifeTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        // Find all the tanks in an area around the shell and damage them.
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);

        for (int i=0; i<colliders.Length; i++)
        {
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();

            if (!targetRigidbody)
                continue;

            targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

            TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>();

            if (!targetHealth)
                continue;

            float damage = CalculateDamage(targetRigidbody.position);

            targetHealth.TakeDamage(damage);
        }

        PlayExplosionParticlesServerRpc();

    }

    [ServerRpc]
    private void PlayExplosionParticlesServerRpc()
    {
        GameObject shellExplosion = Instantiate(m_ShellExplosion, transform.position, transform.rotation);
        shellExplosion.GetComponent<NetworkObject>().Spawn();

        //shellExplosion.GetComponent<NetworkObject>().Despawn(); //Editor keeps warning about destroying while object is still spawned if this is kept in
        Destroy(shellExplosion.gameObject, shellExplosion.GetComponent<ParticleSystem>().main.duration);

        this.gameObject.GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }

    private float CalculateDamage(Vector3 targetPosition)
    {
        // Calculate the amount of damage a target should take based on it's position.
        Vector3 explosionToTarget = targetPosition - transform.position;

        float explosionDistance = explosionToTarget.magnitude;

        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;

        float damage = relativeDistance * m_MaxDamage;
        damage = Mathf.Max(0f, damage);

        return damage;
    }
}