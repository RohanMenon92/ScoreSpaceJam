using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public enum GameState
{
    Start,
    Play,
    End,
    Exit
}

public class GameManager : MonoBehaviour
{
    public GameObject shotgunBulletPrefab;
    public GameObject bulletPrefab;
    public GameObject grenadeBulletPrefab;

    public Transform unusedBulletPool;
    public Transform unusedShotgunBulletPool;
    public Transform unusedGrenadeBulletPool;

    public GameObject gunTurretPrefab;
    public GameObject shotTurretPrefab;
    public GameObject dualTurretPrefab;

    public Transform unusedGunTurretPool;
    public Transform unusedShotTurretPool;
    public Transform unusedDualTurretPool;

    public GameObject shieldHitPrefab;
    public GameObject bulletHitPrefab;
    public GameObject explodeEffectPrefab;

    public Transform shieldHitEffectsPool;
    public Transform bulletHitEffectsPool;
    public Transform explodeEffectsPool;

    public Transform worldBullets;
    public Transform worldEffects;

    public Image healthHolder;
    public Image healthBar;

    public Image timeHolder;
    public Image timeBar;
    public TextMeshProUGUI scoreHolder;

    public int score;
    public float time;

    public Material parriedBulletMaterial;

    Sequence healthSequence;

    GameState currentState;


    // Start is called before the first frame update
    void Start()
    {
        // reset score
        score = 0;
        time = GameConstants.maxTime;

        // Instantiate bullet pools in start
        for (int i = 0; i <= GameConstants.bulletPoolSize; i++)
        {
            GameObject newBullet = Instantiate(bulletPrefab, unusedBulletPool);
            newBullet.SetActive(false);
        }
        for (int i = 0; i <= GameConstants.shotBulletPoolSize; i++)
        {
            GameObject newBullet = Instantiate(shotgunBulletPrefab, unusedShotgunBulletPool);
            newBullet.SetActive(false);
        }
        for (int i = 0; i <= GameConstants.grenadeBulletPoolSize; i++)
        {
            GameObject newBullet = Instantiate(grenadeBulletPrefab, unusedGrenadeBulletPool);
            newBullet.SetActive(false);
        }

        // Instantiate turrets pools in start
        for (int i = 0; i <= GameConstants.gunTurretPoolSize; i++)
        {
            GameObject gunTurret = Instantiate(gunTurretPrefab, unusedGunTurretPool);
            gunTurret.SetActive(false);
        }
        for (int i = 0; i <= GameConstants.shotTurretPoolSize; i++)
        {
            GameObject gunTurret = Instantiate(shotTurretPrefab, unusedShotTurretPool);
            gunTurret.SetActive(false);
        }
        for (int i = 0; i <= GameConstants.dualTurretPoolSize; i++)
        {
            GameObject gunTurret = Instantiate(dualTurretPrefab, unusedDualTurretPool);
            gunTurret.SetActive(false);
        }

        // Instantiate Effects
        for (int i = 0; i <= GameConstants.shieldEffectsPoolSize; i++)
        {
            GameObject newShieldhit = Instantiate(shieldHitPrefab, shieldHitEffectsPool);
            newShieldhit.SetActive(false);
        }
        for (int i = 0; i <= GameConstants.bulletEffectsPoolSize; i++)
        {
            GameObject newBulletHit = Instantiate(bulletHitPrefab, bulletHitEffectsPool);
            newBulletHit.SetActive(false);
        }
        for (int i = 0; i <= GameConstants.explosionPoolSize; i++)
        {
            GameObject explodeEffect = Instantiate(explodeEffectPrefab, explodeEffectsPool);
            explodeEffect.SetActive(false);
        }
    }

    public void WaitAndSwitchState(GameState newState, float delay)
    {
        StartCoroutine(WaitStateSwitch(newState, delay));
    }

    public void SwitchState(GameState newState)
    {
        bool switchAllowed = false;

        // Do check for if switch is possible
        switch (currentState)
        {
            case GameState.Start:
                {
                    switchAllowed = newState == GameState.Play;
                }
                break;
            case GameState.Play:
                {
                    switchAllowed = newState == GameState.End;
                }
                break;
            case GameState.End:
                {
                    switchAllowed = newState == GameState.Exit;
                }
                break;
            case GameState.Exit:
                {
                }
                break;
        }

        if (switchAllowed)
        {
            OnExitState(currentState);
            currentState = newState;
            OnEnterState(currentState);
        }
    }

    // Check entry to stateEnter
    void OnEnterState(GameState stateEnter)
    {
        switch (stateEnter)
        {
            case GameState.Start:
                {
                }
                break;
            case GameState.Play:
                {
                }
                break;
            case GameState.End:
                {
                }
                break;
            case GameState.Exit:
                {
                }
                break;
        }
    }

    internal void AddScore(int scoreAmount)
    {
        healthSequence.Insert(0f, healthBar.rectTransform.DOScale(1.2f, GameConstants.scoreUpdate).SetLoops(2, LoopType.Yoyo));
        score += scoreAmount;
        scoreHolder.SetText(GameConstants.scorePrefix + score);
    }

    IEnumerator WaitStateSwitch(GameState newState, float delay)
    {
        yield return new WaitForSeconds(delay);

        SwitchState(newState);
    }

    void OnExitState(GameState stateExit)
    {
        switch (stateExit)
        {

            case GameState.Start:
                {
                }
                break;
            case GameState.Play:
                {
                }
                break;
            case GameState.End:
                {
                }
                break;
            case GameState.Exit:
                {
                }
                break;
        }
    }
    void OnProcessState(GameState stateProcess)
    {

        switch (stateProcess)
        {

            case GameState.Start:
                {
                }
                break;
            case GameState.Play:
                {
                }
                break;
            case GameState.End:
                {
                }
                break;
            case GameState.Exit:
                {
                }
                break;
        }
    }
    // FSM end region

    // Effect Pooling
    public GameObject BeginEffect(GameConstants.EffectTypes effectType, Vector3 position, Vector3 lookAt)
    {
        GameObject effectObject = null;

        switch (effectType)
        {
            // Get First Child, set parent to gunport (to remove from respective pool)
            case GameConstants.EffectTypes.BulletHit:
                effectObject = bulletHitEffectsPool.GetComponentInChildren<BulletHitEffect>(true).gameObject;
                break;
            case GameConstants.EffectTypes.ShieldHit:
                effectObject = shieldHitEffectsPool.GetComponentInChildren<ShieldHitEffect>(true).gameObject;
                break;
                break;
            case GameConstants.EffectTypes.ShipExplosion:
                effectObject = explodeEffectsPool.GetComponentInChildren<ExplosionEffect>(true).gameObject;
                break;
        }

        //bulletObject.transform.SetParent(worldBullets);
        //bulletObject.transform.position = gunPort.transform.position;
        //// Return bullet and let GunPort handle how to fire and set initial velocities
        //return bulletObject;
        effectObject.transform.SetParent(worldEffects);
        effectObject.transform.position = new Vector3(position.x, position.y, position.z);
        effectObject.transform.up = new Vector3(lookAt.x, lookAt.y, lookAt.z);

        effectObject.SetActive(true);

        switch (effectType)
        {
            // Get First Child, set parent to gunport (to remove from respective pool)
            case GameConstants.EffectTypes.BulletHit:
                effectObject.GetComponent<BulletHitEffect>().FadeIn();
                break;
            case GameConstants.EffectTypes.ShieldHit:
                effectObject.GetComponent<ShieldHitEffect>().FadeIn();
                break;
            case GameConstants.EffectTypes.ShipExplosion:
                effectObject.GetComponent<ExplosionEffect>().FadeIn();
                break;
        }

        return effectObject;
    }

    public void ReturnEffectToPool(GameObject effectToStore, GameConstants.EffectTypes effectType)
    {
        if (effectType == GameConstants.EffectTypes.BulletHit)
        {
            // Return to normal bullet pool
            effectToStore.transform.SetParent(bulletHitEffectsPool);
        }
        else if (effectType == GameConstants.EffectTypes.ShieldHit)
        {
            // Return to shotgun bullet pool
            effectToStore.transform.SetParent(shieldHitEffectsPool);
        }
        else if (effectType == GameConstants.EffectTypes.ShipExplosion)
        {
            // Return to laser bullet pool
            effectToStore.transform.SetParent(explodeEffectsPool);
        }

        effectToStore.gameObject.SetActive(false);
        effectToStore.transform.localScale = Vector3.one;
        effectToStore.transform.position = Vector3.zero;
    }

    // Bullet Pooling
    public GameObject GetBullet(GameConstants.GunTypes gunType, Transform gunPort)
    {
        GameObject bulletObject = null;
        // Get Bullet From pool and return it
        switch (gunType)
        {
            // Get First Child, set parent to gunport (to remove from respective pool)
            case GameConstants.GunTypes.Machine:
                bulletObject = unusedBulletPool.GetComponentInChildren<BulletScript>(true).gameObject;
                break;
            case GameConstants.GunTypes.Shot:
                bulletObject = unusedShotgunBulletPool.GetComponentInChildren<BulletScript>(true).gameObject;
                break;
            case GameConstants.GunTypes.Grenade:
                bulletObject = unusedGrenadeBulletPool.GetComponentInChildren<BulletScript>(true).gameObject;
                break;
        }

        bulletObject.transform.SetParent(worldBullets);
        bulletObject.transform.position = gunPort.transform.position;
        // Return bullet and let GunPort handle how to fire and set initial velocities
        return bulletObject;
    }

    public void ReturnBulletToPool(GameObject bulletToStore, GameConstants.GunTypes bulletType)
    {
        switch(bulletType)
        {
            case GameConstants.GunTypes.Machine:
                // Return to normal bullet pool
                bulletToStore.transform.SetParent(unusedBulletPool);
                bulletToStore.GetComponent<MeshRenderer>().material = bulletPrefab.GetComponent<MeshRenderer>().sharedMaterial;
                break;
            case GameConstants.GunTypes.Shot:
                // Return to shotgun bullet pool
                bulletToStore.transform.SetParent(unusedShotgunBulletPool);
                bulletToStore.GetComponent<MeshRenderer>().material = shotgunBulletPrefab.GetComponent<MeshRenderer>().sharedMaterial;
                break;
            case GameConstants.GunTypes.Grenade:
                // Return to laser bullet pool
                bulletToStore.transform.SetParent(unusedGrenadeBulletPool);
                bulletToStore.GetComponent<MeshRenderer>().material = grenadeBulletPrefab.GetComponent<MeshRenderer>().sharedMaterial;
                break;
        }
       bulletToStore.gameObject.SetActive(false);
        bulletToStore.transform.eulerAngles = Vector3.zero;
        bulletToStore.transform.position = Vector3.zero;
    }

    // Enemy Pooling
    public GameObject GetEnemy(GameConstants.EnemyTypes enemyType)
    {
        GameObject enemyObject = null;
        // Get Bullet From pool and return it
        switch (enemyType)
        {
            // Get First Child, set parent to gunport (to remove from respective pool)
            case GameConstants.EnemyTypes.GunTurret:
                enemyObject = unusedGunTurretPool.GetComponentInChildren<TurretController>(true).gameObject;
                break;
            case GameConstants.EnemyTypes.ShotTurret:
                enemyObject = unusedShotTurretPool.GetComponentInChildren<TurretController>(true).gameObject;
                break;
            case GameConstants.EnemyTypes.DualTurret:
                enemyObject = unusedDualTurretPool.GetComponentInChildren<TurretController>(true).gameObject;
                break;
        }

        enemyObject.transform.SetParent(worldBullets);
        // Return bullet and let GunPort handle how to fire and set initial velocities
        return enemyObject;
    }

    public void ReturnEnemyToPool(GameObject enemyToStore, GameConstants.EnemyTypes enemyType)
    {
        switch(enemyType)
        {
            case GameConstants.EnemyTypes.GunTurret:
                enemyToStore.transform.SetParent(unusedGunTurretPool);
                break;
            case GameConstants.EnemyTypes.ShotTurret:
                enemyToStore.transform.SetParent(unusedShotTurretPool);
                break;
            case GameConstants.EnemyTypes.DualTurret:
                enemyToStore.transform.SetParent(unusedDualTurretPool);
                break;
        }
        enemyToStore.gameObject.SetActive(false);
        enemyToStore.transform.eulerAngles = Vector3.zero;
        enemyToStore.transform.position = Vector3.zero;
    }

    // SlowMo
    public void SlowMoStart()
    {
        Time.timeScale = GameConstants.timeSlowDown;
        Time.fixedDeltaTime = 0.02F * Time.timeScale;
    }
    public void SlowMoStop()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02F;
    }

    public void SuccessfulAttack(float healthFraction)
    {
        OnHealthUpdate(healthFraction);

        timeBar.rectTransform.DOScale(1.5f, GameConstants.healthUpdate).SetLoops(2, LoopType.Yoyo);
        if (time + GameConstants.attackTimeGain < GameConstants.maxTime)
        {
            time += GameConstants.attackTimeGain;
        } else
        {
            time = GameConstants.attackTimeGain;
        }
    }

    public void OnHealthUpdate(float fraction)
    {
        //if(healthSequence != null && healthSequence.IsPlaying())
        //{
        //    healthSequence.Complete();
        //}

        healthSequence = DOTween.Sequence();

        healthSequence.Insert(0f, healthHolder.DOFade(1f, 0f));
        healthSequence.Insert(0f, healthBar.DOFade(1f, 0f));
        healthSequence.Insert(0f, healthBar.rectTransform.DOScale(1.2f, GameConstants.healthUpdate).SetLoops(2, LoopType.Yoyo));
        healthSequence.Insert(0f, healthBar.DOFillAmount(fraction, GameConstants.healthUpdate).SetEase(Ease.OutBack));

        healthSequence.Insert(GameConstants.healthUpdate, healthHolder.DOFade(0.25f, GameConstants.healthUpdate));
        healthSequence.Insert(GameConstants.healthUpdate, healthBar.DOFade(0.25f, GameConstants.healthUpdate));
        healthSequence.Play();
    }

    // Update is called once per frame
    void Update()
    {
        time -= Time.deltaTime;
        timeBar.fillAmount = time / GameConstants.maxTime;
    }

    void SpawnEnemies()
    {

    }
}
