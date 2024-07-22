using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [SerializeField] float lifetime = 5f; // ゲームオブジェクトの寿命

    // Use this for initialization
    void Start()
    {
        // 一定時間経過後にゲームオブジェクトを破棄する
        Destroy(gameObject, lifetime);
    }
}