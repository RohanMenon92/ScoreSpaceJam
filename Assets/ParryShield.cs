using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryShield : MonoBehaviour
{
    List<Transform> affectedBullets = new List<Transform>();
    PlayerScript player;
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponentInParent<PlayerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void EnableShield()
    {
        transform.DOScale(GameConstants.parrySize, GameConstants.shieldAppearTime).SetEase(Ease.InOutBack);
    }

    internal void DisableShield()
    {
        transform.DOScale(0f, GameConstants.shieldAppearTime).SetEase(Ease.OutCubic);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Bullet"))
        {
            if(!affectedBullets.Contains(collision.transform))
            {
                affectedBullets.Add(collision.transform);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(player.currentState != PlayerState.Aiming)
        {
            return;
        }
        if (collision.transform.CompareTag("Enemy"))
        {
            if (player.attackedEnemy == null || Vector3.Distance(player.attackedEnemy.transform.position, player.transform.position) > Vector3.Distance(collision.transform.position, player.transform.position)) {
                player.SetAttackedEnemy(collision.GetComponent<TurretController>());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            if (affectedBullets.Contains(collision.transform))
            {
                affectedBullets.Remove(collision.transform);
            }
        }

        if (collision.transform.CompareTag("Enemy"))
        {
            if (player.attackedEnemy == collision.transform)
            {
                player.UnsetAttackEnemy();
            }
        }

    }

    internal void BoostAction(Vector3 direction)
    {
        foreach(Transform bullet in affectedBullets)
        {
            bullet.GetComponent<BulletScript>().OnParry(-direction);
        }

        DisableShield();
    }
}
