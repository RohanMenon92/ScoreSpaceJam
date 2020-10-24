﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeScript : MonoBehaviour
{
    // Can be part of a "Bullet" Super Class
    public bool isEnemyShot;
    public float damage;
    public float bulletSpeed;
    public float aliveForSeconds = 4f;

    float timeAlive;
    GameManager gameManager;
    Rigidbody2D rigidbody;
    
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        timeAlive = 0;
        gameManager = FindObjectOfType<GameManager>();
    }

    void OnEnable()
    {
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
        gameManager.ReturnBulletToPool(gameObject, GameConstants.GunTypes.Grenade);
    }

    internal void OnHit()
    {
        ReturnBulletToPool();
    }

    // Called when player refelects a shot
    internal void OnShield(Vector3 normalVector)
    {
        // When HitByShield, reverseBullet according to normal Vector
        isEnemyShot = !isEnemyShot;
        timeAlive = 0;

        Vector3 newVelocity = Vector3.Reflect(this.GetComponent<Rigidbody>().velocity, normalVector);
        rigidbody.velocity = new Vector3(newVelocity.x, 0f, newVelocity.z);
    }

    internal void FireBullet()
    {
        rigidbody.AddForce(transform.forward * bulletSpeed, ForceMode2D.Impulse);
    }
}