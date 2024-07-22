using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    // 紙面コード
    void Update()
    {
        transform.forward = GameObject.Find("Main Camera").GetComponent<Camera>().transform.forward;
    }

    // 紙面都合上、上記になっていますが、下記コメント部分の書き方の方が処理が軽くなります。
/*
    Camera mainCamera;        // メインカメラの保持

    void Start()
    {
        // メインカメラの取得
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    void Update()
    {
        transform.forward = mainCamera.transform.forward;
    }
*/
}