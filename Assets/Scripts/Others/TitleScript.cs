using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScript : MonoBehaviour
{

    public void GameStart()
    {
        //�Q�[���X�^�[�g
        string sceneName = "StageScene";
        SceneManager.LoadScene(sceneName);
    }

    public void Exit()
    {
        //�r���h��Ԃ̎��Aunity�����
        Application.Quit();
    }
}
