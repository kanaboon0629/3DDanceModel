using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ResultScore : MonoBehaviour
{
    void Start()
    {
        // ゲームオブジェクトを検索
        var gameObj = GameObject.FindWithTag("Score");

        // gameObjに含まれるScoreコンポーネントを取得
        var score = gameObj.GetComponent<Score>();

        // Text コンポーネントの取得
        var uiText = GetComponent<Text>();

        // 得点の更新
        uiText.text = string.Format("得点：{0:D3}点", score.Points);
    }
}