using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusChange : MonoBehaviour
{
    private int DefencePowerDivideValu = 4;

    private float _guardValue = 0.5f;//�K�[�h���̃_���[�W�y����
    private float _chargeMultiple = 2.5f;//�`���[�W�U�����̔{��

    private float _regeneTime = 0f;�@//Hp�̃_���[�W�H����Ă���̌o�ߎ���
    private float _magicRegeneTime = 0f;//Mp�̃_���[�W�H����Ă���̌o�ߎ���
    private float _regeneValue = 0f;//�����񕜗́A���񂾂�オ��iHP�j
    private float _maxRegeneValue = 3f;//�ő�񕜗́iHP�̍ő�񕜗͖��b)
    private float _magicPointRegeneValue = 0f;//�����񕜗́A���񂾂�オ��(MP)
    private float _magicPointMaxRegeneValue = 10f;//�ő�񕜗́iMP�̍ő�񕜗͖��b)

    private float _actualDamage = default;//�ŏI�I�ɂ��炤�_���[�W��

    private const float START_REGENE_TIME = 2f;//Hp�̉񕜂��n�܂鎞��
    private const float START_MAGICPOINT_REGENE_TIME = 0.75f;//Mp�̉񕜂��n�܂鎞��

    private bool _isDeath = false;//���S����

    private PlayerMoveControll _playerMoveControll = default;
    private PlayerAttack _playerAttack = default;
    private PlayerStatusUI _playerStatusUI = default;
    private PlayerAnimationControll _playerAnimationControll = default;

    private BGMChangeScript _bgmScript = default;

    private GameOverScript _gameOverScript = default;

    [SerializeField] private PlayerStatusManegement _playerStatus = default;


    private Animator _playeDeathAnimation = default;
    void Start()
    {
        //�v���C���[�̂g�o�ɍő�g�o��������
        _playerStatus.HitPoint = _playerStatus.MaxHitPoint;
        //�v���C���[�̂l�o�ɍő�l�o��������
        _playerStatus.MagicPoint = _playerStatus.MaxMagicPoint;

        _playerAnimationControll=this.GetComponentInChildren<PlayerAnimationControll>();
        _playeDeathAnimation = this.GetComponent<Animator>();
        _playerMoveControll = this.GetComponent<PlayerMoveControll>();
        _playerAttack = this.GetComponent<PlayerAttack>();
        _playerStatusUI = this.gameObject.GetComponent<PlayerStatusUI>();

        string findTag = "BackGroundBGM";
        _bgmScript = GameObject.FindGameObjectWithTag(findTag).GetComponent<BGMChangeScript>();

        findTag = "FirstGameOverAnimation";
        _gameOverScript = GameObject.FindGameObjectWithTag(findTag).GetComponent<GameOverScript>();
    }

    // Update is called once per frame
    void Update()
    {
        //�`�[�g�R�}���h
        if (Input.GetKey(KeyCode.H) && Input.GetKey(KeyCode.E) && Input.GetKey(KeyCode.L) && Input.GetKey(KeyCode.P) && Input.GetKeyDown(KeyCode.Return))
        {
            AttackPowerUP(1000);
        }

        //�̗͂������Ă�����
        if (_playerStatus.HitPoint < _playerStatus.MaxHitPoint)
        {
            Regeneration();
        }

        if (_playerStatus.MagicPoint < _playerStatus.MaxMagicPoint)
        {
            MagicpointRegeneration();
        }

        //�̗͂��Ȃ��Ȃ�����
        if (_playerStatus.HitPoint <= 0 && !_isDeath)
        {
            _isDeath = true;
            _playerStatus.HitPoint = 0;
            Death();
        }
    }

    private void Death()
    {
        _playerMoveControll.ControllOff();

        _playerAnimationControll.AnimationChange_Death();

        //���S�A�j���[�V�����𗬂�
        _playeDeathAnimation.SetBool("isDeath", true);

        //BGM�����X�ɏ���������
        _bgmScript.BGMFadeout();

        //��ʂ��Â����Ă���
        _gameOverScript.StartGameOverAnimation();
    }

    public void ConvertHptoMp()
    {
        //����Hp�̗ʁi�ő�Hp�̂Q�O���̈�j
        int CutDownOnLifeValue = 20;

        if (_playerStatus.MagicPoint < _playerStatus.MaxMagicPoint && Mathf.Floor(_playerStatus.HitPoint) > _playerStatus.MaxHitPoint / CutDownOnLifeValue)
        {
            //�񕜎��Ԃ����Z�b�g
            _regeneTime = 0;
            _regeneValue = 0;

            //Hp�̂Q�O���̈�����炷
            _playerStatus.HitPoint -= _playerStatus.MaxHitPoint / CutDownOnLifeValue;

            //HP�o�[�����炷
            _playerStatusUI.HpBarAdaptation();

            //MP�𑝂₷
            _playerStatus.MagicPoint += 20;

            if (_playerStatus.MagicPoint > _playerStatus.MaxMagicPoint)
            {
                _playerStatus.MagicPoint = _playerStatus.MaxMagicPoint;
            }

            //MP�o�[�ɒl��K��������
            _playerStatusUI.MpBarAdaptation();

        }
    }

    /// <summary>
    /// �v���C���[��HP���R��
    /// </summary>
    private void Regeneration()
    {
        float regenePowerUpValue = 4;

        //���R��

        if (_regeneTime >= START_REGENE_TIME)
        {
            //���X�ɉ񕜗͂��グ��
            if (_regeneValue < _maxRegeneValue)
            {
                //��
                _playerStatus.HitPoint += _regeneValue * Time.deltaTime;

                //�񕜗͂��グ��
                _regeneValue += Time.deltaTime / regenePowerUpValue;

                //�̗̓o�[�̍X�V
                _playerStatusUI.HpBarAdaptation();
            }
            else
            {
                //��
                _playerStatus.HitPoint += _regeneValue * Time.deltaTime;

                //�̗̓o�[�̍X�V
                _playerStatusUI.HpBarAdaptation();
            }
        }
        else
        {
            //���Ԍv��
            _regeneTime += Time.deltaTime;
        }
    }

    /// <summary>
    /// �v���C���[��MP���R��
    /// </summary>
    private void MagicpointRegeneration()
    {
        float regenePowerUpValue = 1.25f;

        if (_magicRegeneTime >= START_MAGICPOINT_REGENE_TIME)
        {
            //���X�ɉ񕜗͂��グ��
            if (_magicPointRegeneValue < _magicPointMaxRegeneValue)
            {
                //MP��
                _playerStatus.MagicPoint += _magicPointRegeneValue * Time.deltaTime;

                //�񕜗͂��グ��
                _magicPointRegeneValue += Time.deltaTime / regenePowerUpValue;

                //MP�o�[�̍X�V
                _playerStatusUI.MpBarAdaptation();


            }
            else
            {
                //MP��
                _playerStatus.MagicPoint += _magicPointRegeneValue * Time.deltaTime;

                //MP�o�[�̍X�V
                _playerStatusUI.MpBarAdaptation();
            }
        }
        else
        {
            //���Ԍv��
            _magicRegeneTime += Time.deltaTime;
        }

    }

    public void GuardTakeDamage(float Damage)
    {
        //���ۂ̃_���[�W
        _actualDamage = Mathf.Ceil((Damage - _playerStatus.DefensePower / DefencePowerDivideValu) * _guardValue);

        //HP�����炷
        _playerStatus.HitPoint -= _actualDamage;

        //HP�o�[�����炷
        _playerStatusUI.HpBarAdaptation();

        //�񕜂܂ł̎��Ԃ��O��
        _regeneTime = 0;
        //���R�񕜗͂������l�ɂ���
        _regeneValue = 0;


        _playerStatusUI.GuardDamageTextAppearance(_actualDamage);
    }

    public void ProFessionIsMeleeTakeDamage(float damage)
    {
        float _meleeDamageReductionRate = 1.5f;
        //���m�ɂ��_���[�W�y����
        float _bonusDefence = _playerStatus.DefensePower * _meleeDamageReductionRate;

        //���ۂɎ󂯂�_���[�W
        _actualDamage = Mathf.Ceil(damage - _bonusDefence / DefencePowerDivideValu);

        //HP�����炷
        _playerStatus.HitPoint -= _actualDamage;

        //HP�o�[�����炷
        _playerStatusUI.HpBarAdaptation();

        //�񕜂܂ł̎��Ԃ��O��
        _regeneTime = 0;
        //���R�񕜗͂������l�ɂ���
        _regeneValue = 0;

        _playerStatusUI.UsuallydDamageTextAppearance(_actualDamage);
    }

    public void ProfessionIsMagicTakeDamage(float damage)
    {
        //���ۂ̃_���[�W
        _actualDamage = Mathf.Ceil(damage - _playerStatus.DefensePower / DefencePowerDivideValu);

        //HP�����炷
        _playerStatus.HitPoint -= _actualDamage;

        //HP�o�[�����炷
        _playerStatusUI.HpBarAdaptation();

        //�񕜂܂ł̎��Ԃ��O��
        _regeneTime = 0;
        //���R�񕜗͂������l�ɂ���
        _regeneValue = 0;

        _playerStatusUI.UsuallydDamageTextAppearance(_actualDamage);
    }

    public void HitPointAndMagicPointReset()
    {
        _playerStatus.HitPoint = _playerStatus.MaxHitPoint;
        _playerStatusUI.HpBarAdaptation();

        _playerStatus.MagicPoint = _playerStatus.MaxMagicPoint;
        _playerStatusUI.MpBarAdaptation();

        _isDeath = false;

    }

    public void ReduseMagicPoint(float ReduseValue)
    {
        //MP�����炷
        _playerStatus.MagicPoint -= ReduseValue;

        //_playerStatusUI = this.gameObject.GetComponent<PlayerStatusUI>();
        //MP�o�[�ɒl��K��������
        _playerStatusUI.MpBarAdaptation();

        //MP�񕜂܂ł̌o�ߎ��Ԃ��O�ɂ���
        _magicRegeneTime = 0f;
        //���R�񕜗͂������l�ɂ���
        _magicPointRegeneValue = 0f;


    }

    public void MagicPointRecovery(float recoveryValue)
    {

        //MP�����炷
        _playerStatus.MagicPoint += recoveryValue;

        if (_playerStatus.MagicPoint > _playerStatus.MaxMagicPoint)
        {
            _playerStatus.MagicPoint = _playerStatus.MaxMagicPoint;
        }

        //_playerStatusUI = this.gameObject.GetComponent<PlayerStatusUI>();
        //MP�o�[�ɒl��K��������
        _playerStatusUI.MpBarAdaptation();



        _playerStatusUI.MagicPointRecoveryTextAppearance(recoveryValue);
    }

    /// <summary>
    /// �v���C���[��HP��h��͂����킹������HP��n��
    /// </summary>
    /// <returns></returns>
    public float EnemyAttackPowerAdjustment()
    {
        float returnvalue = default;

        returnvalue = _playerStatus.MaxHitPoint + Mathf.Floor(_playerStatus.DefensePower / DefencePowerDivideValu);
        return (returnvalue);
    }


    /// <summary>
    /// �v���C���[�̐E�ƕʂ̍U���͂�n��
    /// </summary>
    /// <returns></returns>
    public (float, int) EnemyHitPointAdjustment()
    {
        float returnValue = default;

        int profession = _playerAttack.NowProfession();

        switch (profession)
        {
            case 1: //���m��������
                returnValue = _playerStatus.MeleeAttackPower + _playerStatus.BasicPower;

                break;

            case 2://���@�g����������
                returnValue = _playerStatus.MagicattackPower + _playerStatus.BasicPower;
                break;

        }

        return (returnValue, profession);
    }

    public float GiveCriticalMultiple()
    {
        return _playerStatus.CriticalMultiple;
    }

    public float GiveMeleeAttackPower()
    {
        return _playerStatus.MeleeAttackPower;
    }

    #region �A�C�e���擾���\�b�h�S��

    public void AttackPowerUP(float upValue)
    {
        //��b�U����up
        _playerStatus.BasicPower += upValue;
    }

    public void MeleeAttackPowerUp(float upValue)
    {
        //�ߐڍU����up
        _playerStatus.MeleeAttackPower += upValue;
    }

    public void MagicAttackPowerUp(float upValue)
    {
        //��b�U����up
        _playerStatus.MagicattackPower += upValue;

    }

    public void ChargeMultipleUP(float upValue)
    {
        //���ߍU���{��up
        _chargeMultiple += upValue;
    }

    public void hitPointUP(float upValue)
    {
        //�̗�up
        _playerStatus.HitPoint += upValue;
        _playerStatus.MaxHitPoint += upValue;

        _playerStatusUI.HpBarAdaptation();
    }

    public void CriticalChanceUP(float upValue)
    {
        //�N���e�B�J����up
        _playerStatus.CriticalChance += upValue;
    }

    public void CriticalMultipleUP(float upValue)
    {
        //�N���e�B�J���{��up
        _playerStatus.CriticalMultiple += upValue;
    }

    public void DefencePowerUP(float upValue)
    {
        //�h���up
        _playerStatus.DefensePower += upValue;
    }

    public void MagicPointUp(float upValue)
    {
        //���͗�up
        _playerStatus.MagicPoint += upValue;
        _playerStatus.MaxMagicPoint += upValue;

        _playerStatusUI.MpBarAdaptation();
    }

    #endregion
}
