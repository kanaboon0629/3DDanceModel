using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        // 指定されたシーンのロードする
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        // アプリケーションの終了
        Application.Quit();
    }

    public void ReloadScene()
    {
        // 現在のシーンを取得
        var scene = SceneManager.GetActiveScene();

        // 現在のシーンを再ロードする
        SceneManager.LoadScene(scene.name);
    }
}