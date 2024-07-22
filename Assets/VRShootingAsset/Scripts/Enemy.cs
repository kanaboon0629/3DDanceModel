using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Enemy : MonoBehaviour
{
    [SerializeField] AudioClip spawnClip; // 出現時のAudioClip
    [SerializeField] AudioClip hitClip;   // 弾命中時のAudioClip

    // 倒された際に無効化するためにコライダーとレンダラーを持っておく
    [SerializeField] Collider enemyCollider; // コライダー
    [SerializeField] Renderer enemyRenderer; // レンダラー

    AudioSource audioSource;            // 再生に使用するAudioSource

    [SerializeField] int point = 1;     // 倒したときのスコアポイント
    Score score;                        // スコア

    [SerializeField] int hp = 1;        // 敵のヒットポイント

    [SerializeField] GameObject popupTextPrefab;    // 得点表示用Prefab

    void Start()
    {
        // AudioSourceコンポーネントを取得しておく
        audioSource = GetComponent<AudioSource>();

        // 出現時の音を再生
        audioSource.PlayOneShot(spawnClip);

        // ゲームオブジェクトを検索
        var gameObj = GameObject.FindWithTag("Score");

        // gameObjに含まれるScoreコンポーネントを取得
        score = gameObj.GetComponent<Score>();
    }

    // OnHitBulletメッセージから呼び出されることを想定
    void OnHitBullet()
    {
        // 弾命中時の音を再生
        audioSource.PlayOneShot(hitClip);

        // HP 減算
        --hp;

        // HPが0になったら死亡
        if (hp <= 0)
        {
            // 死亡時処理
            GoDown();
        }
    }

    // 死亡時処理
    void GoDown()
    {
        // スコアを加算
        score.AddScore(point);

        // ポップアップテキストの作成
        CreatePopupText();

        // 当たり判定と表示を消す
        enemyCollider.enabled = false;
        enemyRenderer.enabled = false;

        // 自身のゲームオブジェクトを一定時間後に破棄
        Destroy(gameObject, 1f);
    }

    // ポップアップテキストの作成
    void CreatePopupText()
    {
        // ポップアップテキストのインスタンス作成
        var text = Instantiate(popupTextPrefab, transform.position, Quaternion.identity);

        // ポップアップテキストのテキスト変更
        text.GetComponent<TextMesh>().text = string.Format("+{0}", point);
    }
}