using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Effect : MonoBehaviour
{
    public SpriteRenderer effectSpriteRenderer;
    public float timeToScale = 0.5f;
    public float timeToWait = 0.5f;

    private void Start()
    {
        transform.localScale = Vector3.zero;
        Animate();
    }

    private void Animate()
    {
        transform.DOScale(Vector3.one, timeToScale).SetEase(Ease.InExpo).OnComplete(() =>
        {
            StartCoroutine(Wait());
        });
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(timeToWait);

        transform.DOScale(Vector3.zero, timeToScale).SetEase(Ease.OutExpo).OnComplete(() => Destroy(gameObject));
    }
}
