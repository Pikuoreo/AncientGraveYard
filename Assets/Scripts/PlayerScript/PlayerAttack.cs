using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private bool _isTackle = false;//�^�b�N����t���Ԃ�
    private bool _isParry = false;//�󂯗�����t���Ԃ�
    private bool _isSordDirection = false;//���̌����@false�͍������@true�͉E
    private bool _isAttackDirection = default;//�U����������@false�͍����� true�͉E����
    private bool _isInvincibilityTime;//�U����H�������̖��G���Ԃ�
    private bool _isMeleeAttackCoolDown = false;//�ߐڂ̃N�[���_�E��
    private bool _isMagicattackCoolDown = false;//���@�̃N�[���_�E��
    private bool _isShieldCoolDown = false;//�������܂��Ă��邩
    private bool _isAlive = true;

    private int _profession = 0;//0�͖��E
                                //1�͌��m
                                //�Q�͖��@�g��

    private int _damageSeValue = default;

    private const int MIN_CRITICAL_VALUE = 1;
    private const int MAX_CRITICAL_VALUE = 100;

    private const int PLAYER_FLIP_Y = 180;//�v���C���[�𔽓]�����邽�߂̒l

    private float _meleeAttackCoolDownTime = 0.5f;//�ߐڍU���̃N�[���_�E��
    private float _magicattackCoolDownTime = 0.55f;//���@�U���̃N�[���_�E��

    private AudioSource _playerAudio = default;

    [Header("�G�ƐڐG�������̉�"), SerializeField] private List<AudioClip> _damageSEs = new List<AudioClip>();

    private PlayerMoveControll _moveControll = default;

    private PlayerStatusUI _statusUI = default;

    private PlayerStatusChange _playerStatusChange = default;

    private PlayerAnimationControll _animationcontroll = default;

    [SerializeField] private PlayerStatusManegement _playerStatus = default;

    #region ����p�ϐ�

    [Header("��m�p����ۊǌ�"), SerializeField] private GameObject _meleeWeapon1 = default; //��m�̎��������镐��1
    [Header("��m�p�����2�ۊǌ�"), SerializeField] private GameObject _meleeWeapon2 = default; //��m�̎��������镐��2

    [Header("���p�t�p����ۊǌ�"), SerializeField] private GameObject _magicWeapon1 = default; //���p�t�̎��������镐��1
    [Header("���p�t�p�����2�ۊǌ�"), SerializeField] private GameObject _magicWeapon2 = default; //���p�t�̎��������镐��1

    [Header("���݂̎�������"), SerializeField] private GameObject _currentWeapon1 = default; //�������Ă��镐��1
    [Header("���݂̎�������2"), SerializeField] private GameObject _currentWeapon2 = default; //�������Ă��镐��Q

    private GameObject _leftClickWeapon = default;//�Q�[����ɏo�Ă��鍶�N���b�N����
    private GameObject _rightClickWeapon = default;//�Q�[����ɏo�Ă���E�N���b�N����

    private SpriteRenderer _leftClickWeaponSprite = default;�@//�����Ă��镐��̃X�v���C�g�����_���[�@
    private SpriteRenderer _rightClickWeaponSprite = default;�@//�����Ă��镐��̃X�v���C�g�����_���[2

    private SordMove _meleeWeaponMove = default; //���̓����ɂ��񂷂�X�N���v�g
    private SordAttack _meleeWeaponAttack = default;//���̍U���Ɋւ���X�N���v�g

    private ShieldWeapon _shieldAttack = default;

    private MagicStickAttack _magicStickAttack = default;//���i���@�̍U���X�N���v�g
    private ChaserCaneAttack _chaserCaneAttack = default;//�ǔ����@�̍U���X�N���v�g
    #endregion

    void Start()
    {
        _moveControll = this.GetComponent<PlayerMoveControll>();
        _playerAudio = this.GetComponent<AudioSource>();
        _playerStatusChange = this.GetComponent<PlayerStatusChange>();
        _statusUI = this.GetComponent<PlayerStatusUI>();
        _animationcontroll=this.GetComponentInChildren<PlayerAnimationControll>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isAlive)
        {
            //�E�ƕʍU��
            switch (_profession)
            {
                //���m�̎�
                case 1:

                    WorriorMoveControll();
                    break;

                //���@�g���̎�
                case 2:

                    WizardMovecontroll();
                    break;
            }
        }
    }

    #region �ߐڍU�����\�b�h
    private void WorriorMoveControll()
    {
        #region ����U�鏈��

        //����U��グ��
        if (Input.GetMouseButton(0) && !_isMeleeAttackCoolDown)
        {
            _animationcontroll.ReadyMeleeAttackAnimation();
            SordFlip();

            if (!_leftClickWeaponSprite.enabled)
            {
                //�U������
                _meleeWeaponAttack.StartPreAttack(true, _isAttackDirection, _playerStatus.MagicPoint);

                //currentWeapon1����������
                _leftClickWeaponSprite.enabled = true;

                //�v���C���[�ɒǔ�������
                _meleeWeaponMove.AttackStart(true, _isSordDirection);
            }
        }

        //����U��
        if (Input.GetMouseButtonUp(0) && _leftClickWeaponSprite.enabled)
        {
            _animationcontroll.StartMeleeAttackAnimation();
            //�\������߂�
            _meleeWeaponAttack.StartPreAttack(false, _isAttackDirection, _playerStatus.MagicPoint);

            //�U���N�[���_�E���J�n
            _isMeleeAttackCoolDown = true;
            StartCoroutine(MeleeAttackCoolDown());
        }
        #endregion

        #region �����\���鏈��
        if (Input.GetMouseButtonDown(1) && !_isShieldCoolDown)//�V�[���h���\����
        {
            _rightClickWeaponSprite.enabled = true;
            _rightClickWeaponSprite.color = Color.yellow;
            _moveControll.GetIsShield(true);
            _moveControll.IsGetParry(true);
            _isParry = true;
            _isShieldCoolDown = true;
            StartCoroutine(ParryTime());
        }

        if (_moveControll.GiveIsShield() && Input.GetMouseButtonUp(1))
        {
            _rightClickWeaponSprite.enabled = false;

            _moveControll.GetIsShield(false);

            StartCoroutine(ShieldCoolDown());
        }

        #endregion
    }

    public float GiveAttackPower(bool isCharge)
    {
        //�`���[�W�U���̍U���͌v�Z
        if (isCharge)
        {
            float chargiMultiple = 1.75f;
            return (_playerStatus.MeleeAttackPower + _playerStatus.BasicPower) * chargiMultiple;
        }
        //��`���[�W�U���̍U���͌v�Z
        else
        {
            return _playerStatus.MeleeAttackPower + _playerStatus.BasicPower;
        }
    }

    public float GiveMeleeAttackPower()
    {
        return _playerStatus.MeleeAttackPower;
    }

    public void MeleeAttackEnd()
    {
        _moveControll.GetDerectionFixed(false);
    }

    private void SordFlip()
    {
        //�����N���b�N�����ʒu���v���C��[���E��������
        if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x > this.transform.position.x)
        {
            _isAttackDirection = true;
            //�E����������
            _moveControll.PlayerDirectionChange(true);
            _moveControll.GetDerectionFixed(true);
            this.transform.localRotation = new Quaternion(0, 0, 0, 0);
        }

        //�����N���b�N�����ʒu���v���C���[��荶��������
        else
        {
            _isAttackDirection = false;
            //������������
            _moveControll.PlayerDirectionChange(false);
            _moveControll.GetDerectionFixed(true);
            this.transform.localRotation = new Quaternion(0, PLAYER_FLIP_Y, 0, 0);
        }
    }

    public void TackleAttack()
    {
        _isTackle = true;
        StartCoroutine(ShieldDushtime());
    }

    #endregion

    #region ���@�U�����\�b�h

    private void WizardMovecontroll()
    {
        //����Mp�̗�
        float consumptionPoint = _magicStickAttack.GiveMagicpointConsumptionValue();


        //HP�������MP��
        if (Input.GetKey(KeyCode.G) && Input.GetMouseButtonDown(0))
        {
            _playerStatusChange.ConvertHptoMp();
        }

        #region ���@�U���P

        else if (Input.GetMouseButtonDown(0) && !_isMagicattackCoolDown && _playerStatus.MagicPoint >= consumptionPoint)
        {
            _animationcontroll.MagicAttackAnimation();
            _magicStickAttack.AttackStart();

            _playerStatusChange.ReduseMagicPoint(consumptionPoint);

            //�U���̃N�[���_�E��
            _isMagicattackCoolDown = true;

            StartCoroutine(MagicAttackCoolDown());
        }

        #endregion

        #region ���@�U���Q

        //����MP�̗�
        consumptionPoint = 8;

        if (Input.GetMouseButtonDown(1) && !_isMagicattackCoolDown && _playerStatus.MagicPoint >= consumptionPoint)
        {
            _animationcontroll.MagicAttackAnimation();
            _chaserCaneAttack.AttackStart();

            _playerStatusChange.ReduseMagicPoint(consumptionPoint);

            //�U���̃N�[���_�E��
            _isMagicattackCoolDown = true;
            StartCoroutine(MagicAttackCoolDown());
        }

        #endregion
    }

    public float GiveStickMagicAttackPower()
    {
       
        return _playerStatus.MagicattackPower + _playerStatus.BasicPower;
    }

    public float GiveCaneMagicAttackPower()
    {
        float attackMultiplier = 1.2f;//���i�U����菭���З͂��グ��
        return _playerStatus.BasicPower + _playerStatus.MagicattackPower * attackMultiplier ;
    }

    #endregion

    #region ����
    public bool GiveCriticalJudge()
    {
        //�N���e�B�J�����擾
        int randomValue = Random.Range(MIN_CRITICAL_VALUE, MAX_CRITICAL_VALUE);

        //�N���e�B�J������
        if (randomValue <= _playerStatus.CriticalChance)
        {
            //�N���e�B�J��
            return true;
        }
        else
        {
            //��N���e�B�J��
            return false;
        }
    }
    #endregion

    public float GiveCriticalMultiple()
    {

        //�N���e�B�J���{���擾
        float criticalMultiplier = _playerStatusChange.GiveCriticalMultiple();

        return criticalMultiplier / MAX_CRITICAL_VALUE;

    }

    public void ChangeForProffession(int professionNumber)
    {
        _profession = professionNumber;
        switch (_profession)
        {
            //��m�ɂȂ�����
            case 1:

                //�����Ă��镐��̐؂�ւ�
                _currentWeapon1 = _meleeWeapon1;
                _currentWeapon2 = _meleeWeapon2;

                //��ɃQ�[����ɏo�Ă��镐�킪����Ώ���
                if (_leftClickWeapon != null)
                {
                    Destroy(_leftClickWeapon);
                }

                //��ɃQ�[����ɏo�Ă��镐�킪����Ώ���
                if (_rightClickWeapon != null)
                {
                    Destroy(_rightClickWeapon);
                }

                //�������Ă��镐����Q�[����ɏo��
                _leftClickWeapon = Instantiate(_currentWeapon1);


                int SordFlipZ = 30;
                //�v���C���[���E�������Ă�����
                if (_moveControll.GivePlayerDirection())
                {
                    //���̌������E�����ɂ���
                    _isSordDirection = true;

                    _leftClickWeapon.transform.localRotation = Quaternion.Euler(0, 0, SordFlipZ);
                }
                //�v���C���[�����������Ă�����
                else
                {
                    //���̌������������ɂ���
                    _isSordDirection = false;
                    _leftClickWeapon.transform.localRotation = Quaternion.Euler(0, 0, -SordFlipZ);
                }

                _leftClickWeapon.transform.parent = this.transform;

                float shieldPositonX = 0.4f;

                //�����Q�[����ɐ���
                _rightClickWeapon = Instantiate(_currentWeapon2, this.transform.position + new Vector3(shieldPositonX, 0, 0), Quaternion.identity);
                _rightClickWeapon.transform.parent = this.transform;

                //�X�N���v�g�̎擾
                _meleeWeaponMove = _leftClickWeapon.GetComponent<SordMove>();
                _meleeWeaponAttack = _leftClickWeapon.GetComponent<SordAttack>();

                _shieldAttack=_rightClickWeapon.GetComponent<ShieldWeapon>();

                GetRendererAndCollider();
                break;


            //���p�t�ɂȂ�����
            case 2:

                //�����Ă��镐��̐؂�ւ�s
                _currentWeapon1 = _magicWeapon1;
                _currentWeapon2 = _magicWeapon2;



                //��ɃQ�[����ɏo�Ă��镐�킪����Ώ���
                if (_leftClickWeapon != null)
                {
                    Destroy(_leftClickWeapon);
                }

                //��ɃQ�[����ɏo�Ă��镐�킪����Ώ���
                if (_rightClickWeapon != null)
                {
                    Destroy(_rightClickWeapon);
                }

                //�������Ă��镐����Q�[����ɏo��
                _leftClickWeapon = Instantiate(_currentWeapon1, this.transform.position, Quaternion.identity);
                //_weaponObject.transform.parent = this.transform;

                _rightClickWeapon = Instantiate(_currentWeapon2, this.transform.position, Quaternion.identity);
                //_weaponObject2.transform.parent = this.transform;

                //�X�N���v�g�̎擾
                _magicStickAttack = _leftClickWeapon.GetComponent<MagicStickAttack>();
                _chaserCaneAttack = _rightClickWeapon.GetComponent<ChaserCaneAttack>();

                GetRendererAndCollider();
                break;
        }
    }
    
    private void GetRendererAndCollider()
    {
        //�����Ă��镐��̃X�v���C�g�����_���[���擾
        _leftClickWeaponSprite = _leftClickWeapon.GetComponent<SpriteRenderer>();
        _rightClickWeaponSprite = _rightClickWeapon.GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// �G�ƐڐG�����ۂ̔���
    /// </summary>
    /// <param name="damage">��炤�_���[�W��</param>
    /// <param name="attackedEnemy">�U���𓖂Ă�ꂽ�G</param>
    /// <param name="isParryPossible">�󂯗������ł���U�����ǂ��� true�Ȃ�󂯗�����Afalse�Ȃ�󂯗����Ȃ�</param>
    /// <param name="isTacklePossible">�^�b�N���U�����\���@true�Ȃ�ł���Afalse�Ȃ�o���Ȃ�</param>
    public void TakeDamage(float damage, GameObject attackedEnemy, bool isParryPossible,bool isTacklePossible)
    {
        if (_isParry && isParryPossible)//�U�����󂯗���
        {

            //�󂯗����̎��̉��𗬂�
            _damageSeValue = 2;
            _playerAudio.PlayOneShot(_damageSEs[_damageSeValue]);

            _statusUI.ParryDamageTextAppearance();

            _moveControll.ParryKnockBack();

            int enemyTagNumber = 10;
            int flyEnemyTagNumber = 22;
            int enemyBurretLayer = 17;
            //���������Ώۂ�HP�����G�I�u�W�F�N�g��������
            if (attackedEnemy.gameObject.layer == enemyTagNumber
                ||
                attackedEnemy.gameObject.layer == flyEnemyTagNumber)
            {
                if (attackedEnemy.gameObject.CompareTag("NormalEnemy"))
                {
                    attackedEnemy.GetComponent<NormalEnemyScript>().Stun();
                }
                else if (attackedEnemy.gameObject.CompareTag("ChaserEnemy"))
                {
                    attackedEnemy.GetComponent<ChaserEnemyScript>().Stun();
                }
                else if (attackedEnemy.gameObject.CompareTag("FlyEnemy"))
                {
                    attackedEnemy.GetComponent<FlyEnemyScript>().Stun();
                }
                else
                {
                    print("�s���ȓGTag");
                }

            }
            //HP�������Ȃ��G�I�u�W�F�N�g��������
            else if (attackedEnemy.gameObject.layer == enemyBurretLayer)
            {
                attackedEnemy.GetComponent<TurrentBurretScript>().BurretErase();
            }

        }
        else if (_isTackle && isTacklePossible && !_isInvincibilityTime)
        {
            _isTackle = false;
            //�V�[���h�U���̉��𗬂�
            //_playerAudio.PlayOneShot(_damageSEs[_damageSeValue]);

            //�������������m�b�N�o�b�N����
            _moveControll.TakeDamageKnockBack();

            _statusUI.UsuallydDamageTextAppearance(_playerStatus.MeleeAttackPower);

            int EnemyTagNumber = 10;
            int FlyEnemyTagNumber = 22;

            //���������Ώۂ�HP�����G�I�u�W�F�N�g��������
            if (attackedEnemy.gameObject.layer == EnemyTagNumber
                ||
                attackedEnemy.gameObject.layer == FlyEnemyTagNumber)
            {
                //���ōU������
                _shieldAttack.ShieldBash(attackedEnemy);
            }
        }
        else if (!_isInvincibilityTime)//���G���Ԃ���Ȃ�������
        {
            this.gameObject.layer = 21;
            //���G���ԊJ�n
            _isInvincibilityTime = true;



            //�����\���Ă��āA�󂯗����s�U������Ȃ�������
            if (_moveControll.GiveIsShield())
            {
                //�K�[�h���̉��𗬂�
                _damageSeValue = 1;
                _playerAudio.PlayOneShot(_damageSEs[_damageSeValue]);


                _playerStatusChange.GuardTakeDamage(damage);

                StartCoroutine(EndInvincible());
            }
            else //�_���[�W��H�炤
            {
                //�U����H��������̉��𗬂�
                _damageSeValue = 0;
                _playerAudio.PlayOneShot(_damageSEs[_damageSeValue]);
                switch (_profession)
                {
                    case 1: //���m�̎�

                        _playerStatusChange.ProFessionIsMeleeTakeDamage(damage);


                        _moveControll.TakeDamageKnockBack();


                        StartCoroutine(EndInvincible());
                        break;

                    case 2://���@�g���̎�

                        _playerStatusChange.ProfessionIsMagicTakeDamage(damage);

                        _moveControll.TakeDamageKnockBack();

                        StartCoroutine(EndInvincible());
                        break;
                }

            }
        }
    }

    public int NowProfession()
    {
        return _profession;
    }

    public void ControllSwitch(bool aliveSwitch)
    {
        //�U���ł��Ȃ�����
        _isAlive = aliveSwitch;

        if (aliveSwitch)
        {
            _isInvincibilityTime=false;
        }
    }

    #region �R���[�`��
    private IEnumerator MeleeAttackCoolDown()
    {
        yield return new WaitForSeconds(_meleeAttackCoolDownTime);
        _isMeleeAttackCoolDown = false;
    }

    private IEnumerator MagicAttackCoolDown()
    {
        yield return new WaitForSeconds(_magicattackCoolDownTime);//0.75�b��ɍU���ł���悤�ɂȂ�
        _isMagicattackCoolDown = false;
    }

    private IEnumerator ParryTime()
    {
        float _paryyTime = 0.25f;//�󂯗�����t����
        yield return new WaitForSeconds(_paryyTime);
        _moveControll.IsGetParry(false);
        _isParry = false;
        _rightClickWeaponSprite.color = Color.white;
    }

    private IEnumerator ShieldCoolDown()
    {
        float _shieldCoolDownTime = 0.5f;
        yield return new WaitForSeconds(_shieldCoolDownTime);
        _isShieldCoolDown = false;
    }

    private IEnumerator EndInvincible()
    {
        float invincibleTime = 1.2f; //���G����

        yield return new WaitForSeconds(invincibleTime); //1.2�b��A���G���ԉ���

        if (_moveControll.AliveJudge())
        {
            this.gameObject.layer = 11;
            _isInvincibilityTime = false;
        }
    }

    private IEnumerator ShieldDushtime()
    {

        yield return new WaitForSeconds(0.3f);
        {
            _isTackle = false;
        }
    }
    #endregion
}
