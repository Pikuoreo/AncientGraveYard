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
    //�Q�[���I�[�o�[������i�K��
    public void StartGameOverAnimation()
    {
        //��ʂ�^���Âɂ���
        string _animationName = "isBlackOut";
        _panelAnimation.SetBool(_animationName, true);
    }

    //�Q�[���I�[�o�[������i�K��
    public void StartSecondGameOverAnimation()
    {
        //BGM����������悤�ɂ���
        _bgmScript.BGMOn();
        //�Q�[���I�[�o�[��BGM �𗬂�
        _bgmScript.ChangeGameOverBGM();

        //�Q�[���I�[�o�[�p�̃X�e�[�W���o��
        _gameManager.GameOver();
        //�e�L�X�g�̕\��
        string _animationName = "isTextAppearance";
        _textAnimation.SetBool(_animationName, true);
    }

    //�Q�[���I�[�o�[�����O�i�K��
    public void StartThirdGameOverAnimation()
    {
        //�e�L�X�g�AReTry�{�^���AEXIT�{�^���̕\��
        _ReTryTextObject.SetActive(true);
    }

    public void GameOverAnimationReset()
    {
        string _animationName = "isBlackOut";
        _panelAnimation.SetBool(_animationName, false);

        //�e�L�X�g�̍폜
         _animationName = "isTextAppearance";
        _textAnimation.SetBool(_animationName, false);

        //�e�L�X�g�AReTry�{�^���AEXIT�{�^��������
         _ReTryTextObject.SetActive(false);
    }
}
