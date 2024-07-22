// using UnityEngine;
// using UnityEngine.Events;
// using UnityEngine.EventSystems;
// using Oculus;

// public class ClickEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
// {
//     [SerializeField] UnityEvent Clicked;         // ボタンをタップしたときのイベント

//     float timer;    // ポインターがUI領域上にある時間
//     bool isHover;   // ポインターがUI領域上にあるか？

//     // ポインターがUI領域に入った時のイベント処理
//     public void OnPointerEnter(PointerEventData eventData)
//     {
//         // Hover状態へ
//         isHover = true;
//     }

//     // ポインターがUI領域から出た時のイベント処理
//     public void OnPointerExit(PointerEventData eventData)
//     {
//         // Hover状態解除
//         isHover = false;
//     }

//     public void Update()
//     {
//         // Hover状態でなければ処理を行わない
//         if (!isHover)
//         {
//             return;
//         }

//         if (OVRInput.GetDown(OVRInput.Button.One)) // Aボタンが押された場合
//         {
//             // イベント実行
//             Clicked.Invoke();

//             // Hover状態解除
//             isHover = false;
//         }
//     }
// }