using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPositionController : MonoBehaviour
{
    public Transform rightController; // 右のコントローラーのTransformをアタッチ
    public Transform gunTransform; // 銃のTransformをアタッチ

    private Vector3 initialControllerPositionOffset; // 初期のコントローラー位置オフセット（銃）
    private Quaternion initialControllerRotationOffset; // 初期のコントローラー回転オフセット（銃）

    private void Start()
    {
        // 初期のコントローラー位置と回転から銃の位置と向きへのオフセットを計算
        initialControllerPositionOffset = gunTransform.position - rightController.position;
        initialControllerRotationOffset = Quaternion.Inverse(rightController.rotation) * gunTransform.rotation;
    }

    private void Update()
    {
        // 右コントローラーの位置と向きに基づいて銃の位置と向きを更新
        Vector3 newGunPosition = rightController.position + initialControllerPositionOffset;
        Quaternion newGunRotation = rightController.rotation * initialControllerRotationOffset;

        gunTransform.position = newGunPosition;
        gunTransform.rotation = newGunRotation;
    }

    // 銃の位置を取得するメソッド
    public Vector3 GetGunPosition()
    {
        return gunTransform.position;
    }

    // 銃の向きを取得するメソッド
    public Quaternion GetGunRotation()
    {
        return gunTransform.rotation;
    }

}
