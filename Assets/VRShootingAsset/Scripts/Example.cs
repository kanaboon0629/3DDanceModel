using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Example : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        // 最初に一度メッセージを表示する
        Debug.Log("[Start]");
    }

    // Update is called once per frame
    void Update()
    {
        // Spaceキーが押されている間メッセージを表示する
        if (Input.GetKey(KeyCode.Space))
        {
            Debug.Log("[Update] Space key pressed");
        }
    }
}
