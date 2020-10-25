using System.Collections;
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
    public SpriteRenderer attackSprite;

    public TurretController attackedEnemy;

    bool canBeHit = true;

    public PlayerState currentState = PlayerState.Idle;
    Vector3 aimStart;
    Vector3 aimEnd;

    Rigidbody2D playerRigidBody;
    GameManager gameManager;
    ParryShield parryShield;
    Camera2DFollow camFollow;
    // Start is called before the first frame update
    void Start()
    {
        health = GameConstants.maxHealth;
        parryShield = GetComponentInChildren<ParryShield>();
        gameManager = FindObjectOfType<GameManager>();
        playerRigidBody = GetComponent<Rigidbody2D>();
        camFollow = FindObjectOfType<Camera2DFollow>();

        parryShield.transform.localScale = Vector3.zero;

        attackSprite.color = new Color(0f, 0f, 0f, 0f);
        gameManager.OnHealthUpdate(health / GameConstants.maxHealth);
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
                    UnsetAttackEnemy();
                    gameManager.SlowMoStop();
                }
                break;
            case PlayerState.Aiming:
                {
                    parryShield.EnableShield();
                    playerRigidBody.angularVelocity = 0;

                    aimTransform.position = Camera.main.ScreenToWorldPoint(new Vector3(aimStart.x, aimStart.y, -GameConstants.cameraDistance));
                    aimTransform.DOScale(new Vector3(GameConstants.aimEndScale, 1f, GameConstants.aimEndScale), GameConstants.aimAppearTime).SetEase(Ease.OutBack).OnComplete(() => {
                        if(currentState == PlayerState.Aiming)
                        {
                            playerRigidBody.velocity = Vector2.zero;
                            gameManager.SlowMoStart();
                        }
                    });
                }
                break;
            case PlayerState.Boosting:
                {
                    gameManager.SlowMoStop();
                    playerRigidBody.angularVelocity = 0;

                    // DO LOOK AT
                    Vector3 direction = (aimEnd - aimStart).normalized;
                    parryShield.BoostAction(direction);
                    transform.DOLookAt(transform.position - direction, GameConstants.playerRotateTime, AxisConstraint.None, transform.up).OnComplete(() => {
                        playerRigidBody.velocity = Vector2.zero;
                        playerRigidBody.AddForce(pushForce * transform.forward);
                        SwitchState(PlayerState.Idle);
                    });
                }
                break;
            case PlayerState.Attacking:
                {
                    attackSprite.color = new Color(0f, 0f, 0f, 1f);
                    parryShield.DisableShield();
                    playerRigidBody.gravityScale = 0;
                    camFollow.ZoomIn();
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
                    playerRigidBody.gravityScale = 1;
                    camFollow.transform.eulerAngles = Vector3.zero;
                    camFollow.ZoomNormal();
                    UnsetAttackEnemy();
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

                    parryShield.transform.LookAt(transform.position - (target - aimTransform.position), transform.up);
                }
                break;
            case PlayerState.Boosting:
                {
                }
                break;
            case PlayerState.Attacking:
                {
                    Vector3 target = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -GameConstants.cameraDistance));
                    // "Do NOT LookAt"
                    aimTransform.LookAt(aimTransform.position - (target - aimTransform.position), transform.up);


                    attackSprite.transform.position = attackedEnemy.transform.position + new Vector3(0f, 0f, -2f);
                    camFollow.transform.LookAt(attackedEnemy.transform);
                }
                break;
        }
    }
    // FSM end region

    GameConstants.AttackDirection GetSwipeDirection()
    {
        Vector3 normalizedSwipe = (aimStart - aimEnd).normalized;

        if (normalizedSwipe.y > 0.75f)
        {
            return GameConstants.AttackDirection.Down;
        }
        else if(normalizedSwipe.y < -0.75f)
        {
            return GameConstants.AttackDirection.Up;
        }
        else if (normalizedSwipe.x < -0.75f)
        {
            return GameConstants.AttackDirection.Right;
        }
        else if (normalizedSwipe.x > 0.75f)
        {
            return GameConstants.AttackDirection.Left;
        }

        return GameConstants.AttackDirection.None;
    }

    void TakeInput()
    {
        switch (currentState)
        {
            case PlayerState.Idle:
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        aimStart = Input.mousePosition;
                        SwitchState(PlayerState.Aiming);
                    }
                }
                break;
            case PlayerState.Aiming:
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        aimEnd = Input.mousePosition;

                        if (attackedEnemy != null && attackedEnemy.GetCurrentAttack() == GetSwipeDirection())
                        {
                            attackedEnemy.RegisterAttack();
                            SwitchState(PlayerState.Attacking);
                            SetAttackDirection(attackedEnemy.GetCurrentAttack());
                        }
                        else if (Vector3.Distance(aimStart, aimEnd) > GameConstants.aimThreshold)
                        {
                            HideAttack();
                            SwitchState(PlayerState.Boosting);
                        }
                        else
                        {
                            HideAttack();
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
                    if (Input.GetMouseButtonDown(0))
                    {
                        AttackBegin();
                    }
                    else if (Input.GetMouseButtonUp(0))
                    {
                        AttackEnd();
                    }
                    break;
                }
        }
    }

    void AttackBegin()
    {
        //attackSprite.color = Color.black;
        aimStart = Input.mousePosition;
        aimTransform.position = Camera.main.ScreenToWorldPoint(new Vector3(aimStart.x, aimStart.y, -GameConstants.cameraDistance));
        aimTransform.DOScale(new Vector3(GameConstants.aimEndScale, 0.5f, GameConstants.aimEndScale), GameConstants.aimAppearTime / 4);
    }

    void AttackEnd()
    {
        aimTransform.DOScale(new Vector3(0f, 0f, 0f), GameConstants.aimAppearTime / 2);

        aimEnd = Input.mousePosition;
        if (GetSwipeDirection() == attackedEnemy.GetCurrentAttack())
        {
            // SuccessAttack
            attackSprite.DOColor(Color.white, GameConstants.aimAppearTime / 6).SetLoops(2, LoopType.Yoyo);
            attackSprite.transform.DOScale(1.1f, GameConstants.aimAppearTime / 6).SetLoops(2, LoopType.Yoyo);

            transform.position = attackedEnemy.transform.position - ((aimStart - aimEnd).normalized * 1.5f);
            //transform.LookAt(attackedEnemy.transform);
            // check Successful attack
            if (attackedEnemy.RegisterAttack())
            {
                SuccessfulAttack();
            } else
            {
                SetAttackDirection(attackedEnemy.GetCurrentAttack());
            }
        }
        else
        {
            // FailAttack
            attackedEnemy.ResetAttackSequence();
            SwitchState(PlayerState.Idle);
        }
    }

    void SuccessfulAttack()
    {
        health += GameConstants.attackHealthGain;
        gameManager.SuccessfulAttack(health/GameConstants.maxHealth);
        SwitchState(PlayerState.Idle);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.CompareTag("Bullet") && canBeHit)
        {
            BulletScript bullet = collision.transform.GetComponent<BulletScript>();
            if(bullet.isEnemyShot)
            {
                health -= bullet.damage;
                gameManager.OnHealthUpdate(health/GameConstants.maxHealth);
                bullet.OnHit();
                //canBeHit = false;
                //StartCoroutine("ResetCanBeHit");
            }
        }
    }

    void SetAttackDirection(GameConstants.AttackDirection dir)
    {
        Vector3 targetEuler = new Vector3(0, 180, 0);
        switch (dir)
        {
            case GameConstants.AttackDirection.Up:
                targetEuler = new Vector3(0, 180, 0);
                break;
            case GameConstants.AttackDirection.Down:
                targetEuler = new Vector3(0, 180, 180);
                break;
            case GameConstants.AttackDirection.Left:
                targetEuler = new Vector3(0, 180, -90);
                break;
            case GameConstants.AttackDirection.Right:
                targetEuler = new Vector3(0, 180, 90);
                break;
        }
        attackSprite.transform.DORotate(targetEuler, GameConstants.attackMove).SetEase(Ease.InOutBack);
    }

    public void ShowAttack()
    {
        attackSprite.DOFade(0.8f, GameConstants.attackShow);
    }

    public void HideAttack()
    {
        attackSprite.DOFade(0.0f, GameConstants.attackShow);
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

    public void SetAttackedEnemy(TurretController enemyRange)
    {
        if (currentState != PlayerState.Aiming)
        {
            return;
        }
        attackedEnemy = enemyRange;
        SetAttackDirection(attackedEnemy.GetCurrentAttack());
        attackSprite.transform.DOMove(attackedEnemy.transform.position + new Vector3(0f, 0f, -2f), GameConstants.attackMove);

        ShowAttack();
    }

    public void UnsetAttackEnemy()
    {
        attackedEnemy = null;
        HideAttack();
    }

    public Vector3 GetPredictedAim()
    {
        return transform.position + new Vector3(playerRigidBody.velocity.x, playerRigidBody.velocity.y, 0);
    }
}
