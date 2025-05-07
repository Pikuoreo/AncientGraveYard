using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartPointScript : MonoBehaviour
{
    [SerializeField] private GameProgressScript _gameStartScript = default;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string playerTag = "Player";
        if (collision.gameObject.CompareTag(playerTag))
        {
            _gameStartScript.FadeOut();
        }
    }
}
