using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicStickAttack : MonoBehaviour
{
    [SerializeField] GameObject _magicBullet = default;//���@�e

    private GameObject _player = default;

    private int _weaponLevel = 0;//���@����̐i���i���o�[


    private float _magicAttackBonusPower1 = 1f;//�i���{�[�i�X�U����
    private float _criticalMultiple = default;//�N���e�B�J���{��

    private bool _isAttack = false;//true�ōU���J�n

    private SpriteRenderer _stickSprite = default;

    private AudioSource _seAudio = default;

    private PlayerAttack _playerAttack = default;
    private PlayerStatusChange _playerStatusChange =default;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");

        _playerAttack = _player.GetComponent<PlayerAttack>();

        _playerStatusChange=_player.GetComponent<PlayerStatusChange>();

        _stickSprite = this.gameObject.GetComponent<SpriteRenderer>();

        _seAudio = this.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

        if (_isAttack)
        {
            this.transform.position = _player.transform.position;
        }
    }
    public void AttackStart()
    {
        this.transform.position = _player.transform.position;
        _isAttack = true;

        //���@����P�̉���
        _stickSprite.enabled = true;


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

    private void PowerCalculation(bool isCriticalJudge)
    {
        float AttackValue = _playerAttack.GiveStickMagicAttackPower();

        float FinalAttackValue = Mathf.Floor(AttackValue * _criticalMultiple);

        StraightMagic(FinalAttackValue, isCriticalJudge);
    }

    private void StraightMagic(float power, bool isCritical)
    {
        StartCoroutine(WeaponInvisible());
        _isAttack = true;

        //����̐i���`��
        switch (_weaponLevel)
        {
            case 0: //���i�K�̎�

                WeaponLevel0(power, isCritical);
                break;

            case 1:

                WeaponLevel1(power, isCritical);
                break;
        }
    }

    private void WeaponLevel0(float attackPower, bool isCriticalJudge)
    {
        //�N���b�N�����|�W�V�����̎擾
        Vector3 clickposition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        clickposition.z += 10;

        //������N���b�N���������Ɍ�������
        this.transform.localRotation = Quaternion.FromToRotation(Vector3.up, clickposition - this.transform.position);

        GameObject burret = default;

        //�񂩂猩�ď�����ɒe����
        burret = Instantiate(_magicBullet, this.transform.position + this.transform.up, Quaternion.FromToRotation(Vector3.up, clickposition - this.transform.position));

        //�����������̃_���[�W����������
        burret.GetComponent<MagicBurretScript>().GetPower(attackPower * _magicAttackBonusPower1, this.GetComponent<MagicStickAttack>(), isCriticalJudge, _weaponLevel);
    }

    private void WeaponLevel1(float attackPower, bool isCriticalJudge)
    {
        //�N���b�N�����|�W�V�����̎擾
        Vector3 clickposition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        clickposition.z += 10;

        //������N���b�N���������Ɍ�������
        this.transform.localRotation = Quaternion.FromToRotation(Vector3.up, clickposition - this.transform.position);

        GameObject burret = default;

        //�񂩂猩�ď�����ɒe����
        burret = Instantiate(_magicBullet, this.transform.position + this.transform.up, Quaternion.FromToRotation(Vector3.up, clickposition - this.transform.position));

        //�����������̃_���[�W����������
        burret.GetComponent<MagicBurretScript>().GetPower(attackPower * _magicAttackBonusPower1, this.GetComponent<MagicStickAttack>(), isCriticalJudge, _weaponLevel);
    }

    //�Đ�����SE���󂯎��A����
    public void ReProductionSE(AudioClip seMaterial)
    {
        _seAudio.PlayOneShot(seMaterial);
    }

    public void WeaponLevelUp()
    {
        _weaponLevel++;
     
        //�i���{�[�i�X��t�^
        switch(_weaponLevel)
        {
            case 1:
                //���@�U���͂��グ��
                int upValue = 30;
                _playerStatusChange.MagicAttackPowerUp(upValue);
                break;
        }
    }

    public int GiveWeaponLevel()
    {
        return _weaponLevel;
    }

    public float GiveMagicpointConsumptionValue()
    {
        float consumptionValue = default;
        switch (_weaponLevel)
        {
            case 0:

                //�U�������Ƃ��̏���͗�
                float consumptionValueLv_1 = 5;
                consumptionValue = consumptionValueLv_1;

                break;

            case 1:

                //�U�������Ƃ��̏���͗�
                float consumptionValueLv_2 = 8;
                consumptionValue = consumptionValueLv_2;
                break;
        }

        return consumptionValue;
    }




    private IEnumerator WeaponInvisible()
    {
        yield return new WaitForSeconds(0.5f);
        _stickSprite.enabled = false;
        _isAttack = false;
    }
}
