using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverButtonScript : MonoBehaviour
{
    [SerializeField] private GameProgressScript _gameManager = default;

    [SerializeField]private GameOverScript gameOverCanvas=default;

    [SerializeField] private PlayerStatusManegement _playerStatusManager = default;
   public void ReTry()
    {
        //死んだ階からリスタート
        _gameManager.RestartStage();
        gameOverCanvas.GameOverAnimationReset();

    }

    public void FromTheBeginning()
    {
        //初めからゲームスタート
        _playerStatusManager.OnEnable();
        string sceneName = "StageScene"; 
        SceneManager.LoadScene(sceneName);
    }

    public void Exit()
    {
        //タイトルに戻る
        string sceneName = "TitleScene";
        SceneManager.LoadScene(sceneName);
    }


}
