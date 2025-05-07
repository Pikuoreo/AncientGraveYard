using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearScreenText : MonoBehaviour
{
    [SerializeField] private GameObject _nextScreenAnimation = default;//次に実行するアニメーションのオブジェクト

    [SerializeField] private GameObject _reTryButton = default;//もう一度挑戦のボタン
    
    [SerializeField] private GameObject _exitButton = default;//タイトルへ戻るボタン

    [SerializeField] private AudioClip _clearSe = default;
    // Start is called before the first frame update
    public void NextAnimation()
    {
        if (_clearSe != null)
        {
            //クリアSeを流すオブジェクトなら、クリアSeを流す
            this.GetComponent<AudioSource>().PlayOneShot(_clearSe);
        }
        //次のアニメーションを流す
        _nextScreenAnimation.GetComponent<Animator>().enabled = true;
    }

    public void SelectButtonDisplay()
    {
        //選択ボタンを出す
        _reTryButton.SetActive(true);
        _exitButton.SetActive(true);
    }
}
