using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class RemainTimer : MonoBehaviour
{
    [SerializeField] float gameTime = 30.0f;        // ゲーム制限時間 [s]
    Text uiText;                                    // UIText コンポーネント
    float currentTime;                              // 残り時間タイマー

    void Start()
    {
        // Textコンポーネント取得
        uiText = GetComponent<Text>();
        // 残り時間を設定
        currentTime = gameTime;
    }

    void Update()
    {
        // 残り時間を計算
        currentTime -= Time.deltaTime;

        // 0秒以下にはならない
        if (currentTime <= 0.0f)
        {
            currentTime = 0.0f;
        }

        // 残り時間テキスト更新
        uiText.text = string.Format("残り時間 : {0:F} 秒", currentTime);
    }

    // カウントダウンを行っているか？
    public bool IsCountingDown()
    {
        // カウンターが0でなければ、カウント中
        return currentTime > 0.0f;
    }
}