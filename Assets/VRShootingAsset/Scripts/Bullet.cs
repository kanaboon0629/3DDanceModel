using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField] float speed = 20f; // 弾速 [m/s]

    [SerializeField] ParticleSystem hitParticlePrefab; // 着弾時演出プレハブ

    // Use this for initialization
    void Start()
    {
        // ゲームオブジェクト前方向の速度ベクトルを計算
        var velocity = speed * transform.forward;

        // Rigidbodyコンポーネントを取得
        var rigidbody = GetComponent<Rigidbody>();

        // Rigidbodyコンポーネントを使って初速を与える
        rigidbody.AddForce(velocity, ForceMode.VelocityChange);
    }

    // トリガー領域進入時に呼び出される
    void OnTriggerEnter(Collider other)
    {
        // 衝突対象に"OnHitBullet"メッセージ
        other.SendMessage("OnHitBullet");

        // 着弾地点に演出自動再生のゲームオブジェクトを生成
        Instantiate(hitParticlePrefab, transform.position, transform.rotation);

        // 自身のゲームオブジェクトを破棄
        Destroy(gameObject);
    }
}