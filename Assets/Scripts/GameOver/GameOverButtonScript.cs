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
        //���񂾊K���烊�X�^�[�g
        _gameManager.RestartStage();
        gameOverCanvas.GameOverAnimationReset();

    }

    public void FromTheBeginning()
    {
        //���߂���Q�[���X�^�[�g
        _playerStatusManager.OnEnable();
        string sceneName = "StageScene"; 
        SceneManager.LoadScene(sceneName);
    }

    public void Exit()
    {
        //�^�C�g���ɖ߂�
        string sceneName = "TitleScene";
        SceneManager.LoadScene(sceneName);
    }


}
