using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    [SerializeField] float spawnInterval = 3f; // 敵出現間隔

    EnemySpawner[] spawners; // EnemySpawnerのリスト
    float timer = 0f;        // 出現時間判定用のタイマー変数

    // Use this for initialization
    void Start()
    {
        // 子オブジェクトに存在するEnemySpawnerのリストを取得
        spawners = GetComponentsInChildren<EnemySpawner>();
    }

    // Update is called once per frame
    void Update()
    {
        // タイマー更新
        timer += Time.deltaTime;

        // 出現間隔の判定
        if (spawnInterval < timer)
        {
            // ランダムにEnemySpawnerを選択して敵を出現させる
            var index = Random.Range(0, spawners.Length);
            spawners[index].Spawn();

            // タイマーリセット
            timer = 0f;
        }
    }
}