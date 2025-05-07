using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NormalEnemyScript : MonoBehaviour
{
    private GameObject _player = default;
    private PlayerStatusChange _playerStatusScript=default;

    [Header("�G��HP "), SerializeField] private float _hitPoint = 0f;
    [Header("�G�̍U���͂̍Œ�l"), SerializeField] private float _attackMinPower = 0f;
    [Header("�G�̍U���͂̍ō��l"), SerializeField] private float _attackMaxPower = 0f;
    [Header("�m�b�N�o�b�N�̑傫��"), SerializeField] private float _knockBackValue = 0f;
    [Header("�ς��Ăق����U����"),SerializeField] private float NumberOfAttacks = default;

    [Header("Mp�񕜃A�C�e��"),SerializeField] private GameObject _magicPointStar = default;

    private Rigidbody2D _enemyRB = default;

    private float _moveX = 2f;

    private bool _isMoveDirection = false; //true�͍������Afalse�͉E����
    private bool _isStun = false;
    private bool _isRightKnockBack = false;//�E�m�b�N�o�b�N����
    private bool _isLeftKnockBack = false;//���m�b�N�o�b�N����

    private const float MAX_MOVEMENT_SPEED = 2.5f;//�v���C���[�̍ő�ړ����x
    private const float TIME_DELTATIME = 500f; //Time.Deltatime�Ɋ|����l

    private const int FLIP_VALUE = 180;

    // Start is called before the first frame update
    void Start()
    {
        _enemyRB = this.GetComponent<Rigidbody2D>();
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        //�X�^�����Ă��Ȃ�
        if (!_isStun)
        {
            DefaultMove();
        }
        //�X�^�����Ă�����
        else
        {
            StunProcess();
        }

        if (this._hitPoint <= 0)
        {
            Death();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //�ǂɓ���������
        if (collision.gameObject.CompareTag("Wall"))
        {
            //��������������
            if (_isMoveDirection)
            {
                //�E�����ɂ���
                _isMoveDirection = false;

                //���΂ɂ���
                this.transform.rotation = new Quaternion(0, 0, 0, 0);
            }
            //�E������������
            else
            {
                //�������ɂ���
                _isMoveDirection = true;

                //���΂ɂ���
                this.transform.rotation = new Quaternion(0, FLIP_VALUE, 0, 0);
            }
        }

        string _playerTag = "Player";
        if (collision.gameObject.CompareTag(_playerTag))
        {
            float attackPower = Random.Range(_attackMinPower, _attackMaxPower);
            collision.gameObject.GetComponent<PlayerAttack>().TakeDamage(attackPower, this.gameObject, true, true);
        }

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("TransparentWall"))
        {
            //��������������
            if (_isMoveDirection)
            {
                //�E�����ɂ���
                _isMoveDirection = false;

                //���΂ɂ���
                this.transform.rotation = new Quaternion(0, 0, 0, 0);
            }
            //�E������������
            else
            {
                //�������ɂ���
                _isMoveDirection = true;

                //���΂ɂ���
                this.transform.rotation = new Quaternion(0, FLIP_VALUE, 0, 0);
            }
        }
    }
    

    private void DefaultMove()
    {
        //���ړ�
        if (_isMoveDirection)
        {
            //�G�̈ړ����x��MAXMOVEMENTSPEED�܂łɐ�������
            if (_enemyRB.velocity.x < -MAX_MOVEMENT_SPEED)
            {

                _enemyRB.AddForce(new Vector2(_moveX * Time.deltaTime * TIME_DELTATIME, 0));
            }
            //
            else if (_enemyRB.velocity.x > -MAX_MOVEMENT_SPEED)
            {
                _enemyRB.AddForce(new Vector2(-_moveX * Time.deltaTime * TIME_DELTATIME, 0));
            }

            //�ʏ�ړ�
            else
            {
                _enemyRB.velocity = new Vector2(-MAX_MOVEMENT_SPEED, this._enemyRB.velocity.y);
            }
        }
        //�E�ړ�
        else
        {
            //�G�̈ړ����x��MAXMOVEMENTSPEED�܂łɐ�������
            if (_enemyRB.velocity.x > MAX_MOVEMENT_SPEED)
            {
                _enemyRB.AddForce(new Vector2(-_moveX * Time.deltaTime * TIME_DELTATIME, 0));
            }
            else if (_enemyRB.velocity.x < MAX_MOVEMENT_SPEED)
            {
                _enemyRB.AddForce(new Vector2(_moveX * Time.deltaTime * TIME_DELTATIME, 0));
            }

            //�ʏ�ړ�
            else
            {
                _enemyRB.velocity = new Vector2(MAX_MOVEMENT_SPEED, this._enemyRB.velocity.y);
            }
        }
        //this._enemyRB.velocity = new Vector2(_moveX, this._enemyRB.velocity.y);
    }

    private void StunProcess()
    {
        //�ړ����Ă��Ȃ����A�E�Ƀm�b�N�o�b�N��
        if (_isRightKnockBack)
        {
            if (_enemyRB.velocity.x > 0)
            {
                //���x�����X�ɖ߂�
                _enemyRB.AddForce(new Vector2(-_moveX * Time.deltaTime * TIME_DELTATIME, 0));
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
                _enemyRB.AddForce(new Vector2(_moveX * Time.deltaTime * TIME_DELTATIME, 0));
            }
            else
            {
                //�m�b�N�o�b�N�I��
                _isLeftKnockBack = false;
            }
        }
    }
    /// <summary>
    /// �U����H��������̏���
    /// </summary>
    /// <param name="damage">��炤�_���[�W��</param>
    /// <param name="knockBackPower">�m�b�N�o�b�N�̋���</param>
    public void BeAttacked(float damage, float knockBackPower,bool isKnockBackAttack)
    {
        //HP�����炷
        _hitPoint -= damage;

        //�m�b�N�o�b�N

        if (isKnockBackAttack)
        {
            //�v���C���[���������E�ɂ�����
            if (_player.transform.position.x > this.transform.position.x)
            {
                //��������������
                if (_isMoveDirection)
                {

                    //��������݂Ă܂������Ƀm�b�N�o�b�N
                    _enemyRB.velocity = Vector2.zero;
                    _enemyRB.AddForce(transform.right * knockBackPower, ForceMode2D.Impulse);
                    _KnockBackDirection();

                    //�E�����ɂ���
                    _isMoveDirection = false;
                    //���]������
                    this.transform.rotation = new Quaternion(0, 0, 0, 0);
                }
                else
                {
                    //�������猩�Č��Ƀm�b�N�o�b�N
                    _enemyRB.velocity = Vector2.zero;
                    _enemyRB.AddForce(-transform.right * knockBackPower, ForceMode2D.Impulse);
                    _KnockBackDirection();

                }

            }
            //�v���C���[��������荶�ɂ�����
            else
            {
                //��������������
                if (_isMoveDirection)
                {
                    //��������݂Č��Ƀm�b�N�o�b�N
                    _enemyRB.velocity = Vector2.zero;
                    _enemyRB.AddForce(-transform.right * knockBackPower, ForceMode2D.Impulse);
                    _KnockBackDirection();

                }
                //�E������������
                else
                {
                    //�������猩�Ă܂������Ƀm�b�N�o�b�N
                    _enemyRB.velocity = Vector2.zero;
                    _enemyRB.AddForce(transform.right * knockBackPower, ForceMode2D.Impulse);
                    _KnockBackDirection();

                    //�������ɂ���
                    _isMoveDirection = true;

                    //���]������
                    this.transform.rotation = new Quaternion(0, FLIP_VALUE, 0, 0);
                }
            }
        }
    }

    private void _KnockBackDirection()
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
        //�m�b�N�o�b�N
        _isStun = true;
        _enemyRB.velocity = Vector2.zero;
        _enemyRB.AddForce(transform.right * _knockBackValue, ForceMode2D.Impulse);
        StartCoroutine(StunRecover());
        //�v���C���[�ɓ�����Ȃ�����
        int DontHitEnemyLayer = 20;
        this.gameObject.layer = DontHitEnemyLayer;
    }

   

    public void StatusIncrease(float hitPointIncreasedValue,float attackPowerIncreasedValue)
    {

        //���g�̊�bHP�ɔ{����������
        _hitPoint *= hitPointIncreasedValue;

        //���g�̍U���́i�Œ�l�A�ō��l�j�ɔ{����������
        _attackMinPower *= attackPowerIncreasedValue;
        _attackMaxPower *= attackPowerIncreasedValue;

        //�v���C���[�̃X�N���v�g�擾
        string playerTag = "Player";
        _player = GameObject.FindGameObjectWithTag(playerTag);
        _playerStatusScript = _player.GetComponent<PlayerStatusChange>();

        //�v���C���[�̃X�e�[�^�X�ɂ���Ď��g�̃X�e�[�^�X�ω������郁�\�b�h���Ăяo��
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
                if (playerAttackvalue*6 < _hitPoint)
                {
                    
                    float lestHitPoint = default;

                    //�U��U���������̎c��HP���v�Z
                   lestHitPoint=_hitPoint - playerAttackvalue * 6;

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

    private IEnumerator StunRecover()
    {
        float stunTime = 2f;
        yield return new WaitForSeconds(stunTime);
        _isStun = false;

        //�v���C���[�ɓ�����悤�ɂ���
        int EnemyLayer = 10;
        this.gameObject.layer = EnemyLayer;
    }

    public void Death()
    {

        float minRandomValue = 0;
        float maxRandomValue = 101;
        float halfValue = 3;

        float randomValue =Random.Range(minRandomValue, maxRandomValue);

        //3���̈��Mp�񕜃A�C�e�����o��
        if (randomValue <= maxRandomValue / halfValue)
        {
            Instantiate(_magicPointStar, this.transform.transform.position, Quaternion.identity);
        }

        //���񂾂��Ƃ��X�e�[�W�R�A�ɓ`����
        this.GetComponentInParent<SummonEnemyScript>().BeDefeated();
        Destroy(this.gameObject);
    }

    
}
