using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaserCaneAttack : MonoBehaviour
{

    [SerializeField] private GameObject _chaserBullet = default;//���@�e

    private int _weaponLevel = 0;//���@����̐i���i���o�[

    private float _magicAttackBonusPower = 1f;//�i���{�[�i�X�U����
    private float _criticalMultiple = default;//�N���e�B�J���{��

    private GameObject _player = default;

    private bool _isAttack = false;//�U�������Ă��邩

    private SpriteRenderer _caneSprite = default;//����̃X�v���C�g

    private AudioSource _seAudio = default;

    private PlayerAttack _playerAttack = default;
    private PlayerStatusChange _playerStatusChange;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");

        _playerAttack = _player.GetComponent<PlayerAttack>();

        _playerStatusChange= _player.GetComponent<PlayerStatusChange>();

        _caneSprite = this.gameObject.GetComponent<SpriteRenderer>();

        _seAudio = this.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

        if (_isAttack)
        {
            //�U�����͕�����v���C���[�ɒǔ�������
            this.transform.position = _player.transform.position;
        }
    }

    public void AttackStart()
    {
        this.transform.position = _player.transform.position;
        _isAttack = true;

        //���@����̉���
        _caneSprite.enabled = true;

        //�N���e�B�J������ƃN���e�B�J���{�����擾
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
        //�U���͂���
        float AttackValue = _playerAttack.GiveCaneMagicAttackPower();

        //�N���e�B�J���{���̌v�Z�i�f�t�H���g�łP�j
        float FinalAttackValue = Mathf.Floor(AttackValue * _criticalMultiple);

        ChaserMagic(FinalAttackValue,isCriticalJudge);
    }


    public void ChaserMagic(float power,bool isCritical)
    {

        StartCoroutine(WeaponInvisible());
        _isAttack = true;

        switch (_weaponLevel)
        {
            case 0://���i�K�̎�
                WeaponLevel0(power, isCritical);
                break;

            case 1:
                WeaponLevel1(power, isCritical);
                break;
        }
    }

    public void WeaponLevel0(float attackPower, bool isCriticalJudge)
    {
        //�N���b�N�����|�W�V�����̎擾
        Vector3 clickposition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        int mouse2DClickPositionZ = 10;
        clickposition.z += mouse2DClickPositionZ;

        //������N���b�N���������Ɍ�������
        this.transform.localRotation = Quaternion.FromToRotation(Vector3.up, clickposition - this.transform.position);

        GameObject burret = default;

        burret = Instantiate(_chaserBullet, this.transform.position, Quaternion.FromToRotation(Vector3.up, clickposition - this.transform.position));
        burret.GetComponent<ChaserBurretScript>().GetPower(attackPower * _magicAttackBonusPower, this.GetComponent<ChaserCaneAttack>(), isCriticalJudge,_weaponLevel);
    }

    public void WeaponLevel1(float attackPower, bool isCriticalJudge)
    {
        //�N���b�N�����|�W�V�����̎擾
        Vector3 clickposition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        int mouse2DClickPositionZ = 10;
        clickposition.z += mouse2DClickPositionZ;

        //������N���b�N���������Ɍ�������
        this.transform.localRotation = Quaternion.FromToRotation(Vector3.up, clickposition - this.transform.position);

        GameObject burret = default;

        burret = Instantiate(_chaserBullet, this.transform.position, Quaternion.FromToRotation(Vector3.up, clickposition - this.transform.position));
        burret.GetComponent<ChaserBurretScript>().GetPower(attackPower * _magicAttackBonusPower, this.GetComponent<ChaserCaneAttack>(), isCriticalJudge, _weaponLevel);
    }

    public void ReProductionSE(AudioClip seMaterial)
    {
        //print(seMaterial.name);

        _seAudio.PlayOneShot(seMaterial);
    }

    public void WeaponLevelUp()
    {

        _weaponLevel++;

        //�i���{�[�i�X��n��
        switch (_weaponLevel)
        {
            case 1:
               //MP���グ��
                float upValue = 20;
                _playerStatusChange.MagicPointUp(upValue);
                break;
        }
    }

    public int GiveWeaponLevel()
    {
        return _weaponLevel;
    }

    private IEnumerator WeaponInvisible()
    {
        yield return new WaitForSeconds(0.5f);
        _caneSprite.enabled = false;
        _isAttack = false;

    }
}
