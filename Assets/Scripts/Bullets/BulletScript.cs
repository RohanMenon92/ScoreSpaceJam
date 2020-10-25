using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    // Can be part of a "Bullet" Super Class
    public GameConstants.GunTypes gunType = GameConstants.GunTypes.Machine;
    public bool isEnemyShot;
    public float damage;
    public float bulletSpeed;
    public float aliveForSeconds = 2f;

    float timeAlive;
    GameManager gameManager;
    Rigidbody2D rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        timeAlive = 0;
        gameManager = FindObjectOfType<GameManager>();
        rigidbody = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        if(rigidbody == null)
        {
            rigidbody = GetComponent<Rigidbody2D>();
        }
        timeAlive = 0;
    }

    void CheckDeath(float time)
    {
        timeAlive += time;
        if(timeAlive > aliveForSeconds)
        {
            ReturnBulletToPool();
        }
    }
    // Update is called once per frame
    void Update()
    {
        CheckDeath(Time.deltaTime);
    }

    void ReturnBulletToPool()
    {
        // Return Bullet
        gameManager.ReturnBulletToPool(gameObject, gunType);
    }

    internal void OnHit()
    {
        ReturnBulletToPool();
    }

    // Called when player refelects a shot
    public void OnParry(Vector3 direction)
    {
        // When HitByShield, reverseBullet according to normal Vector
        isEnemyShot = !isEnemyShot;
        timeAlive = 0;

        rigidbody.velocity = Vector3.zero;
        rigidbody.AddForce(-direction * GameConstants.parryForce);
    }

    internal void FireBullet()
    {
        rigidbody.AddForce(transform.forward * bulletSpeed, ForceMode2D.Impulse);
    }
}
