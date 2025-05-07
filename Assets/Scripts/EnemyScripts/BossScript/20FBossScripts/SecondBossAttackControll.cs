using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondBossAttackControll : BossStatus,IBossStatus
{
    private const int MAX_HEALTH = 10000;//�{�X�̗̑͒�`
    private const int BODY_CONTACT_DAMAGE = 40;//�{�X�̐ڐG�_���[�W��`
    private const int MAX_ATTACK_NUMBER = 4;//�ő�̍U���ԍ�+1

    [SerializeField] private GameObject _exitHoleObject = default;//�X�e�[�W��̏o���I�u�W�F�N�g
    [SerializeField] private GameObject _healthBarObject = default;//�X�e�[�W��̃{�X�̗̑̓o�[�I�u�W�F�N�g

    private GameObject _player = default;

    private Animator _bossAnime = default;

    [Header("�{�X�̌�"), SerializeField] private GameObject _bossSword = default;
    [SerializeField] private List<SummonEnemyScript> _summonEnemyPoint = new List<SummonEnemyScript>();

    private float _rushspeed = 10f;

    private bool _isFacingRight = false;//�E����
    private bool _isFacingLeft = false;//������
    private bool _isDirection = false;
    private bool _isLeftRush = false;
    private bool _isRightRush = false;
    private bool _isDistanceAdjustment = false;
    private bool _isDeath = false;

    private float _startPositionX = default;
    private float _hitPointMultipleValue = 1.2f;
    private float _attackPowerMultipleValue = 0.9f;
    private float _playerDistance = default;

    private int _attackNumber = 0;

    private float _coolDownTime = default; //�U����̃N�[���^�C��

    private string _animationName = default;

    private void Start()
    {

        Health = MAX_HEALTH;
        BodyContactDamage = BODY_CONTACT_DAMAGE;

        string findTag = "Player";
        _player = GameObject.FindGameObjectWithTag(findTag);

        _bossAnime = this.GetComponent<Animator>();

        _startPositionX = this.transform.position.x;
    }

    private void Update()
    {

        if (!IsAttackCoolDown)
        {
            DecisionAttack();
            IsAttackCoolDown = true;
        }

        switch (_attackNumber)
        {
            //�ːi�U��
            case 1:
                FirstAttackNumber();
                break;

            //3�A��
            case 2:
                SecondAttackNumber();
                break;

            //�G������
            case 3:
                ThirdAttackNumber();
                break;
        }

        if (Health <= 0&&!_isDeath)
        {
            _isDeath = true;
            _exitHoleObject.SetActive(true);
            Death();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            _isLeftRush = false;
            _isRightRush = false;
        }

        string playerTag = "Player";

        if (collision.gameObject.CompareTag(playerTag) && BodyContactDamage > 0)
        {
            BeAttacked(collision.gameObject);
        }
    }

    #region �U�����\�b�h
    private void DecisionAttack()
    {
        int chooseAttackNumber = ChooseAttack(MAX_ATTACK_NUMBER);

        switch (chooseAttackNumber)
        {
            //�ːi�U��
            case 1:
                //�������E�ɂ���
                if (_player.transform.position.x >= this.transform.position.x)
                {
                    _isRightRush = true;
                    this.transform.localRotation = new Quaternion(0, 0, 0, 0);
                }
                else
                {
                    _isLeftRush = true;
                    this.transform.localRotation = new Quaternion(0, 180, 0, 0);
                }
                _attackNumber = chooseAttackNumber;
                break;

            //3�A���U��
            case 2:

                //�������v���C���[���E�ɂ�����
                if (_player.transform.position.x < this.transform.position.x)
                {
                    //������������
                    _isFacingLeft = true;

                    this.transform.localRotation = new Quaternion(0, 180, 0, 0);
                }
                //�������^�񒆂�荶�ɂ�����
                else if (_player.transform.position.x > this.transform.position.x)
                {

                    //�E����������
                    _isFacingRight = true;

                    this.transform.localRotation = new Quaternion(0, 0, 0, 0);
                }

                _isDistanceAdjustment = true;

                _attackNumber = chooseAttackNumber;

                break;

            //�G������
            case 3:

                //�������^�񒆂��E�ɂ�����
                if (_startPositionX < this.transform.position.x)
                {
                    //������������
                    _isFacingLeft = true;

                    this.transform.localRotation = new Quaternion(0, 180, 0, 0);
                }
                //�������^�񒆂�荶�ɂ�����
                else if (_startPositionX > this.transform.position.x)
                {

                    //�E����������
                    _isFacingRight = true;

                    this.transform.localRotation = new Quaternion(0, 0, 0, 0);
                }



                _attackNumber = chooseAttackNumber;

                break;
        }
    }

    private void FirstAttackNumber()
    {
        _bossAnime.enabled = true;

        if (_isRightRush)
        {
            this.transform.position += Vector3.right * _rushspeed * Time.deltaTime;
        }
        else if (_isLeftRush)
        {
            this.transform.position -= Vector3.right * _rushspeed * Time.deltaTime;
        }
        else
        {
            _attackNumber = 0;
            _coolDownTime = 3f;

            _animationName = "RushEndAnimation";
            _bossAnime.PlayInFixedTime(_animationName, 0, 0f);

            _isLeftRush = false;
            _isRightRush = false;


            StartCoroutine(AttackCoolDown(_coolDownTime));
        }
    }

    private void SecondAttackNumber()
    {
        if (_isDistanceAdjustment)
        {
            //��苗���ɂ��Ȃ��ꍇ�̓v���C���[�Ǝ����̋������v�Z��������
            if (_isFacingLeft || _isFacingRight)
            {
                _playerDistance = this.transform.position.x - _player.transform.position.x;
            }

            float stopAdjustmentDistance = 3;

            //�E�����ł��A�����̃|�W�V�����ƃv���C���[�Ƃ̋�����3�ȉ���������
            if (_isFacingRight && _playerDistance < -stopAdjustmentDistance)
            {
                //�E�Ɉړ�
                this.transform.position += Vector3.right * _rushspeed * Time.deltaTime;
            }
            //�������ŏ��A�����̃|�W�V�����ƃv���C���[�Ƃ̋�����3�ȏゾ������
            else if (_isFacingLeft && _playerDistance > stopAdjustmentDistance)
            {
                //���Ɉړ�
                this.transform.position -= Vector3.right * _rushspeed * Time.deltaTime;
            }
            else
            {
                _isDistanceAdjustment = false;
                _isFacingLeft = false;
                _isFacingRight = false;

                _bossAnime.enabled = true;
                _bossSword.GetComponent<BoxCollider2D>().enabled = true;

                _animationName = "SordAttackAnimation";
                _bossAnime.PlayInFixedTime(_animationName, 0, 0f);

            }
        }


        if (_isDirection)
        {
            DirectionChange();
        }


        //�N�[���_�E���̓��\�b�h��EndSordAttack()�ɂĐ���
    }

    private void ThirdAttackNumber()
    {
        //�E�����ł��A�����̃|�W�V�������^�񒆂�荶��������
        if (_isFacingRight && _startPositionX >= this.transform.position.x)
        {
            //�E�Ɉړ�
            this.transform.position += Vector3.right * _rushspeed * Time.deltaTime;
        }
        //�������ŏ��A�����̃|�W�V�������^�񒆂��E��������
        else if (_isFacingLeft && _startPositionX <= this.transform.position.x)
        {
            //���Ɉړ�
            this.transform.position -= Vector3.right * _rushspeed * Time.deltaTime;
        }
        else
        {

            _attackNumber = 0;
            for (int summonAmmount = 0; summonAmmount < _summonEnemyPoint.Count; summonAmmount++)
            {
                _summonEnemyPoint[summonAmmount].SummonEnemyPreparation(null, _hitPointMultipleValue, _attackPowerMultipleValue);
            }

            _isFacingLeft = false;
            _isFacingRight = false;

            _coolDownTime = 5f;
            StartCoroutine(AttackCoolDown(_coolDownTime));

        }
    }

    #endregion
    public void StartDirectionChange()
    {
        _isDirection = true;
    }

    public void EndDirectionChange()
    {
        _isDirection = false;
    }

    public void DirectionChange()
    {
        //�����]��
        //�������v���C���[���E�ɂ�����
        if (_player.transform.position.x >= this.transform.position.x && !_isFacingRight)
        {
            //���]������
            _isFacingRight = true;
            _isFacingLeft = false;
            this.transform.localRotation = new Quaternion(0, 0, 0, 0);
        }
        //�������v���C���[�����ɂ�����
        else if (_player.transform.position.x <= this.transform.position.x && !_isFacingLeft)
        {
            _isFacingRight = false;
            _isFacingLeft = true;
            this.transform.localRotation = new Quaternion(0, 180, 0, 0);
        }
    }

    public void EndSordAttack()
    {
        _attackNumber = 0;
        _bossSword.GetComponent<BoxCollider2D>().enabled = false;
        _isFacingLeft = false;
        _isFacingRight = false;


        _coolDownTime = 2.5f;
        StartCoroutine(AttackCoolDown(_coolDownTime));

    }

    public void HealthBarSet()
    {
        BossHealthBar = _healthBarObject;
        print("a");
        HealthBarDisplay();

    }


}
