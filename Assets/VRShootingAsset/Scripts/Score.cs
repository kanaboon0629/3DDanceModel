using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class Score : MonoBehaviour
{
    Text uiText;                                // UIText コンポーネント

    public int Points { get; private set; }     // 現在のスコアポイント

    void Start()
    {
        uiText = GetComponent<Text>();
    }

    public void AddScore(int addPoint)
    {
        // 現在のポイントに加算
        Points += addPoint;

        // 得点の更新
        uiText.text = string.Format("得点：{0:D3}点", Points);
    }
}