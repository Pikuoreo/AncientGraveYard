using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScript : MonoBehaviour
{

    public void GameStart()
    {
        //ゲームスタート
        string sceneName = "StageScene";
        SceneManager.LoadScene(sceneName);
    }

    public void Exit()
    {
        //ビルド状態の時、unityを閉じる
        Application.Quit();
    }
}
