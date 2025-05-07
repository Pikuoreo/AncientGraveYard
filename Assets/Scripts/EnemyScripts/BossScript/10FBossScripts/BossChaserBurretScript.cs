using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossChaserBurretScript : MonoBehaviour
{
    private GameObject _chaserTarget = default;//�ǔ�����^�[�Q�b�g

    private bool _isChaseStart = false;//true�Œǔ��J�n
    private bool _isAttack = false;//true�ōU���J�n

    private float _moveSpeed = 0.009f;//�ړ��̑���

    private int _attackPower = 20;//�U����

    private const int TIME_DELTATIME = 500;

    private Animator _burretAnimation = default;

    private PlayerAttack _playerAttackScript = default;
    // Start is called before the first frame update
    void Start()
    {
        string findtag = "Player";
        _chaserTarget = GameObject.FindGameObjectWithTag(findtag);
        _burretAnimation = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isChaseStart)
        {
            Chase();
        }
        
    }

   private void Chase()
    {

        Vector2 targetDistance = (_chaserTarget.transform.position - this.transform.position).normalized;

        Vector2 movement = targetDistance * _moveSpeed * Time.deltaTime * TIME_DELTATIME;

        this.transform.position += (Vector3)movement;
    }

    public IEnumerator AttackStart()
    {
        //�P�b�҂��čU���J�n
        float _animationTime = 1f;
        yield return new WaitForSeconds(_animationTime);
        _isChaseStart = true;

        //�T�b�҂��Ēe�����X�ɏ����Ă���
        float AttackEndTime = 5f;
        yield return new WaitForSeconds(AttackEndTime);

        string animationName = "AttackEnd";
        _burretAnimation.SetBool(animationName,true);

    }

    public void Attackend()
    {
        //�A�j���[�V�������I����������
        Destroy(this.gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        string _playerTag = "Player";

        if (!_isAttack&&collision.gameObject.CompareTag(_playerTag))
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
                //�_���[�W��^����
                _playerAttackScript.TakeDamage(_attackPower, this.gameObject, false, false);
            }
            //���b�Ԏ������U���ł��Ȃ�����
            StartCoroutine(AttackCoolDown());
        }
    }

    private IEnumerator AttackCoolDown()
    {
        //�U���𓖂Ă����ƁA1.2�b�Ԃ͍U���𓖂ĂȂ�����
        float waitTime = 1.2f;

        yield return new WaitForSeconds(waitTime);
        _isAttack = false;
    }
}
