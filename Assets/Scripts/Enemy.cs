using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] float health = 100;
    [SerializeField] int scoreValue = 150;

    [Header("Shooting")]
    float shotCounter;
    [SerializeField] float minTimeBetweenShots = 0.2f;
    [SerializeField] float maxTimeBetweenShots = 3f;
    [SerializeField] GameObject projectile;
    [SerializeField] float projectileSpeed = 10f;

    [Header("VFX")]
    [SerializeField] GameObject explosionVFX;
    [SerializeField] float durationOfExplosion = 1f;

    [Header("SFX")]
    [SerializeField] AudioClip explosionSFX;
    [SerializeField] AudioClip enemyLaser;
    [SerializeField] [Range(0, 1)] float laserVolume = .75f;
    [SerializeField] [Range(0, 1)] float explosionVolume = 1f;

    // Start is called before the first frame update
    void Start()
    {
        shotCounter = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
    }

    // Update is called once per frame
    void Update()
    {
        CountDownAndShoot();  
    }

    private void CountDownAndShoot()
    {
        shotCounter -= Time.deltaTime;

        if (shotCounter <= 0f)
        {
            Fire();
            shotCounter = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
        }
    }

    private void Fire()
    {
        GameObject laser = Instantiate(
               projectile,                         // the object being created
               transform.position,                 // location to be spawned
               Quaternion.identity) as GameObject; // no rotation is quaternion identity

        laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -projectileSpeed);

        AudioSource.PlayClipAtPoint(enemyLaser, Camera.main.transform.position, laserVolume);

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();

        //checks null reference is damage dealer is not present
        if (!damageDealer)
        {
            return;
        }

        ProcessHit(damageDealer);
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();

        damageDealer.Hit();

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        //score up
        FindObjectOfType<GameSession>().AddToScore(scoreValue);

        //destroy the enemy
        Destroy(gameObject);

        //particle exposion VFX
        GameObject explosion = Instantiate(
           explosionVFX,                         
           transform.position,                 
           transform.rotation);

        Destroy(explosion, durationOfExplosion);

        //play SFX
        AudioSource.PlayClipAtPoint(explosionSFX, Camera.main.transform.position, explosionVolume);
    }
}
