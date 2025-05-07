using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClearTriggerScript : MonoBehaviour
{

    [SerializeField] private GameClearStageScript _clearManager = default;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //�v���C���[������������
        string playerTag = "Player";
        if (collision.gameObject.CompareTag(playerTag))
        {
            //�Q�[���N���A������o��
            collision.gameObject.GetComponent<PlayerMoveControll>().ControllOff();
            _clearManager.GameClear();
        }
    }
}
