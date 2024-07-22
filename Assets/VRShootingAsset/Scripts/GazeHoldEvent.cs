using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GazeHoldEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] float gazeTapTime = 2.0f;      // ボタンをタップする時間
    [SerializeField] UnityEvent onGazeHold;         // ボタンをタップしたときのイベント

    float timer;    // ポインターがUI領域上にある時間
    bool isHover;   // ポインターがUI領域上にあるか？

    // ポインターがUI領域に入った時のイベント処理
    public void OnPointerEnter(PointerEventData eventData)
    {
        // タイマーを0に
        timer = 0.0f;

        // Hover状態へ
        isHover = true;
    }

    // ポインターがUI領域から出た時のイベント処理
    public void OnPointerExit(PointerEventData eventData)
    {
        // Hover状態解除
        isHover = false;
    }

    public void Update()
    {
        // Hover状態でなければ処理を行わない
        if (!isHover)
        {
            return;
        }

        // 経過時間
        timer += Time.deltaTime;

        // 指定の時間以上たった場合
        if (gazeTapTime < timer)
        {
            // イベント実行
            onGazeHold.Invoke();

            // Hover状態解除
            isHover = false;
        }
    }
}