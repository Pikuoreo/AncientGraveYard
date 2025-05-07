using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SordAttack : MonoBehaviour
{
    private bool _isAttack = false;//true�ōU���J�n
    private bool _isCharge = default;//true�Ń`���[�W�U��
    private bool _isAttackdirection = false;//false�͍������Atrue�͉E����

    private int _weaponLevel = 0;//����̐i���`��

    private float _meleeChargeTime = 0f;//�ߐڍU���̃`���[�W����
    private float _criticalMultiple = default;
    private float _playerMagicPoint = default;

    private SpriteRenderer _sordSprite = default;

    private  float _chargeAttackTime = 1f;//�`���[�W�܂ł̎���

    private PlayerAttack _playerAttack=default;
    private PlayerStatusChange _playerStatus = default;
    private SordMove _meleeAttackMove = default;

    [Header("�ǉ��U���̌��C"),SerializeField] private GameObject _sordAir = default;
    private GameObject _sordAirClone = default;
    private SordAirMove _sordAirmove = default;

    // Start is called before the first frame update
    void Start()
    {
        _sordSprite = this.GetComponent<SpriteRenderer>();
        _playerAttack = this.GetComponentInParent<PlayerAttack>();
        _meleeAttackMove = this.GetComponent<SordMove>();
        _playerStatus= this.GetComponentInParent<PlayerStatusChange>();
    }

    // Update is called once per frame
    void Update()
    {
        //�N���b�N��������
        if (_isAttack)
        {
            //�U���`���[�W���Ԃ��v��
            _meleeChargeTime += Time.deltaTime;

            if (_meleeChargeTime > _chargeAttackTime&&!_isCharge)
            {
                SpriteChangeCharge();
            }

        }
        //���N���b�N�𗣂�����
        else if(_meleeChargeTime>0)
        {
            _meleeChargeTime = 0;

            switch (_weaponLevel)
            {
                //���i��
                case 0:
                    WeaponLevel0();

                    break;

                //��i��
                case 1:
                    WeaponLevel1();
                    break;
            }
          
        }
    }


    /// <summary>
    /// ���ׂĂ̕��탌�x���̍U���ɋ��ʂ��鏈��
    /// </summary>
    public void AllWeaponLevelProcess()
    {
        //�F�����ɖ߂�
        _sordSprite.color = Color.white;


        bool isCritical = _playerAttack.GiveCriticalJudge();

        //�N���e�B�J���U���Ȃ�
        if (isCritical)
        {
            //�N���e�B�J���{������
            _criticalMultiple = _playerAttack.GiveCriticalMultiple();
        }
        //�ʏ�U���Ȃ�
        else
        {
            //�N���e�B�J���{�����P�ɂ���

            float notcritical = 1;
            _criticalMultiple = notcritical;
        }

        PowerCalculation(isCritical);
    }
    public void WeaponLevel0()
    {
        AllWeaponLevelProcess();
    }

    public void WeaponLevel1()
    {
        int reduseMagicPoint = 10;
        if (_isCharge&&_playerMagicPoint>reduseMagicPoint)
        {
            _playerStatus.ReduseMagicPoint(reduseMagicPoint);
            if (_sordAirClone == null)
            {
                //���C���΂�
                _sordAirClone=Instantiate(_sordAir,this.transform.position,Quaternion.identity);
                _sordAirmove = _sordAirClone.GetComponentInParent<SordAirMove>();
            }

            //�|�W�V���������̈ʒu�ɂ���
            _sordAirClone.transform.position = this.transform.position;
            //�����A�j���[�V�����X�^�[�g
            _sordAirmove.MoveStart(_isAttackdirection);
        }
        AllWeaponLevelProcess();
    }

    private void PowerCalculation(bool isCriticalJudge)
    {
        //�U���́A�N���e�B�J��������擾
        float AttackValue = _playerAttack.GiveAttackPower(_isCharge);

        float FinalAttackValue = Mathf.Floor(AttackValue * _criticalMultiple);

        //�U���͂�n��
        _meleeAttackMove.GetAttackPower(FinalAttackValue,isCriticalJudge);
        _isCharge = false;
    }

    public void StartPreAttack(bool preAttackJudge,bool isSordDirection,float magicPoint)
    {
        _isAttack = preAttackJudge;
        _isAttackdirection = isSordDirection;
        _playerMagicPoint = magicPoint;
    }

    private void SpriteChangeCharge()
    {
        _sordSprite.color = Color.yellow;
        _isCharge = true;
    }

    public void WeaponLevelUp()
    {
        _weaponLevel++;

        //�i���{�[�i�X�t�^
        switch (_weaponLevel)
        {
            case 1:
                //�U���̓A�b�v
                int meleePowerUpValue = 30;
                _playerStatus.MeleeAttackPowerUp(meleePowerUpValue);
                break;
        }
    }

    public int GiveWeaponLevel()
    {
        return _weaponLevel;
    }

   
}
