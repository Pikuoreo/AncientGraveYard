using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClearTriggerScript : MonoBehaviour
{

    [SerializeField] private GameClearStageScript _clearManager = default;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //プレイヤーが当たったら
        string playerTag = "Player";
        if (collision.gameObject.CompareTag(playerTag))
        {
            //ゲームクリア判定を出す
            collision.gameObject.GetComponent<PlayerMoveControll>().ControllOff();
            _clearManager.GameClear();
        }
    }
}
