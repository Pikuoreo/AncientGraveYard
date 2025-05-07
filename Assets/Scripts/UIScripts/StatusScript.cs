using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusScript : MonoBehaviour
{
    private Image _thisImage = default;
    private bool _isOpenList = false;//true�Ȃ烊�X�g���J���Ă���Afalse�Ȃ烊�X�g����Ă���

    [SerializeField] private PlayerStatusUI _playerStatus = default;
    [SerializeField] private GameObject _textParent = default;
    [SerializeField] private GameObject _weaponList = null;
    [SerializeField] private GameObject _itemList = default;
    [SerializeField] private ItemDescriptionScript _itemDescription = default;

    [SerializeField] private GameObject _floorText = default;

    [SerializeField] private PlayerMoveControll _moveController;
    [SerializeField] private PlayerAttack _playerAttack = default;

    // Start is called before the first frame update
    void Start()
    {
        _thisImage = this.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (_moveController.AliveJudge())
            {
                if (ListOpenJudge())
                {
                    ListOpen();
                }
                else
                {
                    ListClosed();
                }
            }
        }
    }

    /// <summary>
    /// ���X�g���J�������邩�̔����n��
    /// </summary>
    /// <returns></returns>
    private bool ListOpenJudge()
    {
        if (_isOpenList)
        {
            _isOpenList = false;
            return _isOpenList;
        }
        else
        {
            _isOpenList = true;
            return _isOpenList;
        }
    }

    /// <summary>
    /// ���X�g���J��
    /// </summary>
    private void ListOpen()
    {
        //�X�e�[�^�X�����J��
        _floorText.SetActive(false);
        _playerStatus.ListOpen();
        _textParent.SetActive(true);
        _isOpenList = true;
        _thisImage.enabled = true;

        //�v���C���[���U���ł��Ȃ��悤�ɂ���
        _playerAttack.ControllSwitch(false);

    }

    /// <summary>
    /// ���X�g�����
    /// </summary>
    private void ListClosed()
    {
        //�X�e�[�^�X�������
        _floorText.SetActive(true);
        _playerStatus.ListClosed();
        _textParent.SetActive(false);
        _itemList.SetActive(false);
        _weaponList.SetActive(false);

        //�v���C���[���U���ł���悤�ɂ���
        _playerAttack.ControllSwitch(true);

        //���������\������Ă��烊�Z�b�g����
        _itemDescription.Textreset();

        _isOpenList = false;
        _thisImage.enabled = false;
    }
}
