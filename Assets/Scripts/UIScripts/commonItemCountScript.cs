using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class commonItemCountScript : MonoBehaviour
{
    string _itemCountString = "X";

    [SerializeField] Text _basicPowerText = default;
    private float _basicPowerCount = 0;//�U���̓A�C�e���̂Ƃ�����

    [SerializeField] Text _hpText = default;
    private float _hpCount = 0;//HP�A�C�e���̂Ƃ�����

    [SerializeField] Text _ChargePowerText = default;
    private float _ChargePowerCount = 0;//�`���[�W�{���A�C�e���̂Ƃ�����

    [SerializeField] Text _CriticalChanceText = default;
    private float _CriticalChanceCount = 0;//�N���e�B�J�����A�C�e���̂Ƃ�����

    [SerializeField] Text _CriticalMultipleText = default;
    private float _CriticalMultipleCount = 0;//�N���e�B�J���{���A�C�e���̂Ƃ�����

    [SerializeField] Text _defencePowerText = default;
    private float _defencePowerCount = 0;//�h��̓A�C�e���̂Ƃ�����

    [SerializeField] Text _magicPointText = default;
    private float _magicPointCount = 0;//���̓A�C�e���̂Ƃ�����

    private void Start()
    {
        //���ׂẴe�L�X�g��������
        _basicPowerText.text = _itemCountString + _basicPowerCount;
        _hpText.text = _itemCountString + _hpCount;
        _ChargePowerText.text = _itemCountString + _ChargePowerCount;
        _CriticalChanceText.text = _itemCountString + _CriticalChanceCount;
        _CriticalMultipleText.text = _itemCountString + _CriticalMultipleCount;
        _defencePowerText.text = _itemCountString + _defencePowerCount;
        _magicPointText.text = _itemCountString + _magicPointCount;
    }
    public void ItemCount(int itemNumber)
    {

        switch (itemNumber)
        {
            case 0://�U���̓e�L�X�g

                _basicPowerCount++;
                _basicPowerText.text = _itemCountString + _basicPowerCount;
                break;

            case 1://HP�e�L�X�g

                _hpCount++;
                _hpText.text = _itemCountString + _hpCount;
                break;

            case 2://�`���[�W�{���e�L�X�g

                _ChargePowerCount++;
                _ChargePowerText.text = _itemCountString + _ChargePowerCount;
                break;

            case 3://�N���e�B�J�����e�L�X�g

                _CriticalChanceCount++;
                _CriticalChanceText.text = _itemCountString + _CriticalChanceCount;

                break;

            case 4://�N���e�B�J���{���e�L�X�g

                _CriticalMultipleCount++;
                _CriticalMultipleText.text = _itemCountString + _CriticalMultipleCount;

                break;

            case 5://�h��̓e�L�X�g

                _defencePowerCount++;
                _defencePowerText.text = _itemCountString + _defencePowerCount;
                
                break;

            case 6://���̓e�L�X�g

                _magicPointCount++;
                _magicPointText.text = _itemCountString + _magicPointCount;

                break;

        }
    }
}
