using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    public GameConstants.EnemyTypes enemyType;
    public List<GunPort> gunPorts = new List<GunPort>();
    public float rotateSpeed = 0.75f;

    public float health = 20f;

    bool isInRange;
    PlayerScript player;
    GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerScript>();
        gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckPlayerRange();
        TryToAim();
    }

    void CheckPlayerRange()
    {
        isInRange = Vector3.Distance(player.transform.position, transform.position) < GameConstants.turretRange;
    }

    void TryToAim()
    {
        if(isInRange)
        {
            Vector3 targetPosition = player.GetPredictedAim();
            targetPosition.z = transform.position.z;
            Quaternion lookOnLook = Quaternion.LookRotation(targetPosition - transform.position);

            transform.rotation = Quaternion.Slerp(transform.rotation, lookOnLook, Time.deltaTime * rotateSpeed);

            foreach(GunPort gun in gunPorts)
            {
                gun.Fire(true, Vector3.zero);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Bullet"))
        {
            BulletScript bullet = collision.transform.GetComponent<BulletScript>();
            health -= bullet.damage;

            if(health < 0)
            {
                OnDeath();
            }

            bullet.OnHit();
            //canBeHit = false;
            //StartCoroutine("ResetCanBeHit");
        }
    }

    public void OnDeath()
    {
        gameManager.ReturnEnemyToPool(gameObject, enemyType);
    }
}
