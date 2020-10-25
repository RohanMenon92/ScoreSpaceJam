﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum PlayerState
{
    Idle,
    Aiming,
    Boosting,
    Attacking
}

public class PlayerScript : MonoBehaviour
{
    public Transform aimTransform;
    public float pushForce = 25f;
    public float health = GameConstants.maxHealth;

    bool canBeHit = true;

    PlayerState currentState = PlayerState.Idle;
    Vector3 aimStart;
    Vector3 aimEnd;

    Rigidbody2D playerRigidBody;
    GameManager gameManager;
    ParryShield parryShield;
    // Start is called before the first frame update
    void Start()
    {
        health = GameConstants.maxHealth;
        parryShield = GetComponentInChildren<ParryShield>();
        gameManager = FindObjectOfType<GameManager>();
        playerRigidBody = GetComponent<Rigidbody2D>();

        parryShield.transform.localScale = Vector3.zero;
    }

    // FSM region

    public void WaitAndSwitchState(PlayerState newState, float delay)
    {
        StartCoroutine(WaitStateSwitch(newState, delay));
    }

    public void SwitchState(PlayerState newState)
    {
        bool switchAllowed = false;

        // Do check for if switch is possible
        switch (currentState)
        {
            case PlayerState.Idle:
                {
                    switchAllowed = newState == PlayerState.Aiming || newState == PlayerState.Boosting;
                }
                break;
            case PlayerState.Aiming:
                {
                    switchAllowed = newState == PlayerState.Boosting || newState == PlayerState.Attacking || newState == PlayerState.Idle;
                }
                break;
            case PlayerState.Boosting:
                {
                    switchAllowed = newState == PlayerState.Idle;
                }
                break;
            case PlayerState.Attacking:
                {
                    switchAllowed = newState == PlayerState.Idle;
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
    void OnEnterState(PlayerState stateEnter)
    {
        switch (stateEnter)
        {
            case PlayerState.Idle:
                {
                    gameManager.SlowMoStop();
                }
                break;
            case PlayerState.Aiming:
                {
                    parryShield.EnableShield();
                    playerRigidBody.velocity = Vector2.zero;

                    aimTransform.position = Camera.main.ScreenToWorldPoint(new Vector3(aimStart.x, aimStart.y, -GameConstants.cameraDistance));
                    aimTransform.DOScale(new Vector3(GameConstants.aimEndScale, 1f, GameConstants.aimEndScale), GameConstants.aimAppearTime).SetEase(Ease.OutBack).OnComplete(() => {
                        if(currentState == PlayerState.Aiming)
                        {
                            gameManager.SlowMoStart();
                        }
                        playerRigidBody.angularVelocity = 0;
                    });
                }
                break;
            case PlayerState.Boosting:
                {
                    gameManager.SlowMoStop();

                    // DO LOOK AT
                    Vector3 direction = (aimEnd - aimStart).normalized;
                    parryShield.BoostAction(direction);
                    transform.DOLookAt(transform.position - direction, GameConstants.playerRotateTime, AxisConstraint.None, transform.up).OnComplete(() => {
                        playerRigidBody.AddForce(pushForce * transform.forward);
                        SwitchState(PlayerState.Idle);
                    });
                }
                break;
            case PlayerState.Attacking:
                {
                }
                break;
        }
    }

    IEnumerator WaitStateSwitch(PlayerState newState, float delay)
    {
        yield return new WaitForSeconds(delay);

        SwitchState(newState);
    }

    void OnExitState(PlayerState stateExit)
    {
        switch (stateExit)
        {
            case PlayerState.Idle:
                {
                }
                break;
            case PlayerState.Aiming:
                {
                    aimTransform.DOScale(new Vector3(0f, 0f, 0f), GameConstants.aimAppearTime).SetEase(Ease.InOutBack);
                }
                break;
            case PlayerState.Boosting:
                {
                }
                break;
            case PlayerState.Attacking:
                {
                }
                break;
        }
    }
    void OnProcessState()
    {
        switch (currentState)
        {
            case PlayerState.Idle:
                {
                }
                break;
            case PlayerState.Aiming:
                {
                    Vector3 target = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -GameConstants.cameraDistance));
                    // "Do NOT LookAt"
                    aimTransform.LookAt(aimTransform.position - (target - aimTransform.position), transform.up);
                }
                break;
            case PlayerState.Boosting:
                {
                }
                break;
            case PlayerState.Attacking:
                {
                }
                break;
        }
    }
    // FSM end region

    void TakeInput()
    {
        switch (currentState)
        {
            case PlayerState.Idle:
                {
                    if(Input.GetMouseButtonDown(0))
                    {
                        aimStart = Input.mousePosition;
                        SwitchState(PlayerState.Aiming);
                    }
                }
                break;
            case PlayerState.Aiming:
                {
                    if(Input.GetMouseButtonUp(0))
                    {
                        aimEnd = Input.mousePosition;
                        if(Vector3.Distance(aimStart, aimEnd) > GameConstants.aimThreshold)
                        {
                            SwitchState(PlayerState.Boosting);
                        } else
                        {
                            parryShield.DisableShield();
                            SwitchState(PlayerState.Idle);
                        }
                    }
                }
                break;
            case PlayerState.Boosting:
                {
                }
                break;
            case PlayerState.Attacking:
                {
                }
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.CompareTag("Bullet") && canBeHit)
        {
            BulletScript bullet = collision.transform.GetComponent<BulletScript>();
            if(bullet.isEnemyShot)
            {
                health -= bullet.damage;
                bullet.OnHit();
                //canBeHit = false;
                //StartCoroutine("ResetCanBeHit");
            }
        }
    }

    IEnumerator ResetCanBeHit()
    {
        yield return new WaitForSeconds(GameConstants.invulnerabilityDuration);
        canBeHit = true;
    }

    // Update is called once per frame
    void Update()
    {
        OnProcessState();
        TakeInput();
    }

    public Vector3 GetPredictedAim()
    {
        return transform.position + new Vector3(playerRigidBody.velocity.x, playerRigidBody.velocity.y, 0);
    }
}
