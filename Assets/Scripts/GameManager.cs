using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public enum GameState
{
    Start,
    Play,
    End
}

public class GameManager : MonoBehaviour
{
    public CanvasGroup gameOverCanvas;
    public CanvasGroup highScoreCanvas;
    public CanvasGroup tutorialCanvas;

    public TextMeshProUGUI GOScore;
    public TextMeshProUGUI GOenemies;
    public TextMeshProUGUI GOwaves;


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

    public Transform entities;
    public Transform worldBullets;
    public Transform worldEffects;

    public Image healthHolder;
    public Image healthBar;

    public Image timeHolder;
    public Image timeBar;
    public TextMeshProUGUI scoreHolder;
    public TextMeshProUGUI enemiesHolder;
    public int score;
    public int waveCount = 0;
    public int enemiesKilled = 0;
    public float time;

    [Header("Audio")]
    public AudioClip shotSound;
    public AudioClip hitSound;
    public AudioClip shieldOnSound;
    public AudioClip shieldOffSound;
    public AudioClip explodeSound;
    public AudioSource audioSource;

    public Material parriedBulletMaterial;

    Sequence healthSequence;

    GameState currentState = GameState.Start;


    // Start is called before the first frame update
    void Start()
    {
        // reset score
        score = 0;
        waveCount = 0;
        time = GameConstants.maxTime;
        enemiesKilled = 0;

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

        timeHolder.DOFade(0f, 0f);
        timeBar.DOFade(0f, 0f);
        healthHolder.DOFade(0f, 0f);
        healthBar.DOFade(0f, 0f);
        healthBar.fillAmount = 1f;

        UpdateEnemies();
        GameStart();
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
                    //switchAllowed = newState == GameState.Exit;
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

    void GameStart()
    {
        tutorialCanvas.DOFade(1f, 0.5f).OnComplete(() => {
            tutorialCanvas.DOFade(0f, 0.5f).SetDelay(3f).OnComplete(() => {
                SwitchState(GameState.Play);
            });
        });
    }

    // Check entry to stateEnter
    void OnEnterState(GameState stateEnter)
    {
        switch (stateEnter)
        {
            case GameState.Start:
                {
                    GameStart();
                }
                break;
            case GameState.Play:
                {
                    timeHolder.DOFade(1f, 0.25f);
                    timeBar.DOFade(1f, 0.25f);
                    healthHolder.DOFade(1f, 0.25f);
                    healthBar.DOFade(1f, 0.25f);
                }
                break;
            case GameState.End:
                {
                    GOScore.text = score.ToString();
                    GOenemies.text = enemiesKilled.ToString();
                    GOwaves.text = waveCount.ToString();

                    bool isHighScore = PlayerPrefs.GetInt(GameConstants.HighScorePlayerPref) < score;
                    if (isHighScore)
                    {
                        PlayerPrefs.SetInt(GameConstants.HighScorePlayerPref, score);
                    }

                    gameOverCanvas.DOFade(1f, 0.5f).OnComplete(() => {
                        highScoreCanvas.DOFade(isHighScore ? 1f : 0f, 0.25f);
                        gameOverCanvas.DOFade(0f, 0.5f).SetDelay(2f).OnComplete(() => {
                            SceneManager.LoadScene(0);
                        });
                    });
                }
                break;
        }
    }

    internal void UpdateEnemies()
    {
        enemiesHolder.rectTransform.DOScale(1.2f, GameConstants.scoreUpdate).SetLoops(2, LoopType.Yoyo);
        enemiesHolder.SetText(GameConstants.enemiesPrefix + entities.childCount);

    }

    internal void AddScore(int scoreAmount)
    {
        scoreHolder.rectTransform.DOScale(1.2f, GameConstants.scoreUpdate).SetLoops(2, LoopType.Yoyo);
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

        }
    }
    void OnProcessState()
    {

        switch (currentState)
        {

            case GameState.Start:
                {
                }
                break;
            case GameState.Play:
                {
                    time -= Time.deltaTime;
                    timeBar.fillAmount = time / GameConstants.maxTime;
                    if (time < 0)
                    {
                        GameOver();
                    }
                }
                break;
            case GameState.End:
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
                effectObject.transform.localScale = Vector3.one * GameConstants.bulletHitSize;
                effectObject.GetComponent<BulletHitEffect>().FadeIn();
                break;
            case GameConstants.EffectTypes.ShieldHit:
                effectObject.transform.localScale = Vector3.one * GameConstants.shieldHitSize;
                effectObject.GetComponent<ShieldHitEffect>().FadeIn();
                break;
            case GameConstants.EffectTypes.ShipExplosion:
                effectObject.transform.localScale = Vector3.one * GameConstants.explodeSize;
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

        enemyObject.transform.SetParent(entities);
        enemyObject.SetActive(true);
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
        UpdateEnemies();
        enemiesKilled++;
        if(entities.childCount == 0)
        {
            AddScore(GameConstants.clearSceneScore);
            SpawnEnemies();
        }
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
        AddTime(GameConstants.attackTimeGain);
    }

    public void AddTime(float addTime)
    {
        timeBar.rectTransform.DOScale(1.5f, GameConstants.healthUpdate).SetLoops(2, LoopType.Yoyo);
        if (time + addTime < GameConstants.maxTime)
        {
            time += addTime;
        }
        else
        {
            time = addTime;
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

        if(fraction < 0)
        {
            GameOver();
        }
    }

    public void PlaySound(GameConstants.SoundType sound, float volume = 1.0f)
    {
        switch (sound)
        {
            case GameConstants.SoundType.Explode:
                audioSource.PlayOneShot(explodeSound, volume);
                break;
            case GameConstants.SoundType.Hit:
                audioSource.PlayOneShot(hitSound, volume);
                break;
            case GameConstants.SoundType.Shot:
                audioSource.PlayOneShot(shotSound, volume);
                break;
            case GameConstants.SoundType.ShieldOff:
                audioSource.PlayOneShot(shieldOffSound, volume);
                break;
            case GameConstants.SoundType.ShieldOn:
                audioSource.PlayOneShot(shieldOnSound, volume);
                break;
        }
    }

    // Update is called once per fram/e
    void Update()
    {
        OnProcessState();
    }

    void GameOver()
    {
        SwitchState(GameState.End);
    }

    void SpawnEnemies()
    {
        waveCount++;

        AddTime(GameConstants.waveCompleteTimeGain);

        for(int i =0; i <= GameConstants.EnemyBaseCount + waveCount; i++)
        {
            float chooser = UnityEngine.Random.Range(0f, 1f);
            GameObject enemy = null;
            if(chooser < 0.4f)
            {
                enemy = GetEnemy(GameConstants.EnemyTypes.GunTurret);
            } else if(chooser < 0.75f)
            {
                enemy = GetEnemy(GameConstants.EnemyTypes.ShotTurret);
            }
            else
            {
                enemy = GetEnemy(GameConstants.EnemyTypes.DualTurret);
            }

            enemy.transform.SetParent(entities);
            enemy.transform.position = new Vector3(UnityEngine.Random.Range(-25f, 25f), UnityEngine.Random.Range(-20f, 20f), -1f);
        }
        UpdateEnemies();
           
    }
}
