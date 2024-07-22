using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
public class SlideInOut : MonoBehaviour
{
    void Start()
    {
        // rectTranformコンポーネント取得
        var rectTranform = GetComponent<RectTransform>();

        // DOTweenのシーケンスを作成
        var sequence = DOTween.Sequence();

        // 画面右からスライドインさせる
        sequence.Append(rectTranform.DOMoveX(0.0f, 1.0f));

        // 画面左へスライドアウトさせる
        sequence.Append(rectTranform.DOMoveX(-1400.0f, 0.8f));
    }
}