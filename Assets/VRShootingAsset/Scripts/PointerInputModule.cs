using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;
using System.Linq;

public class PointerInputModule : BaseInputModule
{
    RaycastResultComparer comparer = new RaycastResultComparer();   // RaycastResultデータの比較処理
    PointerEventData pointerData;       // ポインタ用のイベントデータ
    List<RaycastResult> resultList;     // Raycast結果
    Vector2 viewportCenter;             // 画面中心位置
    Vector2 pointerPosition;
   [SerializeField] GameObject pointer;   // Pointer参照 ゲームオブジェクト参照

    // RaycastResultデータの比較処理クラス
    class RaycastResultComparer : EqualityComparer<RaycastResult>
    {
        public override bool Equals(RaycastResult a, RaycastResult b)
        {
            return a.gameObject == b.gameObject;
        }

        public override int GetHashCode(RaycastResult r)
        {
            return r.gameObject.GetHashCode();
        }
    }

    protected override void Start()
    {
        // イベントデータの作成
        pointerData = new PointerEventData(eventSystem);
        // 画面の中心位置を設定
        viewportCenter = GetViewportCenter();
        pointerPosition = ConvertTransformToVector2(pointer.transform);
    }

    public override void Process()
    {
        // Raycastの結果データ
        resultList = new List<RaycastResult>();

        // 画面センター位置設定
        pointerData.Reset();
        pointerData.position = viewportCenter;
        //pointerData.position = pointerPosition;

        // カメラからポインタに向けてRaycastを行う
        eventSystem.RaycastAll(pointerData, resultList);

        // ポインタがこのフレームでUIの領域にはいったものを抜き出してリスト化する
        var enterList = resultList.Except<RaycastResult>(m_RaycastResultCache, comparer);
        // 対象のUIに対してPointerEnterイベントを実行
        foreach (var r in enterList)
        {
            ExecuteEvents.Execute(r.gameObject, pointerData, ExecuteEvents.pointerEnterHandler);
        }

        // ポインタがこのフレームでUIの領域から出たものを抜き出してリスト化する
        var exitList = m_RaycastResultCache.Except<RaycastResult>(resultList, comparer);
        // 対象のUIに対してPointerExitイベントを実行
        foreach (var r in exitList)
        {
            ExecuteEvents.Execute(r.gameObject, pointerData, ExecuteEvents.pointerExitHandler);
        }

        // 今回の結果を保存
        m_RaycastResultCache = resultList;
    }

    // 画面の中心位置を計算
    public Vector2 GetViewportCenter()
    {
        // 画面のサイズ
        var viewportWidth = Screen.width;
        var viewportHeight = Screen.height;

        // VRで見ているとき
        if (XRSettings.enabled)
        {
            // 表示用テクスチャーのサイズ
            viewportWidth = XRSettings.eyeTextureWidth;
            viewportHeight = XRSettings.eyeTextureHeight;
        }

        // XYサイズの半分が、画面の中心位置
        return new Vector2(viewportWidth * 0.5f, viewportHeight * 0.5f);
    }

    Vector2 ConvertTransformToVector2(Transform transform)
    {
        // ゲームオブジェクトの位置情報を取得
        Vector3 position = transform.position;

        // 2DのVector2に変換
        Vector2 vector2Position = new Vector2(position.x, position.y);

        return vector2Position;
    }
}
/*
// Unity 2017.2以前のソースコード
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.VR;
using System.Linq;

public class PointerInputModule : BaseInputModule
{
    RaycastResultComparer comparer = new RaycastResultComparer();   // RaycastResultデータの比較処理
    PointerEventData pointerData;       // ポインタ用のイベントデータ
    List<RaycastResult> resultList;     // Raycast結果
    Vector2 viewportCenter;             // 画面中心位置

    // RaycastResultデータの比較処理クラス
    class RaycastResultComparer : EqualityComparer<RaycastResult>
    {
        public override bool Equals(RaycastResult a, RaycastResult b)
        {
            return a.gameObject == b.gameObject;
        }

        public override int GetHashCode(RaycastResult r)
        {
            return r.gameObject.GetHashCode();
        }
    }

    protected override void Start()
    {
        // イベントデータの作成
        pointerData = new PointerEventData(eventSystem);
        // 画面の中心位置を設定
        viewportCenter = GetViewportCenter();
    }

    public override void Process()
    {
        // Raycastの結果データ
        resultList = new List<RaycastResult>();

        // 画面センター位置設定
        pointerData.Reset();
        pointerData.position = viewportCenter;

        // カメラからポインタに向けてRaycastを行う
        eventSystem.RaycastAll(pointerData, resultList);

        // ポインタがこのフレームでUIの領域にはいったものを抜き出してリスト化する
        var enterList = resultList.Except<RaycastResult>(m_RaycastResultCache, comparer);
        // 対象のUIに対してPointerEnterイベントを実行
        foreach (var r in enterList)
        {
            ExecuteEvents.Execute(r.gameObject, pointerData, ExecuteEvents.pointerEnterHandler);
        }

        // ポインタがこのフレームでUIの領域から出たものを抜き出してリスト化する
        var exitList = m_RaycastResultCache.Except<RaycastResult>(resultList, comparer);
        // 対象のUIに対してPointerExitイベントを実行
        foreach (var r in exitList)
        {
            ExecuteEvents.Execute(r.gameObject, pointerData, ExecuteEvents.pointerExitHandler);
        }

        // 今回の結果を保存
        m_RaycastResultCache = resultList;
    }

    // 画面の中心位置を計算
    public Vector2 GetViewportCenter()
    {
        // 画面のサイズ
        var viewportWidth = Screen.width;
        var viewportHeight = Screen.height;

        // VRで見ているとき
        if (VRSettings.enabled)
        {
            // 表示用テクスチャーのサイズ
            viewportWidth = VRSettings.eyeTextureWidth;
            viewportHeight = VRSettings.eyeTextureHeight;
        }

        // XYサイズの半分が、画面の中心位置
        return new Vector2(viewportWidth * 0.5f, viewportHeight * 0.5f);
    }
}     
*/