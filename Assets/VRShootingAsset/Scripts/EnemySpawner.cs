using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] Enemy[] enemyPrefabs; // 出現させる敵のプレハブ

    Enemy enemy; // 出現中の敵を保持

    // 敵を発生させる
    public void Spawn()
    {
        // 出現中でなければ敵を出現させる
        if (enemy == null)
        {
            // 登録されている敵のPrefabから1つをランダムで選ぶ
            var index = Random.Range(0, enemyPrefabs.Length);

            // 選んだ敵のインスタンスを作成
            enemy = Instantiate(enemyPrefabs[index], transform.position, transform.rotation);
        }
    }
}