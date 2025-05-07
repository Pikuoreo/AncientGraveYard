using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaserEnemyScript : MonoBehaviour
{
    [Header("�G��HP "), SerializeField] private float _hitPoint = 0f;
    [Header("�G�̍U���͂̍Œ�l"), SerializeField] private float _attackMinPower = 0f;
    [Header("�G�̍U���͂̍ō��l"), SerializeField] private float _attackMaxPower = 0f;
    [Header("�m�b�N�o�b�N�̑傫��"), SerializeField] private float _knockBackValue = 0f;
    [Header("�ς��Ăق����U����"), SerializeField] private float NumberOfAttacks = default;

    [Header("Mp�񕜃A�C�e��"), SerializeField] private GameObject _magicPointStar = default;

    private GameObject _playerPosition = default; //�v���C���[�̃|�W�V�������Ƃ�
    private PlayerStatusChange _playerStatusScript = default;

    private Rigidbody2D _enemyRB = default;//�G�̃��W�b�g�{�f�B

    private bool _isTurn = false;//false�Ȃ獶�����Atrue�Ȃ�E����
    private bool _isStartCoroutine = false;
    private bool _isRightKnockBack = false;//�E�m�b�N�o�b�N����
    private bool _isLeftKnockBack = false;//���m�b�N�o�b�N����

    private bool _isStun = false;

    private int _turnTime = 0;


    private float _moveSpeed = 2f;//�ړ��̑���

    private const float MAX_MOVEMENT_SPEED = 2.5f;//�G�̍ő�ړ����x
    private const float TIME_DELTATIME = 500f; //Time.Deltatime�Ɋ|����l

    private const int FLIP_VALUE = 180;
    // Start is called before the first frame update
    void Start()
    {
        
        _enemyRB = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //�X�^��������Ȃ������炩�A�v���C���[�������Ă�����
        if (!_isStun&&_playerPosition!=null)
        {
            Move();
        }
        //�X�^�����Ă�����
        else
        {
            StunKnockBackControl();
        }
        //�̗͂��O�ɂȂ�����
        if (_hitPoint < 0)
        {
            Death();
        }


    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        string PlayerTag = "Player";
        //�v���C���[�ɓ���������
        if (collision.gameObject.CompareTag(PlayerTag))
        {
            //�_���[�W�l�̗����擾
            float attackPower = Random.Range(_attackMinPower, _attackMaxPower);

            //�_���[�W��^����
            collision.gameObject.GetComponent<PlayerAttack>().TakeDamage(attackPower, this.gameObject, true, true);
        }

     
    }
    private void Death()
    {
        float minRandomValue = 0;
        float maxRandomValue = 101;
        float halfValue = 3;

        float randomValue = Random.Range(minRandomValue, maxRandomValue);

        //3���̈��Mp�񕜃A�C�e�����o��
        if (randomValue <= maxRandomValue / halfValue)
        {
            Instantiate(_magicPointStar, this.transform.transform.position, Quaternion.identity);
        }

        //���񂾂��Ƃ��X�ā[�W�R�A�ɓ`����
        this.GetComponentInParent<SummonEnemyScript>().BeDefeated();
        Destroy(this.gameObject);
    }
    private void Move()
    {

        int maxTurnNumberOfTime = 7;//�ő唽�]��

        //�V��U���������ǔ����ꎞ���f����
        if (_turnTime < maxTurnNumberOfTime)
        {
            //�v���C���[��ǔ�������
            if (_playerPosition.transform.position.x <= this.transform.position.x)
            {
                //�E������������
                if (_isTurn)
                {
                    //���Ɍ�������
                    _isTurn = false;
                    _turnTime++;

                    //���΂ɂ���
                    this.transform.rotation = new Quaternion(0, 0, 0, 0);
                }

                //�G�̈ړ����x��MAXMOVEMENTSPEED�܂łɐ�������
                if (_enemyRB.velocity.x > -MAX_MOVEMENT_SPEED)
                {
                    _enemyRB.AddForce(new Vector2(-_moveSpeed * Time.deltaTime * TIME_DELTATIME, 0));
                }
                else if (_enemyRB.velocity.x < -MAX_MOVEMENT_SPEED)
                {
                    _enemyRB.AddForce(new Vector2(_moveSpeed * Time.deltaTime * TIME_DELTATIME, 0));
                }
                else
                {
                    //print("��"+ this._playerRB.velocity.x);
                    _enemyRB.velocity = new Vector2(-MAX_MOVEMENT_SPEED, this._enemyRB.velocity.y);
                }
            }
            else
            {
                //��������������
                if (!_isTurn)
                {
                    //�E�Ɍ�������
                    _isTurn = true;
                    _turnTime++;
                    //���΂ɂ���
                    this.transform.rotation = new Quaternion(0, FLIP_VALUE, 0, 0);
                }
              

                //�G�̈ړ����x��MAXMOVEMENTSPEED�܂łɐ�������
                if (_enemyRB.velocity.x < MAX_MOVEMENT_SPEED)
                {

                    _enemyRB.AddForce(new Vector2(_moveSpeed * Time.deltaTime * TIME_DELTATIME, 0));
                }
                else if(_enemyRB.velocity.x > MAX_MOVEMENT_SPEED)
                {
                    _enemyRB.AddForce(new Vector2(-_moveSpeed * Time.deltaTime * TIME_DELTATIME, 0));
                }
                else
                {
                    //print("��"+this._playerRB.velocity.x);
                    _enemyRB.velocity = new Vector2(MAX_MOVEMENT_SPEED, this._enemyRB.velocity.y);
                }
            }
        }
        else
        {
            //�������Ȃ�
            if (!_isTurn)
            {
                //�G�̈ړ����x��MAXMOVEMENTSPEED�܂łɐ�������
                if (_enemyRB.velocity.x > -MAX_MOVEMENT_SPEED)
                {
                    _enemyRB.AddForce(new Vector2(-_moveSpeed * Time.deltaTime * TIME_DELTATIME, 0));
                }
                else if (_enemyRB.velocity.x < -MAX_MOVEMENT_SPEED)
                {
                    _enemyRB.AddForce(new Vector2(_moveSpeed * Time.deltaTime * TIME_DELTATIME, 0));
                }
                else
                {
                    _enemyRB.velocity = new Vector2(-MAX_MOVEMENT_SPEED, this._enemyRB.velocity.y);
                }
            }
            //�E�����Ȃ�
            else
            {
                //�G�̈ړ����x��MAXMOVEMENTSPEED�܂łɐ�������
                if (_enemyRB.velocity.x < MAX_MOVEMENT_SPEED)
                {

                    _enemyRB.AddForce(new Vector2(_moveSpeed * Time.deltaTime * TIME_DELTATIME, 0));
                }
                else if (_enemyRB.velocity.x > MAX_MOVEMENT_SPEED)
                {
                    _enemyRB.AddForce(new Vector2(-_moveSpeed * Time.deltaTime * TIME_DELTATIME, 0));
                }
                else
                {
                    _enemyRB.velocity = new Vector2(MAX_MOVEMENT_SPEED, this._enemyRB.velocity.y);
                }
            }

            if (!_isStartCoroutine)
            {
                _isStartCoroutine = true;
                StartCoroutine(ResetTurnTime());
            }

        }

    }

    private void StunKnockBackControl()
    {
        //�ړ����Ă��Ȃ����A�E�Ƀm�b�N�o�b�N��
        if (_isRightKnockBack)
        {
            if (_enemyRB.velocity.x > 0)
            {
                //���x�����X�ɖ߂�
                _enemyRB.AddForce(new Vector2(-_moveSpeed * Time.deltaTime * TIME_DELTATIME, 0));
            }
            else
            {
                //�m�b�N�o�b�N�I��
                _isRightKnockBack = false;
            }
        }
        //�ړ����Ă��Ȃ����A���Ƀm�b�N�o�b�N��
        else if (_isLeftKnockBack)
        {
            if (_enemyRB.velocity.x < 0)
            {
                //���x�����X�ɖ߂�
                _enemyRB.AddForce(new Vector2(_moveSpeed * Time.deltaTime * TIME_DELTATIME, 0));
            }
            else
            {
                //�m�b�N�o�b�N�I��
                _isLeftKnockBack = false;
            }
        }
    }

    public void StatusIncrease(float hitPointincreasedValue, float attackPowerIncreasedValue)
    {
        //�[�x�ɂ���ăX�e�[�^�X���グ��
        _hitPoint *= hitPointincreasedValue;
        _attackMinPower *= attackPowerIncreasedValue;
        _attackMaxPower *= attackPowerIncreasedValue;

        _playerPosition = GameObject.FindGameObjectWithTag("Player");

        _playerStatusScript = _playerPosition.GetComponent<PlayerStatusChange>();

        StatusAdjustment();
    }

    private void StatusAdjustment()
    {
        //�v���C���[�̎���HP 
        float _playerHP = _playerStatusScript.EnemyAttackPowerAdjustment();

        if (_playerHP < _attackMaxPower * NumberOfAttacks)
        {
            //�ő�U���͂̓v���C���[�̗̑͂̉�������
            float firstCalculation = _attackMaxPower / _playerHP;

            //�ő�U���͂�firstCalculation�ŏo�����l���|����
            float secondCalculation = _attackMaxPower * firstCalculation;

            //secondCalculation�ŏo�����l��ς��Ăق����U���񐔂Ŋ���
            float thirdCalculation = Mathf.Floor(secondCalculation / NumberOfAttacks);

            //���g�̍U���͂�thirdCalculation�ŏo����������
            _attackMinPower -= thirdCalculation;
            _attackMaxPower -= thirdCalculation;

        }



        //�v���C���[�̍U���͂��擾
        float playerAttackvalue = _playerStatusScript.EnemyHitPointAdjustment().Item1;

        //�v���C���[�̍��̐E�Ƃ��擾
        int playerProfession = _playerStatusScript.EnemyHitPointAdjustment().Item2;



        switch (playerProfession)
        {
            //���m�̎�
            case 1:

                //�v���C���[���U��U�����Ă��|���Ȃ�HP�Ȃ�
                if (playerAttackvalue * 6 < _hitPoint)
                {

                    float lestHitPoint = default;

                    //�U��U���������̎c��HP���v�Z
                    lestHitPoint = _hitPoint - playerAttackvalue * 6;

                    //�v�Z���ʂ̒l���Q����
                    _hitPoint -= Mathf.Floor(lestHitPoint / 2);
                }

                break;

            //���@�g���̎�
            case 2:

                //�v���C���[��8��U�����Ă��|���Ȃ�HP�Ȃ�
                if (playerAttackvalue * 8 < _hitPoint)
                {
                    float lestHitPoint = default;

                    //�U��U���������̎c��HP���v�Z
                    lestHitPoint = _hitPoint - playerAttackvalue * 6;

                    //�v�Z���ʂ̒l���Q����
                    _hitPoint -= Mathf.Floor(lestHitPoint / 2);
                }

                break;
        }

    }

    public void BeAttacked(float Damage, float knockBackPower , bool isKnockBackAttack)
    {
        //hp�����炷
        _hitPoint -= Damage;

        if (isKnockBackAttack)
        {
            //�v���C���[���������E�ɂ�����
            if (_playerPosition.transform.position.x > this.transform.position.x)
            {
                //��������������
                if (_isTurn)
                {
                    //��������݂Ă܂������Ƀm�b�N�o�b�N
                    _enemyRB.velocity = Vector2.zero;
                    _enemyRB.AddForce(transform.right * knockBackPower, ForceMode2D.Impulse);
                    _knockBackDirection();
                }
                else
                {
                    //�������猩�Č��Ƀm�b�N�o�b�N
                    _enemyRB.velocity = Vector2.zero;
                    _enemyRB.AddForce(-transform.right * knockBackPower, ForceMode2D.Impulse);
                    _knockBackDirection();
                }

            }
            //�v���C���[��������荶�ɂ�����
            else
            {
                //��������������
                if (_isTurn)
                {
                    //��������݂Č��Ƀm�b�N�o�b�N
                    _enemyRB.velocity = Vector2.zero;
                    _enemyRB.AddForce(-transform.right * knockBackPower, ForceMode2D.Impulse);
                    _knockBackDirection();
                }
                else
                {
                    //�������猩�Ă܂������Ƀm�b�N�o�b�N
                    _enemyRB.velocity = Vector2.zero;
                    _enemyRB.AddForce(transform.right * knockBackPower, ForceMode2D.Impulse);
                    _knockBackDirection();
                }
            }
        }
       
    }

    private void _knockBackDirection()
    {
        //�m�b�N�o�b�N�̌���������

        //�E�Ƀm�b�N�o�b�N���Ă�����
        if (_enemyRB.velocity.x > 0)
        {
            _isRightKnockBack = true;
        }
        //���Ƀm�b�N�o�b�N���Ă�����
        else
        {
            _isLeftKnockBack = true;
        }

    }

    /// <summary>
    /// �m�b�N�o�b�N���āA��莞�ԍs���s��
    /// </summary>
    public void Stun()
    {
        //��莞�ԍs���s�\
        _isStun = true;
        _enemyRB.velocity = Vector2.zero;
        _enemyRB.AddForce(transform.right * _knockBackValue, ForceMode2D.Impulse);
        StartCoroutine(WakeUp());
        //�v���C���[�ɓ�����Ȃ�����
        int DontHitEnemyLayer = 20;
        this.gameObject.layer = DontHitEnemyLayer;
    }

    private IEnumerator WakeUp()
    {
        float stunTime = 2f;
        yield return new WaitForSeconds(stunTime);
        _isStun = false;

        //�v���C���[�ɓ�����悤�ɂ���
        int EnemyLayer = 10;
        this.gameObject.layer = EnemyLayer;
    }
    private IEnumerator ResetTurnTime()
    {
        //�T�b��ɒǔ����ĊJ������
        float waitTime = 5f;
        yield return new WaitForSeconds(waitTime);
        _turnTime = 0;
        _isStartCoroutine = false;
    }


}
