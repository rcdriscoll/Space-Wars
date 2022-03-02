using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //configuration parameters
    [Header("Player")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float padding = 1f;
    [SerializeField] int health = 500;

    [Header("Projectile")]
    [SerializeField] GameObject laserPrefab;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] float projectileFirePeriod = 0.1f;

    [Header("SFX")]
    [SerializeField] AudioClip playerLaser;
    [SerializeField] AudioClip explosionSFX;
    [SerializeField] [Range(0, 1)] float laserVolume = .25f;
    [SerializeField] [Range (0,1)] float explosionVolume = 1f;

    [Header("VFX")]
    [SerializeField] GameObject explosionVFX;
    [SerializeField] float durationOfExplosion = 1f;


    Coroutine firingCoroutine;
         

    //state variables
    float xMin;
    float xMax;
    float yMin;
    float yMax;

    // Start is called before the first frame update
    void Start()
    {
        SetUpMoveBoundaries();
    }


    // Update is called once per frame
    void Update()
    {
        Move();
        Fire();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();

        //checks null reference is damage dealer is not present
        if(!damageDealer)
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
        Destroy(gameObject);

        //particle exposion VFX
        GameObject explosion = Instantiate(
           explosionVFX,
           transform.position,
           transform.rotation);

        Destroy(explosion, durationOfExplosion);

        AudioSource.PlayClipAtPoint(explosionSFX, Camera.main.transform.position, explosionVolume);


        FindObjectOfType<Level>().LoadGameOver();
    }

    public int GetHealth()
    {
        return health;
    }

    private void Move()
    {
        //horizontal movement
        var deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        var newXPos = Mathf.Clamp(transform.position.x + deltaX, xMin, xMax);

        //vertical movement
        var deltaY = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
        var newYPos = Mathf.Clamp(transform.position.y + deltaY, yMin, yMax);

        //actually moving the player
        transform.position = new Vector2(newXPos, newYPos);
    }

    private void Fire()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            firingCoroutine = StartCoroutine(FireContinuously());

        }
        if (Input.GetButtonUp("Fire1"))
        {
            StopCoroutine(firingCoroutine);
        }
    }

    IEnumerator FireContinuously()
    {
        while (true)
        {
            GameObject laser = Instantiate(
                laserPrefab,                        // the object being created
                 transform.position,                 // location to be spawned
                 Quaternion.identity) as GameObject; // no rotation is quaternion identity

            laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, projectileSpeed);

            yield return new WaitForSeconds(projectileFirePeriod);

            AudioSource.PlayClipAtPoint(playerLaser, Camera.main.transform.position, laserVolume);
        }

    }

    private void SetUpMoveBoundaries()
    {
        Camera gameCamera = Camera.main;
        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + padding;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - padding;

        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + padding;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - padding;

    }
}
