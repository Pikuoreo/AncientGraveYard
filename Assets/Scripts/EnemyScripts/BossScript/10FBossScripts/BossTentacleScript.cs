using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossTentacleScript : MonoBehaviour
{
    private Animator _leaveAnime = default;

    private BoxCollider2D _thisCollider = default;

    private SpriteRenderer _thisSprite = default;

    private float _attackPower = 30f;//攻撃力

    private bool _isAttack = false;//tureで攻撃開始

    private PlayerAttack _playerAttackScript = default;
    private void Start()
    {
        _leaveAnime =  this.GetComponent<Animator>();
        _thisCollider = this.GetComponent<BoxCollider2D>();
        _thisSprite = this.GetComponent<SpriteRenderer>();
    }
    public void EndAnimation()
    {
        //見た目、当たり判定をoffにする
        _leaveAnime.SetBool("isLeave", false);
        _thisSprite.enabled = false;
        _thisCollider.enabled = false;
    }
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        string _playertag = "Player";

        if (!_isAttack&&collision.gameObject.CompareTag(_playertag))
        {
            _isAttack = true;

            //取得は一度だけ
            if (_playerAttackScript == null)
            {
                _playerAttackScript = collision.gameObject.GetComponent<PlayerAttack>();

                _playerAttackScript.TakeDamage(_attackPower, this.gameObject, false, false);
            
            }
            else
            {
                //プレイヤーにダメージを与える
               _playerAttackScript.TakeDamage(_attackPower, this.gameObject, false, false);
            }
            StartCoroutine(AttackCooldown());
        }
    }

    private IEnumerator AttackCooldown()
    {
        //一度攻撃したら、１．２秒間攻撃しなくなる
        float waitTime = 1.2f;

        yield return new WaitForSeconds(waitTime);
        _isAttack = false;
    }
}
