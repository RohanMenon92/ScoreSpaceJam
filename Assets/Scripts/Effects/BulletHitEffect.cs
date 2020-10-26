﻿using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BulletHitEffect : MonoBehaviour
{
    VisualEffect visualEffect;
    Sequence effectSequence;
    // Start is called before the first frame update

    void Start()
    {
        visualEffect = this.GetComponent<VisualEffect>();
    }

    public void FadeIn()
    {
        if (visualEffect == null)
        {
            visualEffect = this.GetComponent<VisualEffect>();
        }

        visualEffect.Stop();
        visualEffect.SetFloat("Rate", 0.5f);
        effectSequence = DOTween.Sequence();
        effectSequence.Insert(0f, DOTween.To(() => visualEffect.GetFloat("Rate"), x => visualEffect.SetFloat("Rate", x), 1.0f, GameConstants.bulletHitLife * 0.2f));
        effectSequence.Append(DOTween.To(() => visualEffect.GetFloat("Rate"), x => visualEffect.SetFloat("Rate", x), 0f, GameConstants.bulletHitLife * 0.2f).SetDelay(GameConstants.bulletHitLife * 0.6f));
        effectSequence.AppendInterval(GameConstants.bulletHitLife);
        effectSequence.OnComplete(() =>
        {
            FadeAway();
        });
        visualEffect.Play();
        effectSequence.Play();
    }

    void FadeAway()
    {
        FindObjectOfType<GameManager>().ReturnEffectToPool(gameObject, GameConstants.EffectTypes.BulletHit);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
