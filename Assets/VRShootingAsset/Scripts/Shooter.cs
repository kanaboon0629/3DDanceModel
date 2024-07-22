// using UnityEngine;
// using Oculus;

// public class Shooter : MonoBehaviour
// {
//     [SerializeField] GameObject bulletPrefab;       // 弾のプレハブ
//     [SerializeField] Transform gunBarrelEnd;        // 銃口(弾の発射位置)

//     [SerializeField] ParticleSystem gunParticle;    // 発射時演出
//     [SerializeField] AudioSource gunAudioSource;    // 発射音の音源

//     private void Update()
//     {
//         if (OVRInput.GetDown(OVRInput.Button.One)) // Aボタンが押された場合
//         {
//             Shoot(); // 発射
//         }
//     }

//     // 弾を撃ったときの処理
//     void Shoot()
//     {
//         // プレハブを元に、シーン上に弾を生成
//         GameObject bullet = Instantiate(bulletPrefab, gunBarrelEnd.position, gunBarrelEnd.rotation);

//         // 発射時演出を再生
//         gunParticle.Play();

//         // 発射時の音を再生
//         gunAudioSource.Play();
//     }
// }
