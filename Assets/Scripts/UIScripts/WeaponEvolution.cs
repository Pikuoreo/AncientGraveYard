using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponEvolution : MonoBehaviour
{
    [SerializeField] private GameObject _evolutionImage=default;

    [SerializeField] private GameObject _meleeChoice = default;
    [SerializeField] private GameObject _magicChoice = default;

    [SerializeField] private Image _meleeLeftClickAttackButton = default;
    [SerializeField] private Image _meleeRightClickAttackButton = default;

    [SerializeField] private Image _magicLeftClickAttackButton = default;
    [SerializeField] private Image _magicRightClickAttackButton = default;

    [Header("���̃{�^���C���[�W"),SerializeField] private List<Sprite> _sordButtonImages = new List<Sprite>();
    [Header("���̃{�^���C���[�W"),SerializeField] private List<Sprite> _shieldButtonImages = new List<Sprite>();
    [Header("���i���@����̃{�^���C���[�W"),SerializeField] private List<Sprite> _magicStickButtonImages = new List<Sprite>();
    [Header("�ǔ����@����̃{�^���C���[�W"),SerializeField] private List<Sprite> _chaserCaneButtonImages = new List<Sprite>();

    private PlayerMoveControll playerMoveControll = default;

    private bool _isEvent = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //�C�x���g���s���肪false�Ȃ�
        if (!_isEvent)
        {
            string playerTag = "Player";

            if (collision.gameObject.CompareTag(playerTag))
            {
                //�v���C���[�𓮂��Ȃ�����
                playerMoveControll = collision.gameObject.GetComponent<PlayerMoveControll>();
                playerMoveControll.ControllOff();

                //�v���C���[�̐E�Ƃ���
                int playerProfession = collision.gameObject.GetComponent<PlayerAttack>().NowProfession();

                //�E�ƕ�
                switch (playerProfession)
                {
                    //���m��������
                    case 1:


                        //���m�p�̑I�������o��

                        _meleeLeftClickAttackButton.sprite = _sordButtonImages[GameObject.FindGameObjectWithTag("SordWeapon").
                                                         GetComponent<SordAttack>().
                                                         GiveWeaponLevel()];

                        _meleeRightClickAttackButton.sprite = _shieldButtonImages[GameObject.FindGameObjectWithTag("ShieldWeapon").
                                                          GetComponent<ShieldWeapon>().
                                                          GiveWeaponLevel()];
                        _meleeChoice.SetActive(true);
                        break;

                    //���@�g����������
                    case 2:
                        _magicLeftClickAttackButton.sprite = _magicStickButtonImages[GameObject.FindGameObjectWithTag("MagicStickWeapon").
                                                        GetComponent<MagicStickAttack>().
                                                        GiveWeaponLevel()];

                        _magicRightClickAttackButton.sprite = _chaserCaneButtonImages[GameObject.FindGameObjectWithTag("ChaserCaneWeapon").
                                                         GetComponent<ChaserCaneAttack>().
                                                         GiveWeaponLevel()];
                        //���@�g���p�̑I�������o��
                        _magicChoice.SetActive(true);
                        break;
                }

                //UI�S�̂��o��
                _evolutionImage.SetActive(true);
                _isEvent = true;

            }

            //�C�x���g���s�����true��
           
        }
       
    }

    public void EndEvolutionChoice()
    {
        playerMoveControll.ControllOn();

        //�I��UI�����ׂČ����Ȃ�����
        _meleeChoice.SetActive(false);
        _magicChoice.SetActive(false);
        _evolutionImage.SetActive(false);
    }
}
