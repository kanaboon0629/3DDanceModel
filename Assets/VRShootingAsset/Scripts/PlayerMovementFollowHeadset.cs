using UnityEngine;

public class PlayerMovementFollowHeadset : MonoBehaviour
{
    public Transform headsetTransform; // ヘッドセットの位置と回転を取得するためのTransform
    //public float moveSpeed = 3.0f; // プレイヤーの移動速度

    private void Update()
    {
        // ヘッドセットの回転情報を取得
        Quaternion headsetRotation = headsetTransform.rotation;

        // ヘッドセットの回転をプレイヤーの移動に適用
        Vector3 moveDirection = headsetRotation * Vector3.forward;

        // プレイヤーの移動
        //transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }
}
