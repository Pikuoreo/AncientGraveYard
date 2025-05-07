using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponDescription : MonoBehaviour
{
    [SerializeField] private GameObject _weaponList = default;

    [Header("���N���b�N�U���̃C���[�W"), SerializeField] private Image _leftClickAttackImage = default;
    [Header("�E�N���b�N�U���̃C���[�W"), SerializeField] private Image _rightClickAttackImage = default;
    [Header("�������̃C���[�W"), SerializeField] private Image _descriptionTextImage = default;

    [SerializeField] private PlayerAttack _playerProffesion = default;

    private int _sordLevel = default;
    private int _shieldLevel = default;
    private int _magicStickLevel = default;
    private int _chaserCaneLevel = default;

    private int _playerProfessionNumber = default;

    [Header("���̃��X�g�A�C�e���摜"), SerializeField] List<Sprite> _sordItemImages=new List<Sprite>();
    [Header("���̐����e�L�X�g�摜"), SerializeField] List<Sprite> _sordDescriptionTextImages = new List<Sprite>();


    [Header("���̃��X�g�A�C�e���摜"), SerializeField] List<Sprite> _shieldItemImages=new List<Sprite>();
    [Header("���̐����e�L�X�g�摜"), SerializeField] List<Sprite> _shieldDescriptionTextImages = new List<Sprite>();

    [Header("���i���@����̃��X�g�A�C�e���摜"), SerializeField] List<Sprite> _magicStickItemImages=new List<Sprite>();
    [Header("���i���@����̐����e�L�X�g�摜"), SerializeField] List<Sprite> _magicStickDescriptionTextImages = new List<Sprite>();

    [Header("�ǔ����@����̃��X�g�A�C�e���摜"), SerializeField] List<Sprite> _chaserCaneItemImages=new List<Sprite>();
    [Header("�ǔ����@����̐����e�L�X�g�摜"), SerializeField] List<Sprite> _chaserCaneDescriptionTextImages = new List<Sprite>();
    //���m�̎��ʔԍ�
    const int MELEE_PROFESSION_NUMBER = 1;
    //���@�g���̎��ʔԍ�
    const int MAGIC_PROFESSION_NUMBER = 2;


    /// <summary>
    /// ���N���b�N�U���̐���
    /// </summary>
    public void LeftClickAttackDescription()
    {
        //���m��������
        if (_playerProfessionNumber == MELEE_PROFESSION_NUMBER)
        {
            _descriptionTextImage.sprite = _sordDescriptionTextImages[_sordLevel];
        }
        //���@�g����������
        else if (_playerProfessionNumber == MAGIC_PROFESSION_NUMBER)
        {
            _descriptionTextImage.sprite = _magicStickDescriptionTextImages[_magicStickLevel];
        }
    }

    /// <summary>
    /// �E�N���b�N�U���̐���
    /// </summary>
    public void RightClickAttackDescription()
    {
        //���m��������
        if (_playerProfessionNumber == MELEE_PROFESSION_NUMBER)
        {
            _descriptionTextImage.sprite = _shieldDescriptionTextImages[_shieldLevel];
        }
        //���@�g����������
        else if (_playerProfessionNumber == MAGIC_PROFESSION_NUMBER)
        {
            _descriptionTextImage.sprite = _chaserCaneDescriptionTextImages[_chaserCaneLevel];
        }
    }

    /// <summary>
    /// �E�ƕʂ̃{�^���\��&�E�ƕʂ̌��݂̕��탌�x���擾
    /// </summary>
    public void ProfessionDescriptionDisplay()
    {
        //�v���C���[�̍��̐E�Ƃ��Q��
         _playerProfessionNumber = _playerProffesion.NowProfession();

        //���m��������
        if (_playerProfessionNumber == MELEE_PROFESSION_NUMBER)
        {
            //���̌��݂̃��x�����擾
            string findName = "SordWeapon";
            _sordLevel = GameObject.FindGameObjectWithTag(findName).GetComponent<SordAttack>().GiveWeaponLevel();

            //���N���b�N�U���̃C���[�W�����݂̌��̃��x���ɍ��킹���A�C�e���摜�ɕύX
            _leftClickAttackImage.sprite = _sordItemImages[_sordLevel];


            //���̌��݂̃��x�����擾
            findName = "ShieldWeapon";
            _shieldLevel = GameObject.FindGameObjectWithTag(findName).GetComponent<ShieldWeapon>().GiveWeaponLevel();

            //�E�N���b�N�U���̃C���[�W�����݂̏��̃��x���ɍ��킹���A�C�e���摜�ɕύX
            _rightClickAttackImage.sprite = _shieldItemImages[_shieldLevel];

            //�����̃��X�g���I��
            _weaponList.SetActive(true);
        }
        //���@�g����������
        else if (_playerProfessionNumber == MAGIC_PROFESSION_NUMBER)
        {
            

            //���i���@����̌��݂̃��x�����擾
            string findName = "MagicStickWeapon";
            _magicStickLevel = GameObject.FindGameObjectWithTag(findName).GetComponent<MagicStickAttack>().GiveWeaponLevel();

            //���N���b�N�U���̃C���[�W�����݂̒��i���@����̃��x���ɍ��킹���A�C�e���摜�ɕύX
            _leftClickAttackImage.sprite = _magicStickItemImages[_magicStickLevel];


            //�ǔ����@����̌��݂̃��x�����擾
            findName = "ChaserCaneWeapon";
            _chaserCaneLevel = GameObject.FindGameObjectWithTag(findName).GetComponent<ChaserCaneAttack>().GiveWeaponLevel();

            //�E�N���b�N�U���̃C���[�W�����݂̒ǔ����@����̃��x���ɍ��킹���A�C�e���摜�ɕύX
            _rightClickAttackImage.sprite = _chaserCaneItemImages[_chaserCaneLevel];

            //�����̃��X�g���I��
            _weaponList.SetActive(true);
        }

        DescriptionTextImageReset();
    }

    public void DescriptionTextImageReset()
    {
        _descriptionTextImage.sprite = default;
    }
}
