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

    PlayerState currentState = PlayerState.Idle;
    Vector3 aimStart;
    Vector3 aimEnd;

    Rigidbody2D playerRigidBody;

    // Start is called before the first frame update
    void Start()
    {
        playerRigidBody = GetComponent<Rigidbody2D>();
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
                }
                break;
            case PlayerState.Aiming:
                {
                    aimTransform.DOScale(new Vector3(GameConstants.aimEndScale, 1f, GameConstants.aimEndScale), GameConstants.aimAppearTime).SetEase(Ease.OutBack);
                }
                break;
            case PlayerState.Boosting:
                {
                    // DO LOOK AT
                    transform.DOLookAt(transform.position - (aimEnd - aimStart).normalized, GameConstants.playerRotateTime, AxisConstraint.None, transform.up).OnComplete(() => {
                        playerRigidBody.velocity = Vector2.zero;
                        playerRigidBody.angularVelocity = 0;
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
                    aimTransform.DOScale(new Vector3(0.5f, 1f, 0.5f), GameConstants.aimAppearTime).SetEase(Ease.InOutBack);
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
                    aimTransform.LookAt(transform.position - (Input.mousePosition - aimStart).normalized, transform.up);
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
