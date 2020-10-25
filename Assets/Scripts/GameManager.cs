using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public Transform worldBullets;

    public int score;
    public int time;

    GameState currentState;

    // Start is called before the first frame update
    void Start()
    {
        // reset score
        score = 0;
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

    // Returning Normal Bullet to pool
    public void ReturnBulletToPool(GameObject bulletToStore, GameConstants.GunTypes bulletType)
    {
        if (bulletType == GameConstants.GunTypes.Machine)
        {
            // Return to normal bullet pool
            bulletToStore.transform.SetParent(unusedBulletPool);
        }
        else if (bulletType == GameConstants.GunTypes.Shot)
        {
            // Return to shotgun bullet pool
            bulletToStore.transform.SetParent(unusedShotgunBulletPool);
        }
        else if (bulletType == GameConstants.GunTypes.Grenade)
        {
            // Return to laser bullet pool
            bulletToStore.transform.SetParent(unusedGrenadeBulletPool);
        }
        bulletToStore.gameObject.SetActive(false);
        bulletToStore.transform.eulerAngles = Vector3.zero;
        bulletToStore.transform.position = Vector3.zero;
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


    // Update is called once per frame
    void Update()
    {
        
    }
}
