using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasGroupFade : MonoBehaviour
{
    void Start()
    {
        // CanvasGroupの取得
        var canvasGroup = GetComponent<CanvasGroup>();

        // CanvasGroupをFade アニメーションをさせる
        canvasGroup.DOFade(1.0f, 1.0f).SetEase(Ease.InOutQuart).SetLoops(2, LoopType.Yoyo);
    }
}