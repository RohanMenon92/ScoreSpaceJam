using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPort : MonoBehaviour
{
    public GameConstants.GunTypes gunType = GameConstants.GunTypes.Machine;
    // Start is called before the first frame update
    GameManager gameManager;
    //public AudioClip bulletSound;
    //public AudioClip shotgunSound;
    //public AudioClip laserSound;
    //AudioSource musicPlayer;

    public float firePerSeconds = 2f;
    bool canFire = true;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        //musicPlayer = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Fire(bool isEnemy, Vector3 initVelocity)
    {
        // Check if it can fire
        if(canFire)
        {
            switch (gunType)
            {
                case GameConstants.GunTypes.Machine:
                    BulletScript newBullet = gameManager.GetBullet(gunType, transform).GetComponent<BulletScript>();
                    newBullet.transform.position = transform.position + transform.forward;
                    newBullet.transform.rotation = transform.rotation;
                    newBullet.isEnemyShot = isEnemy;
                    newBullet.GetComponent<Rigidbody2D>().velocity = initVelocity;
                    newBullet.gameObject.SetActive(true);
                    newBullet.FireBullet();
                    //musicPlayer.clip = bulletSound;
                    //if (!musicPlayer.isPlaying) 
                    //{
                    //    musicPlayer.Play();
                    //}
                    // Set transform and set initial velocity of rigidbody component
                    break;
                case GameConstants.GunTypes.Shot:
                    // Do it for 5 shots
                    for(int i = 0; i < 6; i++)
                    {
                        BulletScript newShoutgunBullet = gameManager.GetBullet(gunType, transform).GetComponent<BulletScript>();
                        float variance = +Random.Range(-1f, 1f);
                        newShoutgunBullet.transform.position = transform.position + (transform.forward) + (transform.right * variance * 2);
                        newShoutgunBullet.transform.eulerAngles =  new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + variance, transform.eulerAngles.z);
                        newShoutgunBullet.isEnemyShot = isEnemy;
                        // Set bullet velocity to ship velocity
                        newShoutgunBullet.GetComponent<Rigidbody2D>().velocity = initVelocity;
                        newShoutgunBullet.gameObject.SetActive(true);
                        newShoutgunBullet.FireBullet();
                    }
                    //musicPlayer.clip = shotgunSound;
                    //if (!musicPlayer.isPlaying)
                    //{
                    //    musicPlayer.Play();
                    //}
                    // Get 5-10 ShotGun Bullets from GameManager.ShotGunBulletPool
                    // Give random spread to rotation/initial position
                    // Set Initial Velocity of rigidbody
                    break;
                case GameConstants.GunTypes.Grenade:
                    BulletScript grenadeBullet = gameManager.GetBullet(gunType, transform).GetComponent<BulletScript>();
                    grenadeBullet.transform.position = transform.position + transform.forward;
                    grenadeBullet.transform.rotation = transform.rotation;
                    grenadeBullet.isEnemyShot = isEnemy;
                    grenadeBullet.GetComponent<Rigidbody2D>().velocity = initVelocity;
                    grenadeBullet.gameObject.SetActive(true);
                    grenadeBullet.FireBullet();

                    //musicPlayer.clip = laserSound;
                    //if (!musicPlayer.isPlaying)
                    //{
                    //    musicPlayer.Play();
                    //}
                    break;
            }
            gameManager.PlaySound(GameConstants.SoundType.Shot);
            // Cannot fire and start Coroutine to be able to fire next
            canFire = false;
            StartCoroutine(ResetCanFire());
        }
    }

    IEnumerator ResetCanFire()
    {
        // Wait for firePerSeconds and the say canFire is true 
        yield return new WaitForSeconds(firePerSeconds);
        canFire = true;
    }

    public void resetCanFireOnRespawn()
    {
        StopCoroutine(ResetCanFire());
        canFire = true;
    }
}
