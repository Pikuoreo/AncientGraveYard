using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossTentacleScript : MonoBehaviour
{
    private Animator _leaveAnime = default;

    private BoxCollider2D _thisCollider = default;

    private SpriteRenderer _thisSprite = default;

    private float _attackPower = 30f;//�U����

    private bool _isAttack = false;//ture�ōU���J�n

    private PlayerAttack _playerAttackScript = default;
    private void Start()
    {
        _leaveAnime =  this.GetComponent<Animator>();
        _thisCollider = this.GetComponent<BoxCollider2D>();
        _thisSprite = this.GetComponent<SpriteRenderer>();
    }
    public void EndAnimation()
    {
        //�����ځA�����蔻���off�ɂ���
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

            //�擾�͈�x����
            if (_playerAttackScript == null)
            {
                _playerAttackScript = collision.gameObject.GetComponent<PlayerAttack>();

                _playerAttackScript.TakeDamage(_attackPower, this.gameObject, false, false);
            
            }
            else
            {
                //�v���C���[�Ƀ_���[�W��^����
               _playerAttackScript.TakeDamage(_attackPower, this.gameObject, false, false);
            }
            StartCoroutine(AttackCooldown());
        }
    }

    private IEnumerator AttackCooldown()
    {
        //��x�U��������A�P�D�Q�b�ԍU�����Ȃ��Ȃ�
        float waitTime = 1.2f;

        yield return new WaitForSeconds(waitTime);
        _isAttack = false;
    }
}
