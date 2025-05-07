using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverScript : MonoBehaviour
{
    [SerializeField] Animator _panelAnimation = default;
    [SerializeField] Animator _textAnimation = default;

    [SerializeField] GameObject _ReTryTextObject = default;

    [SerializeField] private GameProgressScript _gameManager = default;

    [SerializeField] private BGMChangeScript _bgmScript = default;
    //ゲームオーバー処理一段階目
    public void StartGameOverAnimation()
    {
        //画面を真っ暗にする
        string _animationName = "isBlackOut";
        _panelAnimation.SetBool(_animationName, true);
    }

    //ゲームオーバー処理二段階目
    public void StartSecondGameOverAnimation()
    {
        //BGMが聞こえるようにする
        _bgmScript.BGMOn();
        //ゲームオーバーのBGM を流す
        _bgmScript.ChangeGameOverBGM();

        //ゲームオーバー用のステージを出す
        _gameManager.GameOver();
        //テキストの表示
        string _animationName = "isTextAppearance";
        _textAnimation.SetBool(_animationName, true);
    }

    //ゲームオーバー処理三段階目
    public void StartThirdGameOverAnimation()
    {
        //テキスト、ReTryボタン、EXITボタンの表示
        _ReTryTextObject.SetActive(true);
    }

    public void GameOverAnimationReset()
    {
        string _animationName = "isBlackOut";
        _panelAnimation.SetBool(_animationName, false);

        //テキストの削除
         _animationName = "isTextAppearance";
        _textAnimation.SetBool(_animationName, false);

        //テキスト、ReTryボタン、EXITボタンを消す
         _ReTryTextObject.SetActive(false);
    }
}
