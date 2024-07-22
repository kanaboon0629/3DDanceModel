using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MoveAgent : MonoBehaviour
{
    NavMeshAgent agent;     // ナビメッシュエージェント

    void Start()
    {
        // ナビメッシュエージェントを取得
        agent = GetComponent<NavMeshAgent>();

        // 次の地点へ移動
        GotoNextPoint();
    }

    void Update()
    {
        // 目的地付近に到着したかどうか？
        if (agent.remainingDistance < 0.5f)
        {
            // 次の地点へ移動
            GotoNextPoint();
        }
    }

    void GotoNextPoint()
    {
        // 床の移動地点をランダムで作成
        var nextPoint = new Vector3(Random.Range(-20.0f, 20.0f), 0.0f, Random.Range(-20.0f, 20.0f));

        // ナビメッシュエージェントへ目的地を設定
        agent.SetDestination(nextPoint);
    }
}