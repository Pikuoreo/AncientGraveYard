using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour
{

    #region �X�e�[�^�X���p
    [Header("�v���C���[�̗̑�"), SerializeField] private Text _hitPointText = default;
    [Header("�v���C���[��MP"), SerializeField] private Text _magicPointText = default;
    [Header("�v���C���[�̊�b�U����"), SerializeField] private Text _basicPowerText = default;
    [Header("�v���C���[�̃N���e�B�J����"), SerializeField] private Text _criticalChanceText = default;

    [Header("�v���C���[�̃N���e�B�J���{��"), SerializeField] private Text _criticalMultipleText = default;
    [Header("�v���C���[�̋ߐڍU����"), SerializeField] private Text _meleeAttackPowerText = default;
    [Header("�v���C���[�̖��@�U����"), SerializeField] private Text _magicAttackPowerText = default;
    [Header("�v���C���[�̖h���"), SerializeField] private Text _defenceText = default;
    #endregion

    #region Hp�o�[�AMp�o�[�Ɋւ������

    [SerializeField] private Scrollbar _hpBar = default;
    [SerializeField] private Scrollbar _mpBar = default;

    [SerializeField] private TextMeshProUGUI _hpText = default;//�c��Hp�̃e�L�X�g
    [SerializeField] private TextMeshProUGUI _mpText = default;//�c��Mp�̃e�L�X�g
    [SerializeField] private TextMeshProUGUI _damageText = default;//�_���[�W�l��\������e�L�X�g

    #endregion

    [SerializeField] private GameObject _worldSpaceCanvas = default;//���[���h�ˑ��̃L�����o�X

    private TextMeshProUGUI _damageTextDummy = default;//�_���[�W�l��\������e�L�X�g�̐��������I�u�W�F�N�g

    private PlayerMoveControll _playerMoveControll = default;

    [SerializeField] private PlayerStatusManegement _playerStatus = default;

    // Start is called before the first frame update
    void Start()
    {
        _playerMoveControll=this.GetComponent<PlayerMoveControll>();

        HpBarAdaptation();
        MpBarAdaptation();
    }

    /// <summary>
    /// �A�C�e�������J��
    /// </summary>
    public void ListOpen()
    {
        Time.timeScale = 0;

        if (_playerMoveControll.AliveJudge())
        {
            //�̗̓X�e�[�^�X
            string hpPremise = "�ő�̗́F";
            _hitPointText.text = hpPremise + _playerStatus.MaxHitPoint;

            //MP�X�e�[�^�X
            string mpPremise = "MP�G";
            _magicPointText.text = mpPremise + _playerStatus.MaxMagicPoint;

            //��b�U���̓X�e�[�^�X
            string basicPowerPremise = "��b�U���́F";
            _basicPowerText.text = basicPowerPremise + _playerStatus.BasicPower;

            //�N���e�B�J�����X�e�[�^�X
            string criticalChancePremise = "�N���e�B�J�����F";
            _criticalChanceText.text = criticalChancePremise + _playerStatus.CriticalChance + "%";

            //�N���e�B�J���{���X�e�[�^�X
            string criticalMultiplePremise = "�N���e�B�J���{���F";
            _criticalMultipleText.text = criticalMultiplePremise + _playerStatus.CriticalMultiple + "%";

            //�ߐڍU���̓X�e�[�^�X
            string meleePowerPremise = "�ߐڍU���́F";
            _meleeAttackPowerText.text = meleePowerPremise + _playerStatus.MeleeAttackPower;

            //���@�U���̓X�e�[�^�X
            string magicPowerPremise = "���@�U���́F";
            _magicAttackPowerText.text = magicPowerPremise + _playerStatus.MagicattackPower;

            //�h��̓X�e�[�^�X
            string defencePowerPremise = "�h��́F";
            _defenceText.text = defencePowerPremise + _playerStatus.DefensePower;
        }
    }

    /// <summary>
    /// �A�C�e���������
    /// </summary>
    public void ListClosed()
    {
        Time.timeScale = 1;
    }


    public void HpBarAdaptation()
    {
        //hp�o�[�̓K��
        _hpBar.size = _playerStatus.HitPoint / _playerStatus.MaxHitPoint;

        HitPointTextchange();
    }

    public void MpBarAdaptation()
    {
        //Mp�o�[�̓K��
        _mpBar.size = _playerStatus.MagicPoint / _playerStatus.MaxMagicPoint;

        MagicPointTextchange();
    }

    private void HitPointTextchange()
    {

        string hpText = "Hp:";
        //Hp�e�L�X�g�̓K��
        _hpText.text = hpText  + _playerStatus.HitPoint.ToString("f0") + "/" + _playerStatus.MaxHitPoint;

    }

    private void MagicPointTextchange()
    {
        string mpText = "Mp:";
        //Mp�e�L�X�g�̓K��
        _mpText.text = mpText  + _playerStatus.MagicPoint.ToString("f0") + "/" + _playerStatus.MaxMagicPoint;
        
    }

    public void ParryDamageTextAppearance()
    {
        string ParryText = "Parry!!";
        //�g���K�[�����������ʒu�ɐ�������
        TextMeshProUGUI _damageTextDummy = Instantiate(_damageText, this.transform.position, Quaternion.identity);

        //�L�����o�X��e�ɂ���
        _damageTextDummy.transform.SetParent(_worldSpaceCanvas.transform, true);

        //�_���[�W�l���e�L�X�g�ɓ����
        _damageTextDummy.text = ParryText;

        //�e�L�X�g�𓮂���
        _damageTextDummy.GetComponent<DamageTextScript>().TextParryMove();
    }
    public void GuardDamageTextAppearance(float damage)
    {
        //�g���K�[�����������ʒu�ɐ�������
        _damageTextDummy = Instantiate(_damageText, this.gameObject.transform.position + new Vector3(0, 1, 0), Quaternion.identity);

        //�L�����o�X��e�ɂ���
        _damageTextDummy.transform.SetParent(_worldSpaceCanvas.transform, true);

        string _guardText = "Guard!!";

        //�_���[�W�l���e�L�X�g�ɓ����
        _damageTextDummy.text = _guardText  + " " + damage.ToString();

        _damageTextDummy.GetComponent<DamageTextScript>().TextDefaultMove();
    }

    public void UsuallydDamageTextAppearance(float damage)
    {
        //�g���K�[�����������ʒu�ɐ�������
        _damageTextDummy = Instantiate(_damageText, this.gameObject.transform.position + new Vector3(0, 1, 0), Quaternion.identity);

        //�L�����o�X��e�ɂ���
        _damageTextDummy.transform.SetParent(_worldSpaceCanvas.transform, true);

        //�_���[�W�l���e�L�X�g�ɓ����
        _damageTextDummy.text = damage.ToString();

        _damageTextDummy.GetComponent<DamageTextScript>().TextDefaultMove();
    }

    public void MagicPointRecoveryTextAppearance(float recoveryValue)
    {
        //�����̈ʒu�ɐ�������
        _damageTextDummy = Instantiate(_damageText,this.transform.position, Quaternion.identity);

        //�L�����o�X��e�ɂ���
        _damageTextDummy.transform.SetParent(_worldSpaceCanvas.transform, true);

        //�񕜒l���e�L�X�g�ɓ����
        _damageTextDummy.text = recoveryValue.ToString();

        _damageTextDummy.GetComponent<DamageTextScript>().TextMagicPointRecoveryMove();
    }
}
