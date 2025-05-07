using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class commonItemCountScript : MonoBehaviour
{
    string _itemCountString = "X";

    [SerializeField] Text _basicPowerText = default;
    private float _basicPowerCount = 0;//攻撃力アイテムのとった数

    [SerializeField] Text _hpText = default;
    private float _hpCount = 0;//HPアイテムのとった数

    [SerializeField] Text _ChargePowerText = default;
    private float _ChargePowerCount = 0;//チャージ倍率アイテムのとった数

    [SerializeField] Text _CriticalChanceText = default;
    private float _CriticalChanceCount = 0;//クリティカル率アイテムのとった数

    [SerializeField] Text _CriticalMultipleText = default;
    private float _CriticalMultipleCount = 0;//クリティカル倍率アイテムのとった数

    [SerializeField] Text _defencePowerText = default;
    private float _defencePowerCount = 0;//防御力アイテムのとった数

    [SerializeField] Text _magicPointText = default;
    private float _magicPointCount = 0;//魔力アイテムのとった数

    private void Start()
    {
        //すべてのテキストを初期化
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
            case 0://攻撃力テキスト

                _basicPowerCount++;
                _basicPowerText.text = _itemCountString + _basicPowerCount;
                break;

            case 1://HPテキスト

                _hpCount++;
                _hpText.text = _itemCountString + _hpCount;
                break;

            case 2://チャージ倍率テキスト

                _ChargePowerCount++;
                _ChargePowerText.text = _itemCountString + _ChargePowerCount;
                break;

            case 3://クリティカル率テキスト

                _CriticalChanceCount++;
                _CriticalChanceText.text = _itemCountString + _CriticalChanceCount;

                break;

            case 4://クリティカル倍率テキスト

                _CriticalMultipleCount++;
                _CriticalMultipleText.text = _itemCountString + _CriticalMultipleCount;

                break;

            case 5://防御力テキスト

                _defencePowerCount++;
                _defencePowerText.text = _itemCountString + _defencePowerCount;
                
                break;

            case 6://魔力テキスト

                _magicPointCount++;
                _magicPointText.text = _itemCountString + _magicPointCount;

                break;

        }
    }
}
