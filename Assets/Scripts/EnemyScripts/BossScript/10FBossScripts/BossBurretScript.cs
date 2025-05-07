using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBurretScript : MonoBehaviour
{
    private bool _isMove = false;

    private float _moveSpeed = 1f;
    private float _attackPower = 25f; //�^����_���[�W�̋���

    private Vector2 _movement = default;
    private Vector2 _direction = default;

    private const float MAX_SPEED = 100f;
    private const int DELTATIME_MULTIPLE = 10;
    
    private void Update()
    {
        if (_isMove)
        {
            BurretMove();
        }
    }

    private void BurretMove()
    {
        if (_moveSpeed < MAX_SPEED)
        {
            _moveSpeed += Time.deltaTime * DELTATIME_MULTIPLE;

            _movement = _direction * _moveSpeed * Time.deltaTime;
            transform.position += (Vector3)_movement;
        }
        else
        {
            transform.position += (Vector3)_movement;
        }
    }

    public void Attack(GameObject target)
    {
        //�v���C���[�̕���������
        _direction = (target.transform.position - this.transform.position).normalized;

        //�ړ��J�n
        _movement = _direction * _moveSpeed * Time.deltaTime;

        _isMove = true;
        StartCoroutine(AttackEnd());
    }

    public IEnumerator AttackEnd()
    {
        
        int waitTime = 3;
        yield return new WaitForSeconds(waitTime);
        
        //�T�b��������ړ�����߂�
        _movement = default;
        _moveSpeed = 1f;
        _isMove = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string playerTag = "Player";
        if (collision.gameObject.CompareTag(playerTag))
        {
            //�v���C���[�ɓ�����ƃ_���[�W��^����
            collision.gameObject.GetComponent<PlayerAttack>().TakeDamage(_attackPower,this.gameObject,true, false);
        }
    }
}
