using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommonItemScript : MonoBehaviour
{
    [Header("�A�C�e���ԍ�"), SerializeField] private int _itemID = 0;

    private float _powerUp = 25f;//�ߐڍU���̓A�b�v
    private float _hitPointUp = 25f;//���@�U���̓A�b�v
    private float _chargeMultipleUp = 0.1f;//�`���[�W�U���̔{���A�b�v
    private float _criticalChanceUp = 2f;//�N���e�B�J�����A�b�v
    private float _criticalMultipleUp = 10;//�N���e�B�J���{���A�b�v
    private float _defencePointUp = 5f;//�h��̓A�b�v
    private float _magicPointUp = 10f;

    private float _itemEffectMultiple = 1f; //�A�C�e�����ʂ̔{��

    private commonItemCountScript _countScript = default;

    private void Start()
    {
        string findTag = "StatusUI";
        _countScript = GameObject.FindGameObjectWithTag(findTag).GetComponent<commonItemCountScript>();
    }
    public void GiveToPlayer(PlayerStatusChange playerController)
    {
        switch (_itemID)
        {
            case 0://�U����up�A�C�e��

                playerController.AttackPowerUP(_powerUp * _itemEffectMultiple);
                _countScript.ItemCount(_itemID);
                break;

            case 1://�̗�up�A�C�e��

                playerController.hitPointUP(_hitPointUp * _itemEffectMultiple);
                _countScript.ItemCount(_itemID);
                break;

            case 2://�`���[�W��up�A�C�e��        

                playerController.ChargeMultipleUP(_chargeMultipleUp * _itemEffectMultiple);
                _countScript.ItemCount(_itemID);
                break;

            case 3://��S��up�A�C�e��

                playerController.CriticalChanceUP(_criticalChanceUp * _itemEffectMultiple);
                _countScript.ItemCount(_itemID);
                break;

            case 4://��S�З�up�A�C�e��

                playerController.CriticalMultipleUP(_criticalMultipleUp * _itemEffectMultiple);
                _countScript.ItemCount(_itemID);
                break;

            case 5://�h���up�A�C�e��

                playerController.DefencePowerUP(_defencePointUp * _itemEffectMultiple);
                _countScript.ItemCount(_itemID);
                break;

            case 6://����up�A�C�e��

                playerController.MagicPointUp(_magicPointUp * _itemEffectMultiple);
                _countScript.ItemCount(_itemID);
                break;
        }
    }
}
