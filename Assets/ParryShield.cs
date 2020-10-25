using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryShield : MonoBehaviour
{
    List<Transform> affectedBullets = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void EnableShield()
    {
        transform.DOScale(5f, GameConstants.shieldAppearTime).SetEase(Ease.InOutBack);
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

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            if (affectedBullets.Contains(collision.transform))
            {
                affectedBullets.Remove(collision.transform);
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
